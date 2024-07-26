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
using KeepAnEye.Entities;
using MongoDB.Driver;

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
            // Validar campos obligatorios
            if (string.IsNullOrEmpty(user.Name.FirstName) || string.IsNullOrEmpty(user.Name.LastName) ||
                string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("First Name, Last Name, Email, and Password are required.");
            }

            user.UserType = "admin";

            // Verificar si el usuario ya existe por correo electrónico
            var existingUser = _mongoDbService.GetUsers().FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return Conflict("User with the same email already exists.");
            }

            // Ajustar el campo Subscription
            if (user.Subscription == ObjectId.Empty)
            {
                user.Subscription = null; // Para manejar internamente como null
            }

            // Eliminar Subscription si es null
            if (user.Subscription == null)
            {
                user.Subscription = null;
            }

            // Filtrar pacientes con IDs vacíos
            user.Patients = user.Patients
                .Where(p => p.PatientId != ObjectId.Empty)
                .Select(p =>
                {
                    p.PatientId = p.PatientId == ObjectId.Empty ? null : p.PatientId;
                    return p;
                })
                .ToList();


            // Log de datos recibidos
            Console.WriteLine("Subscription ID: " + user.Subscription);
            foreach (var patient in user.Patients)
            {
                Console.WriteLine("Patient ID: " + patient.PatientId);
                Console.WriteLine("Relationship: " + patient.Relationship);
            }

            // Crear el usuario en la base de datos
            _mongoDbService.CreateUser(user);

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            // Busca al usuario en la base de datos por correo electrónico
            var user = _mongoDbService.GetUsers()
                                       .FirstOrDefault(u => u.Email == loginModel.Email);

            // Verifica si el usuario existe
            if (user == null)
            {
                return Unauthorized("Credenciales incorrectas.");
            }


            // Genera un token JWT
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token, UserId = user.Id.ToString() });
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult Profile()
        {
            // Obtiene el ID del usuario del token JWT
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Convierte el ID del usuario de string a ObjectId
            if (!ObjectId.TryParse(userId, out var objectId))
            {
                return Unauthorized("Invalid token.");
            }

            // Busca el usuario en la base de datos
            var user = _mongoDbService.GetUser(objectId);

            // Verifica si el usuario existe
            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }

            // Devuelve el perfil del usuario
            return Ok(user);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUser(string id)
        {
            // Convierte el ID de string a ObjectId
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format.");
            }

            var user = _mongoDbService.GetUser(objectId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpPut("updateUser/{id}")]
        public IActionResult UpdateUser(string id, [FromBody] User updateUser)
        {
            // Convierte el ID de string a ObjectId
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format.");
            }

            // Construye el filtro para el usuario
            var filter = Builders<User>.Filter.Eq(user => user.Id, objectId);

            // Construye la definición de actualización
            var update = Builders<User>.Update
                .Combine(
                    !string.IsNullOrEmpty(updateUser.Address?.Street) ? Builders<User>.Update.Set(user => user.Address.Street, updateUser.Address.Street) : Builders<User>.Update.Set(user => user.Address.Street, null),
                    !string.IsNullOrEmpty(updateUser.Address?.City) ? Builders<User>.Update.Set(user => user.Address.City, updateUser.Address.City) : Builders<User>.Update.Set(user => user.Address.City, null),
                    !string.IsNullOrEmpty(updateUser.Address?.State) ? Builders<User>.Update.Set(user => user.Address.State, updateUser.Address.State) : Builders<User>.Update.Set(user => user.Address.State, null),
                    !string.IsNullOrEmpty(updateUser.Address?.Zip) ? Builders<User>.Update.Set(user => user.Address.Zip, updateUser.Address.Zip) : Builders<User>.Update.Set(user => user.Address.Zip, null),
                    !string.IsNullOrEmpty(updateUser.Phone) ? Builders<User>.Update.Set(user => user.Phone, updateUser.Phone) : Builders<User>.Update.Set(user => user.Phone, null),
                    !string.IsNullOrEmpty(updateUser.Password) ? Builders<User>.Update.Set(user => user.Password, BCrypt.Net.BCrypt.HashPassword(updateUser.Password)) : Builders<User>.Update.Set(user => user.Password, null)
                );

            // Actualiza el usuario en la base de datos
            var result = _mongoDbService.GetUsersCollection().UpdateOne(filter, update);

            // Verifica si la actualización fue exitosa
            if (result.ModifiedCount == 0)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Obtiene el usuario actualizado
            var updatedUser = _mongoDbService.GetUser(objectId);
            return Ok(updatedUser);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); // Asegúrate de que la clave tenga al menos 32 bytes

            if (key.Length < 32) // Verifica que la clave tenga al menos 32 bytes
            {
                throw new Exception("La clave de firma JWT debe tener al menos 32 bytes.");
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Convertir ObjectId a string
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
