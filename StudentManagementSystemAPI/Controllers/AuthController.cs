using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentManagementSystemAPI.Data;
using StudentManagementSystemAPI.Models;
using StudentManagementSystemAPI.Models.DTO;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginDTO loginData)
        {
            var authUser = AuthenticateUser(loginData);

            if (authUser != null)
            {
                var token = GenerateToken(authUser);

                var userObj = new {
                    id = authUser.Id,
                    username = authUser.Username
                };

                return Ok(new { user = userObj, utoken = token });
            }

            return StatusCode(401, "Invalid Credentials");
        }

        private User AuthenticateUser(LoginDTO loginData)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == loginData.Username);

            if(user != null)
            {
                if(loginData.Password == user.Password)
                {
                    return user;
                }

                return null;
            }

            return null;
        }

        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                // Add user ID as a claim
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
