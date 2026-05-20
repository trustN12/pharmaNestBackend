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
        
        
        
        /* VIEWING USER ORDER LIST API */
        [HttpPost]
        [Route("OrderList")]

        public Response OrderList(Users users)
        {
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("PharmaNestCS").ToString());
            Response response = dal.OrderList(users, connection);

            return response;
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
    }
}
