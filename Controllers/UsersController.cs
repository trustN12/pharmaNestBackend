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


        [HttpPost]
        [Route("registration")]
        public Response register(Users users)
        {
            Response response = new Response();
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("PharmaNestCS").ToString());
            response = dal.register(users, connection);
            return response;
        }
    }
}
