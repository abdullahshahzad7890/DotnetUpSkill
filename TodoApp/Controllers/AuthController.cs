using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using TodoApp.Dto;
using TodoApp.Enums;
using TodoApp.Models;
using TodoApp.Repository;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly Interfaces.IUser _userRepository;
        public AuthController(IConfiguration configuration, Interfaces.IUser userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        [HttpPost("register")]

        public async Task<ActionResult<User>> Register(UserDto request)
        {
            bool userExists = await _userRepository.UserExists(request.Username);

            if (userExists)
            {
                return BadRequest("User already exists");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Username = request.Username,
                isAdmin = request.isAdmin,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            bool isUserCreated = await _userRepository.CreateUser(user);


            if (!isUserCreated)
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var user = _userRepository.GetUserByUsername(request.Username);

            if (user.Username != request.Username)
            {
                return BadRequest("User not found");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }

            string token = CreateToken(user);
            return Ok(token);
        }

        private string CreateToken(User user)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())

            };
            if (user.isAdmin == true)
            {
                claims.Add(new Claim(ClaimTypes.Role, UserRole.Admin.ToString()));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, UserRole.User.ToString()));
            }

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(

                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }

        }

    }
}
