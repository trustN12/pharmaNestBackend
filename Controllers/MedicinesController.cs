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
    public class MedicinesController : ControllerBase
    {
        private readonly IConfiguration _configuration;


        public MedicinesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        
        /* ADD TO CART API */
        [HttpPost]
        [Route("AddToCart")]

        public Response AddToCart(Cart cart)
        {
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("PharmaNestCS").ToString());
            Response response = dal.AddToCart(cart, connection);

            return response;

        }
        
        
        
        /* ORDER PLACEMENT FROM THE CART API */
        [HttpPost]
        [Route("PlaceOrder")]
        public IActionResult PlaceOrder([FromBody] PlaceOrderRequest req)
        {
            if (req == null)
                return BadRequest("Invalid order request");

            DAL dal = new DAL();

            using (SqlConnection connection =
                   new SqlConnection(_configuration.GetConnectionString("PharmaNestCS")))
            {
                /*connection.Open();*/

                var response = dal.PlaceOrder(req, connection);

                return Ok(response);
            }
        }
        
        
        
        /* VIEWING USER ORDER LIST API */
        [HttpPost]
        [Route("OrderList")]
        public IActionResult OrderList(Users users)
        {
            try
            {
                DAL dal = new DAL();

                SqlConnection connection = new SqlConnection(
                    _configuration.GetConnectionString("PharmaNestCS").ToString()
                );

                Response response = dal.OrderList(users, connection);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        
        /* MEDICINES LIST API */
        [HttpGet]
        [Route("MedicineList")]

        public Response MedicineList()
        {
            DAL dal = new DAL();

            SqlConnection connection = new SqlConnection(
                _configuration.GetConnectionString("PharmaNestCS").ToString()
            );

            Response response = dal.MedicineList(connection);

            return response;
        }
        
        
        
        /* GET CART API */
        [HttpPost]
        [Route("GetCart")]
        public Response GetCart(CartRequest request)
        {
            DAL dal = new DAL();

            SqlConnection connection = new SqlConnection(
                _configuration.GetConnectionString("PharmaNestCS").ToString()
            );

            return dal.GetCart(request.UserId, connection);
        }
        
        
        /* DELETE CART API */
        [HttpPost]
        [Route("DeleteCartItem")]
        public Response DeleteCartItem([FromBody] Cart cart)
        {
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(
                _configuration.GetConnectionString("PharmaNestCS")
            );

            return dal.DeleteCartItem(cart.ID, connection);
        }
        
        
        /* UPDATE QUANTITY API */
        [HttpPost]
        [Route("UpdateCartQuantity")]
        public Response UpdateCartQuantity([FromBody] Cart cart)
        {
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(
                _configuration.GetConnectionString("PharmaNestCS")
            );

            return dal.UpdateCartQuantity(cart, connection);
        }
        
        
        
        /* ADD UPDATE MEDICINE API */
        [HttpPost]
        [Route("AddUpdateMedicine")]
        public Response AddUpdateMedicine(Medicines medicine)
        {
            DAL dal = new DAL();

            SqlConnection connection = new SqlConnection(
                _configuration.GetConnectionString("PharmaNestCS").ToString()
            );

            Response response = dal.AddUpdateMedicine(
                medicine,
                connection
            );

            return response;
        }
        
        
        
        
        /* Razorpay Order API */
        [HttpPost]
        [Route("CreateOrder")]
        public IActionResult CreateOrder([FromBody] OrderRequest req)
        {
            string key = _configuration["Razorpay:Key"];
            string secret = _configuration["Razorpay:Secret"];

            RazorpayClient client = new RazorpayClient(key, secret);

            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", req.amount * 100);
            options.Add("currency", "INR");
            options.Add("payment_capture", 1);

            Order order = client.Order.Create(options);

            // SAVE TO DB
            DAL dal = new DAL();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("PharmaNestCS")))
            {
                dal.CreatePaymentOrder(req.userId, req.amount, order["id"].ToString(), con);
            }

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
    }
}
