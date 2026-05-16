using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PharmaNestBackend.Models;

namespace PharmaNestBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;


        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /* REGISTRATION API */
        [HttpPost]
        [Route("registration")]
        public Response register(Users users)
        {
            Response response = new Response();
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("PharmaNestCS").ToString());
            response = dal.Register(users, connection);
            return response;
        }
        
        
        /* LOGIN API */
        [HttpPost]
        [Route("login")]

        public Response login(Users users)
        {
            
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("PharmaNestCS").ToString());
            Response response = dal.Login(users, connection);
            return response;
        }
    }
}
