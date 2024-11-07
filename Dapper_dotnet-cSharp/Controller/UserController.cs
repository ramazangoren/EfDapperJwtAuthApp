using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_cSharp.Data;
using dotnet_cSharp.DTOs;
using dotnet_cSharp.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_cSharp.controller
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        DapperContext _dapper;

        public UserController(IConfiguration config)
        {
            _dapper = new DapperContext(config);
        }

        [HttpGet("GetUsers")]
        public IEnumerable<User> GetUsers()
        {
            string sql = @"SELECT * FROM [CSharpDotNetFirstProject].[dbo].[Users] ";
            IEnumerable<User> Users = _dapper.LoadData<User>(sql);
            return Users;
        }

        [HttpGet("GetSingleUser/{UserId:int}")]
        public User GetSingleUser(int UserId)
        {
            string sql =
                @"SELECT * FROM [CSharpDotNetFirstProject].[dbo].[Users] where UserId = "+UserId;
            User User = _dapper.LoadDataSingle<User>(sql);
            return User;
        }

        [HttpPut("UpdateUser/{UserId:int}")]
        public IActionResult UpdateUser(UserDto userDto, int UserId)
        {
            string sql =
                @"
                UPDATE [CSharpDotNetFirstProject].[dbo].[Users]
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    Gender = @Gender
                WHERE UserId = @UserId";

            var parameters = new
            {
                userDto.FirstName,
                userDto.LastName,
                userDto.Email,
                userDto.Gender,
                UserId,
            };
            return _dapper.ExecuteSql(sql, parameters)
                ? Ok("User updated successfully.")
                : BadRequest("Failed to update user.");
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserDto userDto)
        {
            string sql =
                @"INSERT INTO Users (FirstName, LastName, Email, Gender)
                  VALUES (@FirstName, @LastName, @Email, @Gender)";
            var parameters = new
            {
                userDto.FirstName,
                userDto.LastName,
                userDto.Email,
                userDto.Gender,
            };

            return _dapper.ExecuteSql(sql, parameters)
                ? Ok("User added successfully.")
                : BadRequest("Failed to add user.");
        }

        [HttpDelete("DeleteUser/{UserId:int}")]
        public IActionResult DeleteUser(int UserId)
        {
            string sql = "DELETE FROM Users WHERE UserId =@UserId";
            var parameter = new { UserId };
            return _dapper.ExecuteSql(sql, parameter)
                ? Ok("User deleted successfully.")
                : BadRequest("User doesn't exist.");
        }

        [HttpGet("GetJobInfo")]
        public IEnumerable<UserJobInfo> GetJobInfo()
        {
            string sql = @"SELECT * FROM [CSharpDotNetFirstProject].[dbo].[UserJobInfo]";
            IEnumerable<UserJobInfo> Users = _dapper.LoadData<UserJobInfo>(sql);
            return Users;
        }

        [HttpGet("GetSingleJobInfo/{UserId:int}")]
        public UserJobInfo GetSingleJobInfor(int UserId)
        {
            string sql =
                @"SELECT * FROM [CSharpDotNetFirstProject].[dbo].[UserJobInfo] where UserId = "+UserId;
            UserJobInfo User = _dapper.LoadDataSingle<UserJobInfo>(sql);
            return User;
        }

        [HttpPut("UpdateJobInfo/{UserId:int}")]
        public IActionResult UpdateJobInfo(UserJobInfo userJobInfo, int UserId)
        {
            string sql =
                @"
                UPDATE [CSharpDotNetFirstProject].[dbo].[UserJobInfo]
                SET JobTitle = @JobTitle,
                    Department = @Department
                WHERE UserId = @UserId";

            var parameters = new
            {
                userJobInfo.JobTitle,
                userJobInfo.Department,
                UserId,
            };
            return _dapper.ExecuteSql(sql, parameters)
                ? Ok("UserJobInfo updated successfully.")
                : BadRequest("Failed to update UserJobInfo.");
        }

        [HttpPost("AddJobInfo")]
        public IActionResult AddJobInfo(UserJobInfo userJobInfo)
        {
            string sql =
                @"INSERT INTO UserJobInfo (JobTitle, Department) VALUES (@JobTitle, @Department)";
            var parameters = new { userJobInfo.JobTitle, userJobInfo.Department };

            return _dapper.ExecuteSql(sql, parameters)
                ? Ok("UserJobInfo added successfully.")
                : BadRequest("Failed to add UserJobInfo.");
        }

        [HttpDelete("DeleteJobInfo/{UserId:int}")]
        public IActionResult DeleteJobInfo(int UserId)
        {
            string sql = "DELETE FROM UserJobInfo WHERE UserId =@UserId";
            var parameter = new { UserId };
            return _dapper.ExecuteSql(sql, parameter)
                ? Ok("UserJobInfo deleted successfully.")
                : BadRequest("UserJobInfo doesn't exist.");
        }
    }
}
