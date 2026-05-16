using System.Data;
using Microsoft.Data.SqlClient;

namespace PharmaNestBackend.Models;

public class DAL
{
    /* FOR REGISTRATION */
    public Response Register(Users users, SqlConnection connection)
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
    
    
    
    
    
    /* FOR LOGIN */

    public Response Login(Users users, SqlConnection connection)
    {
        SqlDataAdapter da = new SqlDataAdapter("sp_login", connection);
        
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.AddWithValue("@Email", users.Email);
        da.SelectCommand.Parameters.AddWithValue("@Password", users.Password);

        DataTable dt = new DataTable();
        da.Fill(dt);

        Response response = new Response();

        Users user = new Users();
        
        if (dt.Rows.Count > 0)
        {
            user.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
            user.FirstName = Convert.ToString(dt.Rows[0]["FirstName"]);
            user.LastName = Convert.ToString(dt.Rows[0]["LastName"]);
            user.Email = Convert.ToString(dt.Rows[0]["Email"]);
            user.Type = Convert.ToString(dt.Rows[0]["Type"]);
            response.StatusCode = 200;
            response.StatusMessage = "Login Successful";
            response.user = user;
        }
        else
        {
            response.StatusCode = 100;
            response.StatusMessage = "Invalid credentials";
            response.user = null;
        }

        return response;

    }
    
    
    
    
    
    /* FOR VIEWING USER */

    public Response ViewUser(Users users, SqlConnection connection)
    {
        SqlDataAdapter da = new SqlDataAdapter("sp_viewUser", connection);

        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.AddWithValue("@ID", users.ID);

        DataTable dt = new DataTable();
        da.Fill(dt);
        
        Response response = new Response();
        
        Users user = new Users();
        
        if (dt.Rows.Count > 0)
        {
            user.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
            user.FirstName = Convert.ToString(dt.Rows[0]["FirstName"]);
            user.LastName = Convert.ToString(dt.Rows[0]["LastName"]);
            user.Email = Convert.ToString(dt.Rows[0]["Email"]);
            user.Type = Convert.ToString(dt.Rows[0]["Type"]);
            user.Fund = Convert.ToDecimal(dt.Rows[0]["Fund"]);
            user.CreatedOn = Convert.ToDateTime(dt.Rows[0]["CreatedOn"]);
            user.Password = Convert.ToString(dt.Rows[0]["Password"]);
            
            response.StatusCode = 200;
            response.StatusMessage = "User exists";
            response.user = user;
            
        }
        else
        {
            response.StatusCode = 100;
            response.StatusMessage = "User does not exist";
            response.user = null;
        }

        return response;
    }
}