using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_cSharp.Data;
using dotnet_cSharp.DTOs;
using dotnet_cSharp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_cSharp.Controller
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        DapperContext _dapper;

        public PostController(IConfiguration config)
        {
            _dapper = new DapperContext(config);
        }

        [HttpGet("GetPosts")]
        public IEnumerable<Post> GetPosts()
        {
            string sql = @"SELECT * FROM CSharpDotNetFirstProject.Posts ";
            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("GetSinglePost/{PostId}")]
        public Post GetSinglePost(int PostId)
        {
            string sql = @"SELECT * FROM CSharpDotNetFirstProject.Posts WHERE PostId = " + PostId;
            return _dapper.LoadDataSingle<Post>(sql);
        }

        [HttpGet("GetPostByUser/{UserId}")]
        public IEnumerable<Post> GetPostByUser(int UserId)
        {
            string sql = @"SELECT * FROM CSharpDotNetFirstProject.Posts WHERE UserId = " + UserId;
            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("GetMyPosts")]
        public IEnumerable<Post> GetMyPosts(int UserId)
        {
            string sql =
                @"SELECT * FROM CSharpDotNetFirstProject.Posts WHERE UserId = "
                + this.User.FindFirst("userId").Value; // what is user in here
            return _dapper.LoadData<Post>(sql);
        }

        [HttpPost("Post")]
        public IActionResult AddPost(PostToAddDto postToAdd)
        {
            string sqlAddPost =
                @"
                INSERT INTO CSharpDotNetFirstProject.Posts ([UserId],[PostTitle],[PostContent],[PostCreated],[PostUpdated]) 
                VALUES (@UserId,@PostTitle,@PostContent,GETDATE(),GETDATE())";
            var parameters = new
            {
                UserId = int.Parse(this.User.FindFirst("userId")?.Value ?? "0"),
                PostTitle = postToAdd.PostTitle,
                PostContent = postToAdd.PostContent,
            };
            bool result = _dapper.ExecuteSql(sqlAddPost, parameters);
            if (result)
            {
                return Ok();
            }

            throw new Exception("Failed to create new post!");
        }

        [HttpPut("Post")]
        public IActionResult EditPost(PostToEditDto postToEdit)
        {
            string sql =
                @"UPDATE CSharpDotNetFirstProject.Posts 
                SET PostContent = @PostContent,PostTitle = @PostTitle,PostUpdated = GETDATE()
                WHERE PostId = @PostId AND UserId = @UserId";

            var parameters = new
            {
                PostContent = postToEdit.PostContent,
                PostTitle = postToEdit.PostTitle,
                PostId = postToEdit.PostId,
                UserId = int.Parse(this.User.FindFirst("userId")?.Value ?? "0"),
            };

            bool result = _dapper.ExecuteSql(sql, parameters);

            if (result)
            {
                return Ok();
            }

            throw new Exception("Failed to edit post!");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql =
                @"DELETE FROM CSharpDotNetFirstProject.Posts WHERE PostId = @PostId AND  UserId = @UserId";
            var parameters = new
            {
                PostId = postId,
                UserId = int.Parse(this.User.FindFirst("userId")?.Value ?? "0"),
            };
            bool result = _dapper.ExecuteSql(sql, parameters);

            if (result)
            {
                return Ok();
            }

            throw new Exception("Failed to delete post!");
        }

        [HttpGet("PostsBySearch/{searchParam}")]
        public IEnumerable<Post> PostsBySearch(string searchParam)
        {
            string sql =
                @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM CSharpDotNetFirstProject.Posts
                    WHERE PostTitle LIKE '%"
                + searchParam
                + "%'"
                + " OR PostContent LIKE '%"
                + searchParam
                + "%'";

            return _dapper.LoadData<Post>(sql);
        }
    }
}
