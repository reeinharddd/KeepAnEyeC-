using KeepAnEye.Models;
using KeepAnEye.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeepAnEye.Controllers
{
    [ApiController]
    [Route("payment/[controller]")]
    public class PaymentController : Controller
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(Payments payment)
        {
            if (payment == null)
            {
                return BadRequest("Payment is null.");
            }

            var paymentId = await _paymentService.CreatePaymentAsync(payment);

            return CreatedAtAction(nameof(CreatePayment), new { id = paymentId }, new { id = paymentId });
        }
    }
}
