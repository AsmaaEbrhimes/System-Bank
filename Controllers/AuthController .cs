
using Banking_System.Data;
using Banking_System.Model;
using Eccomarce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Eccomarce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ContextApi db;
        private readonly IConfiguration configuration;

        public AuthController(ContextApi db, IConfiguration configuration)
        {
            this.db = db;
            this.configuration = configuration;
        }




        [HttpPost("Register")]
        public async Task<IActionResult> Register(Register dto)
        {
            if (await db.Users.AnyAsync(x => x.Email == dto.Email))
                return BadRequest("Email already exists");

            User user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Ok("Registered Successfully");
        }






        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login dto)
        {
            var user = await db.Users.FirstOrDefaultAsync(x =>
                x.Email == dto.Email &&
                x.Password == dto.Password);

            if (user == null)
                return Unauthorized("Invalid Email or Password");

            var token = GenerateToken(user);

            return Ok(new
            {
                Token = token
            });
        }



        private string GenerateToken(User user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Name,user.Name),
            new Claim(ClaimTypes.Email,user.Email)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
