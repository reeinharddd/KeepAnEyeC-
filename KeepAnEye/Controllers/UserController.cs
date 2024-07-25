// Controllers/UserController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using KeepAnEye.Models;
using KeepAnEye.Services;
using Microsoft.AspNetCore.Authorization;

namespace KeepAnEye.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpGet("check-connection")]
        public IActionResult CheckConnection()
        {
            return Ok("La conexión a la base de datos está funcionando correctamente.");
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            _userService.CreateUser(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var user = _userService.GetUsers()
                                   .FirstOrDefault(u => u.Email == loginModel.Email);

            if (user == null)
            {
                return Unauthorized("Credenciales incorrectas.");
            }

            if (loginModel.Password != user.Password) // Aquí puedes usar un método de comparación diferente si es necesario
            {
                return Unauthorized("Credenciales incorrectas.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirst("id")?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = _userService.GetUser(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }


        [HttpGet("{id}")]
        public ActionResult<User> GetUser(string id)
        {
            var user = _userService.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(string id, [FromBody] User userIn)
        {
            var user = _userService.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }

            _userService.UpdateUser(id, userIn);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(string id)
        {
            var user = _userService.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }

            _userService.DeleteUser(id);
            return NoContent();
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Name.FirstName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim("id", user.Id)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "default_secret_key"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }




        public class LoginModel
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }
    }
}
