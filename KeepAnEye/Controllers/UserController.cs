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
using MongoDB.Bson;

namespace KeepAnEye.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(MongoDbService mongoDbService, UserService userService, IConfiguration configuration)
        {
            _mongoDbService = mongoDbService;
            _userService = userService;
            _configuration = configuration;
        }

        [HttpGet("check-connection")]
        public async Task<IActionResult> CheckConnection()
        {
            // Verifica la conexión a la base de datos de forma asincrónica si es necesario
            return Ok("La conexión a la base de datos está funcionando correctamente.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User data is null");
            }

            // Comprobar si el correo electrónico ya está en uso
            var existingUser = (await _userService.GetUsersAsync()).FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "El correo electrónico ya está en uso." });
            }
            Console.WriteLine("Datos del usuario: " + Newtonsoft.Json.JsonConvert.SerializeObject(user));
            try
            {
                // Generar nuevo ID para el usuario
                user.Id = ObjectId.GenerateNewId().ToString();
                user.CreatedAt = DateTime.UtcNow;

                // Crear usuario en la base de datos
                await _userService.CreateUserAsync(user);

                // Devolver respuesta con la información del nuevo usuario
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                // Loguear el error para su análisis
                Console.WriteLine($"Error al registrar usuario: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _userService.GetUserAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            // Busca al usuario en la base de datos por correo electrónico usando UserService de forma asincrónica
            var user = (await _userService.GetUsersAsync()).FirstOrDefault(u => u.Email == loginModel.Email);

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst("id")?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserAsync(userId); // Esperar asincrónicamente
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
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
        [HttpPost("{userId}/patients")]
        public async Task<IActionResult> AddPatientToUser(string userId, [FromBody] AddPatientModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.GetUserAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var patient = await _userService.GetUserAsync(model.PatientId);
            if (patient == null || patient.UserType != "patient")
            {
                return NotFound("Patient not found or not a valid patient.");
            }

            await _userService.AddPatientToUserAsync(userId, model.PatientId, model.Relationship);

            return NoContent();
        }

        public class AddPatientModel
        {
            public required string PatientId { get; set; }
            public required string Relationship { get; set; }
        }
        // [HttpPost("upload-photo")]
        // public async Task<IActionResult> UploadPhoto(IFormFile profilePhoto)
        // {
        //     if (profilePhoto == null || profilePhoto.Length == 0)
        //     {
        //         return BadRequest(new { message = "No file uploaded" });
        //     }

        //     try
        //     {
        //         using (var stream = profilePhoto.OpenReadStream())
        //         {
        //             var fileUrl = await _googleDriveService.UploadFileAsync(stream, profilePhoto.FileName);
        //             return Ok(new { url = fileUrl });
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         // Agrega un registro detallado del error
        //         Console.WriteLine($"Error al subir la foto: {ex.Message}");
        //         return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al subir la foto", details = ex.Message });
        //     }
        // }



        // private async Task<string> SavePhotoLocally(IFormFile file)
        // {
        //     var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        //     if (!Directory.Exists(uploadsFolder))
        //     {
        //         Directory.CreateDirectory(uploadsFolder);
        //     }

        //     var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        //     var fileExtension = Path.GetExtension(file.FileName);
        //     var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{fileExtension}";
        //     var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //     using (var fileStream = new FileStream(filePath, FileMode.Create))
        //     {
        //         await file.CopyToAsync(fileStream);
        //     }

        //     var url = $"/uploads/{uniqueFileName}";
        //     return url;
        // }



    }
}
