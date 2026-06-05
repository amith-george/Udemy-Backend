using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Razorpay.Api; 
using UdemyBackend.Data;
using UdemyApi.DTOs;
using UdemyBackend.Models;

namespace UdemyApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CreateCheckoutOrder()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return BadRequest(new { message = "Student profile not found." });

            var cartItems = await _context.Carts
                .Where(c => c.StudentId == student.Id)
                .Include(c => c.Course) 
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest(new { message = "Your cart is empty." });

            decimal totalAmount = cartItems.Sum(item => item.Course.Price);

            string keyId = _configuration["Razorpay:KeyId"]!;
            string keySecret = _configuration["Razorpay:KeySecret"]!;
            RazorpayClient client = new RazorpayClient(keyId, keySecret);

            Dictionary<string, object> options = new Dictionary<string, object>
            {
                { "amount", (int)(totalAmount * 100) }, 
                { "currency", "INR" },
                { "receipt", $"receipt_u_{userId}_{DateTime.UtcNow.Ticks}" }
            };

            try
            {
                Order order = client.Order.Create(options);
                string razorpayOrderId = order["id"].ToString();

                var response = new CheckoutResponseDTO
                {
                    RazorpayOrderId = razorpayOrderId,
                    Amount = totalAmount,
                    Currency = "INR"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error initializing Razorpay order.", error = ex.Message });
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] PaymentVerificationDTO model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            string keySecret = _configuration["Razorpay:KeySecret"]!;

            string payload = $"{model.RazorpayOrderId}|{model.RazorpayPaymentId}";
            string generatedSignature = ComputeHmacSha256(payload, keySecret);

            if (generatedSignature != model.RazorpaySignature)
            {
                return BadRequest(new { message = "Payment verification failed. Signature mismatch." });
            }

            // CRITICAL ARCHITECTURAL REQUIREMENT: SQL Atomic Transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
                    if (student == null) return BadRequest(new { message = "Student profile not found." });

                    var cartItems = await _context.Carts
                        .Where(c => c.StudentId == student.Id)
                        .Include(c => c.Course)
                        .ToListAsync();

                    if (!cartItems.Any())
                        return BadRequest(new { message = "No items found in cart to process." });

                    decimal totalAmount = cartItems.Sum(item => item.Course.Price);

                    foreach (var item in cartItems)
                    {
                        var payment = new UdemyBackend.Models.Payment
                        {
                            StudentId = student.Id,
                            CourseId = item.CourseId,
                            Amount = item.Course.Price,
                            GatewayTransactionId = model.RazorpayPaymentId,
                            Status = "Success",
                            ProcessedAt = DateTime.UtcNow
                        };
                        _context.Payments.Add(payment);

                        var enrollment = new Enrollment
                        {
                            StudentId = student.Id,
                            CourseId = item.CourseId,
                            EnrolledAt = DateTime.UtcNow,
                            ProgressPercentage = 0 
                        };
                        _context.Enrollments.Add(enrollment);
                    }

                    _context.Carts.RemoveRange(cartItems);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync(); // Saves everything permanently

                    return Ok(new { message = "Payment verified and enrollment successful!" });
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(); // Reverts database shifts if any error triggers
                    return StatusCode(500, new { message = "Critical error processing your order. Payment rolled back." });
                }
            }
        }

        private string ComputeHmacSha256(string data, string key)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }
    }
}