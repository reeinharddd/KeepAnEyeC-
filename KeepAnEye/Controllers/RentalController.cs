using KeepAnEye.Models;
using KeepAnEye.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeepAnEye.Controllers
{
    [ApiController]
    [Route("Rental/[controller]")]
    public class RentalController : Controller
    {
        private readonly RentalService _rentalService;

        public RentalController(RentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRental(Rental rental)
        {
            if (rental == null)
            {
                return BadRequest("Rental is null.");
            }

            await _rentalService.CreateRentalAsync(rental);

            return CreatedAtAction(nameof(CreateRental), new { id = rental.Id }, rental);
        }
    }
}
