using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using PharmaNestBackend.Models;

namespace PharmaNestBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;


        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        
        /* ADD UPDATE MEDICINES API */
        [HttpPost]
        [Route("AddUpdateMedicine")]
        public Response AddUpdateMedicine(Medicines medicine)
        {
            SqlConnection connection = new SqlConnection(
                _configuration.GetConnectionString("PharmaNestCS")
            );

            DAL dal = new DAL();

            Response response = dal.AddUpdateMedicine(medicine, connection);

            return response;
        }
        
        
        /* GET ALL USER LIST */
        [HttpGet]
        [Route("GetUsers")]

        public Response GetUsers()
        {
            DAL dal = new DAL();

            SqlConnection connection = new SqlConnection(
                _configuration.GetConnectionString("PharmaNestCS").ToString()
            );

            Response response = dal.GetUsers(connection);

            return response;
        }
        
        
        
        
        /* APPROVAL OF USERS */
        
        [HttpPut]
        [Route("ApproveUser/{id}")]

        public Response ApproveUser(int id)
        {
            DAL dal = new DAL();

            SqlConnection connection = new SqlConnection(
                _configuration.GetConnectionString("PharmaNestCS").ToString()
            );

            Response response = dal.ApproveUser(
                id,
                connection
            );

            return response;
        }
        
        
        
        /* ADMIN DETAILS API */
        [HttpGet]
        [Route("GetAdminProfile")]
        public Response GetAdminProfile()
        {
            SqlConnection connection = new SqlConnection(
                _configuration.GetConnectionString("PharmaNestCS")
            );

            DAL dal = new DAL();

            Response response = dal.GetAdminProfile(connection);

            return response;
        }
    }
}
