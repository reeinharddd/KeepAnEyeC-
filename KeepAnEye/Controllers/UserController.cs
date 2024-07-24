using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using KeepAnEye.Models;
using KeepAnEye.Services;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;

namespace KeepAnEye.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;
        private readonly IConfiguration _configuration;

        public UserController(MongoDbService mongoDbService, IConfiguration configuration)
        {
            _mongoDbService = mongoDbService;
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
            // Log de datos recibidos
            Console.WriteLine("Subscription ID: " + user.Subscription);
            foreach (var patient in user.Patients)
            {
                Console.WriteLine("Patient ID: " + patient.PatientId);
                Console.WriteLine("Relationship: " + patient.Relationship);
            }

            // Validaciones




            _mongoDbService.CreateUser(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }



        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var user = _mongoDbService.GetUsers()
                                       .FirstOrDefault(u => u.Email == loginModel.Email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
            {
                return Unauthorized("Invalid credentials.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _mongoDbService.GetUser(userId);
            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }

            return Ok(user);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUser(string id)
        {
            var user = _mongoDbService.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public class LoginModel
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }
    }
}
