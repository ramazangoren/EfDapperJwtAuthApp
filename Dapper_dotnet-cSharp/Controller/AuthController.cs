using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using dotnet_cSharp.Data;
using dotnet_cSharp.DTOs;
using dotnet_cSharp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_cSharp.Controller
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DapperContext _dapper;
        // private readonly IConfiguration _config; 
        //we were using _config for GetPasswordHash and CreateToken methods
        //since we we added them to helper file we dont need _config anymore. we are using it in AuthHelper class
        private readonly AuthHelper _authHelper;

        public AuthController(IConfiguration config)
        {
            _dapper = new DapperContext(config);
            _authHelper = new AuthHelper(config);
            // _config = config; // explanation is above
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistrationDto)
        {
            if (userForRegistrationDto.Password == userForRegistrationDto.PasswordConfirm)
            {
                string sqlCheckUserExists =
                    "SELECT Email FROM CSharpDotNetFirstProject.Auth WHERE Email = '"
                    + userForRegistrationDto.Email
                    + "'";
                IEnumerable<string> existingUser = _dapper.LoadData<string>(sqlCheckUserExists);
                if (existingUser.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(
                        userForRegistrationDto.Password,
                        passwordSalt
                    );
                    string sqlAddAuth =
                        @"
                        INSERT INTO CSharpDotNetFirstProject.Auth  (Email,
                        PasswordHash,
                        PasswordSalt) VALUES ('"
                        + userForRegistrationDto.Email
                        + "', @PasswordHash, @PasswordSalt)";
                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter passwordSaltParameter = new SqlParameter(
                        "@PasswordSalt",
                        SqlDbType.VarBinary
                    );
                    passwordSaltParameter.Value = passwordSalt;

                    SqlParameter passwordHashParameter = new SqlParameter(
                        "@PasswordHash",
                        SqlDbType.VarBinary
                    );
                    passwordHashParameter.Value = passwordHash;

                    sqlParameters.Add(passwordSaltParameter);
                    sqlParameters.Add(passwordHashParameter);
                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                    {
                        string sqlAddUser =
                            @"
                            INSERT INTO CSharpDotNetFirstProject.Users ([FirstName],[LastName],[Email],[Gender],[Active]) 
                            VALUES (@FirstName,@LastName,@Email,@Gender,@Active)";

                        // Define parameters to pass to Dapper
                        var parameters = new
                        {
                            FirstName = userForRegistrationDto.FirstName,
                            LastName = userForRegistrationDto.LastName,
                            Email = userForRegistrationDto.Email,
                            Gender = userForRegistrationDto.Gender,
                            Active = 1,
                        };
                        bool result = _dapper.ExecuteSql(sqlAddUser, parameters);
                        if (result)
                        {
                            return Ok("user registered and added to the table");
                        }
                        throw new Exception("Failed to add user.");
                    }
                    throw new Exception("failed to register user");
                }
                throw new Exception("user with this email already exists");
            }
            throw new Exception("passwords do not match");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLoginDto)
        {
            string sqlForHashAndSalt =
                @"SELECT 
                [PasswordHash],
                [PasswordSalt] FROM CSharpDotNetFirstProject.Auth WHERE Email = '"
                + userForLoginDto.Email
                + "'";

            UserForLoginConfirmationDto userForConfirmationDto =
                _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);
            byte[] passwordHash = _authHelper.GetPasswordHash(
                userForLoginDto.Password,
                userForConfirmationDto.PasswordSalt
            );

            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConfirmationDto.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect password!");
                }
            }
            string userIdSql =
                @"SELECT UserId FROM CSharpDotNetFirstProject.Users WHERE Email = '"
                + userForLoginDto.Email
                + "'";
            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(
                new Dictionary<string, string> { { "token", _authHelper.CreateToken(userId) } }
            );
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + ""; // User is from controllerbase
            string userIdSql =
                @"SELECT UserId FROM CSharpDotNetFirstProject.Users WHERE UserId =" + userId;
            int userIdFromDb = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(
                new Dictionary<string, string>
                {
                    { "token", _authHelper.CreateToken(userIdFromDb) },
                }
            );
        }
    }
}
