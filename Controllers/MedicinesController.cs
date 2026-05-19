using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using PharmaNestBackend.Models;

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

        public Response PlaceOrder(Users users)
        {
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("PharmaNestCS").ToString());
            Response response = dal.PlaceOrder(users, connection);

            return response;
        }
    }
}
