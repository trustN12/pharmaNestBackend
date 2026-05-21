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
    try
    {
        string key = _configuration["Razorpay:Key"];
        string secret = _configuration["Razorpay:Secret"];

        RazorpayClient client = new RazorpayClient(key, secret);

        int amountInPaise = (int)(req.amount * 100);

        Dictionary<string, object> options = new Dictionary<string, object>();

        options.Add("amount", amountInPaise);
        options.Add("currency", "INR");
        options.Add("payment_capture", 1);

        Order order = client.Order.Create(options);

        return Ok(new
        {
            id = order["id"].ToString(),
            amount = Convert.ToInt32(order["amount"]),
            currency = order["currency"].ToString()
        });
    }
    catch (Exception ex)
    {
        return BadRequest(ex.Message);
    }
}
        
        
        /* VERIFY PAYMENT API */
        [HttpPost]
[Route("VerifyPayment")]
public IActionResult VerifyPayment([FromBody] PaymentVerify req)
{
    try
    {
        if (
            req == null ||
            string.IsNullOrWhiteSpace(req.razorpay_order_id) ||
            string.IsNullOrWhiteSpace(req.razorpay_payment_id) ||
            string.IsNullOrWhiteSpace(req.razorpay_signature)
        )
        {
            return BadRequest(new
            {
                success = false,
                message = "Payment data missing"
            });
        }

        string secret = _configuration["Razorpay:Secret"];

        string payload =
            $"{req.razorpay_order_id}|{req.razorpay_payment_id}";

        string generatedSignature = "";

        using (
            var hmac = new HMACSHA256(
                Encoding.UTF8.GetBytes(secret)
            )
        )
        {
            byte[] hash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(payload)
            );

            generatedSignature = BitConverter
                .ToString(hash)
                .Replace("-", "")
                .ToLower();
        }

        Console.WriteLine("===============");
        Console.WriteLine("ORDER ID:");
        Console.WriteLine(req.razorpay_order_id);

        Console.WriteLine("PAYMENT ID:");
        Console.WriteLine(req.razorpay_payment_id);

        Console.WriteLine("RAZORPAY SIGNATURE:");
        Console.WriteLine(req.razorpay_signature);

        Console.WriteLine("GENERATED SIGNATURE:");
        Console.WriteLine(generatedSignature);
        Console.WriteLine("===============");

        bool isValid =
            generatedSignature.Trim().ToLower() ==
            req.razorpay_signature.Trim().ToLower();

        DAL dal = new DAL();

        using (
            SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("PharmaNestCS")
            )
        )
        {
            dal.UpdatePaymentStatus(
                req,
                isValid ? "Success" : "Failed",
                con
            );
        }

        if (!isValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid signature",
                generatedSignature = generatedSignature,
                razorpaySignature = req.razorpay_signature
            });
        }

        return Ok(new
        {
            success = true,
            message = "Payment verified successfully"
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new
        {
            success = false,
            message = ex.Message
        });
    }
}
        


        /* CART CLEAR API */
        
        [HttpPost]
        [Route("ClearCart")]
        public Response ClearCart([FromBody] Users users)
        {
            SqlConnection connection =
                new SqlConnection(
                    _configuration.GetConnectionString("PharmaNestCS")
                );

            DAL dal = new DAL();

            Response response = dal.ClearCart(users, connection);

            return response;
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
