using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using PharmaNestBackend.Models;
using Razorpay.Api;
namespace PharmaNestBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;


        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        
        /* Razorpay Order API */
        [HttpPost]
        [Route("CreateOrder")]
        public IActionResult CreateOrder([FromBody] OrderRequest req)
        {
            if (req == null || req.amount <= 0)
                return BadRequest("Invalid amount received");

            /*Console.WriteLine("AMOUNT RECEIVED: " + req.amount);*/

            string key = _configuration["Razorpay:Key"];
            string secret = _configuration["Razorpay:Secret"];

            RazorpayClient client = new RazorpayClient(key, secret);

            int amountInPaise = (int)Math.Round(req.amount * 100m);

            var options = new Dictionary<string, object>
            {
                { "amount", amountInPaise },
                { "currency", "INR" },
                { "payment_capture", 1 }
            };

            Order order = client.Order.Create(options);

            return Ok(order);
        }
        /* VERIFY PAYMENT API */
        
        [HttpPost]
        [Route("VerifyPayment")]
        public IActionResult VerifyPayment([FromBody] PaymentVerify req)
        {
            string secret = _configuration["Razorpay:Secret"];

            string payload = req.razorpay_order_id + "|" + req.razorpay_payment_id;

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                var generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

                bool isValid = generatedSignature == req.razorpay_signature;

                DAL dal = new DAL();

                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("PharmaNestCS")))
                {
                    dal.UpdatePaymentStatus(
                        req,
                        isValid ? "Success" : "Failed",
                        con
                    );
                }

                if (!isValid)
                    return BadRequest("Invalid signature");

                return Ok("Payment Verified");
            }
        }
        
        
        
        
        
        [HttpGet]
        [Route("GetPincodeDetails/{pin}")]
        public async Task<IActionResult> GetPincodeDetails(string pin)
        {
            try 
            {
                using var client = new HttpClient();
        
                // Public APIs block requests without User-Agent headers to prevent bot spam
                client.DefaultRequestHeaders.Add("User-Agent", "PharmaNest-App");

                var url = $"https://api.postalpincode.in/pincode/{pin}";
                var response = await client.GetStringAsync(url);

                // Return as raw JSON content so ASP.NET doesn't double-serialize the string
                return Content(response, "application/json");
            }
            catch (Exception ex)
            {
                // Prevent server crashes by returning a proper error status
                return StatusCode(500, new { error = ex.Message });
            }
        }

        
        
    }
}
