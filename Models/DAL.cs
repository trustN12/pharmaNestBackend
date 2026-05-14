using System.Data;
using Microsoft.Data.SqlClient;

namespace PharmaNestBackend.Models;

public class DAL
{
    public Response register(Users users, SqlConnection connection)
    {
        Response response = new Response();
        SqlCommand cmd = new SqlCommand("sp_register", connection);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@FirstName", users.FirstName);
        cmd.Parameters.AddWithValue("@LastName", users.LastName);
        cmd.Parameters.AddWithValue("@Password", users.Password);
        cmd.Parameters.AddWithValue("@Email", users.Email);
        cmd.Parameters.AddWithValue("@Fund", 0);
        cmd.Parameters.AddWithValue("@Type", "Users");
        cmd.Parameters.AddWithValue("@Status", "Pending");
        
        connection.Open();

        int i = cmd.ExecuteNonQuery();
        
        connection.Close();

        if (i > 0)
        {
            response.StatusCode = 200;
            response.StatusMessage = "User registered successfully";
        }
        else
        {
            response.StatusCode = 100;
            response.StatusMessage = "User registration failed!!";
        }
        
        return response;
    }
}