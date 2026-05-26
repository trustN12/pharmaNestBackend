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
    
    
    
    /* UPDATE PROFILE OF USER */

    public Response UpdateProfile(Users users, SqlConnection connection)
    {
        Response response = new Response();

        SqlCommand cmd = new SqlCommand("sp_updateProfile", connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@ID", users.ID);
        cmd.Parameters.AddWithValue("@FirstName", users.FirstName);
        cmd.Parameters.AddWithValue("@LastName", users.LastName);
        cmd.Parameters.AddWithValue("@Email", users.Email);
        cmd.Parameters.AddWithValue("@Password", users.Password);
        
        connection.Open();
        int i = cmd.ExecuteNonQuery();
        connection.Close();

        if (i > 0)
        {
            response.StatusCode = 200;
            response.StatusMessage = "Record updated successfully";
        }
        else
        {
            response.StatusCode = 100;
            response.StatusMessage = "Some error occured. Try after sometime!!";
        }

        return response;

    }
    
    
    
    /* ADD TO CART */

    public Response AddToCart(Cart cart, SqlConnection connection)
    {
        Response response = new Response();

        // STEP 1: fetch medicine price from DB (IMPORTANT FIX)
        SqlCommand priceCmd = new SqlCommand(
            "SELECT UnitPrice, DiscountedPrice FROM Medicines WHERE Id = @MedicineID",
            connection
        );

        priceCmd.Parameters.AddWithValue("@MedicineID", cart.MedicineID);

        SqlDataAdapter da = new SqlDataAdapter(priceCmd);
        DataTable dt = new DataTable();
        da.Fill(dt);

        if (dt.Rows.Count == 0)
        {
            response.StatusCode = 100;
            response.StatusMessage = "Medicine not found";
            return response;
        }

        decimal unitPrice = Convert.ToDecimal(dt.Rows[0]["UnitPrice"]);
        decimal discountedPrice = Convert.ToDecimal(dt.Rows[0]["DiscountedPrice"]);

        decimal finalPrice = discountedPrice; // ALWAYS use discounted price

        decimal totalPrice = finalPrice * cart.Quantity;

        SqlCommand cmd = new SqlCommand("sp_addToCart", connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@UserId", cart.UserId);
        cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
        cmd.Parameters.AddWithValue("@Discount", unitPrice - discountedPrice);
        cmd.Parameters.AddWithValue("@Quantity", cart.Quantity);
        cmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
        cmd.Parameters.AddWithValue("@MedicineID", cart.MedicineID);

        connection.Open();
        int i = cmd.ExecuteNonQuery();
        connection.Close();

        if (i > 0)
        {
            response.StatusCode = 200;
            response.StatusMessage = "Item added to cart successfully";
        }
        else
        {
            response.StatusCode = 100;
            response.StatusMessage = "Some error occured. Try after sometime!!";
        }

        return response;
    }
    
    
    /* PLACE ORDER FROM CART */

    /*
    public Response PlaceOrder(Users users, SqlConnection connection)
    {
        Response response = new Response();
        SqlCommand cmd = new SqlCommand("sp_placeOrder", connection);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ID", users.ID);
        connection.Open();

        int i = cmd.ExecuteNonQuery();
        connection.Close();
        
        if (i > 0)
        {
            response.StatusCode = 200;
            response.StatusMessage = "Order has been placed successfully";
        }
        else
        {
            response.StatusCode = 100;
            response.StatusMessage = "Some error occured. Try after sometime!!";
        }
        
        return response;
    }
    */
    
    
    public Response PlaceOrder(PlaceOrderRequest req, SqlConnection connection)
    {
        Response response = new Response();

        SqlCommand cmd = new SqlCommand("sp_placeOrderWithAddress", connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@UserId", req.userId);
        cmd.Parameters.AddWithValue("@ReceiverName", req.receiverName);
        cmd.Parameters.AddWithValue("@Phone", req.phone);
        cmd.Parameters.AddWithValue("@AddressLine", req.addressLine);
        cmd.Parameters.AddWithValue("@District", req.district);
        cmd.Parameters.AddWithValue("@State", req.state);
        cmd.Parameters.AddWithValue("@Pincode", req.pincode);
        cmd.Parameters.AddWithValue("@RazorpayPaymentId", req.razorpayPaymentId);
        cmd.Parameters.AddWithValue("@RazorpayOrderId", req.razorpayOrderId);

        connection.Open();
        int i = cmd.ExecuteNonQuery();
        connection.Close();

        if (i > 0)
        {
            response.StatusCode = 200;
            response.StatusMessage = "Order placed successfully";
        }
        else
        {
            response.StatusCode = 100;
            response.StatusMessage = "Order failed";
        }

        return response;
    }
    
    
    
    
    /* VIEWING USER ORDER LIST */
public Response OrderList(Users users, SqlConnection connection)
{
    Response response = new Response();

    SqlCommand cmd = new SqlCommand("sp_orderList", connection);

    cmd.CommandType = CommandType.StoredProcedure;

    // VERY IMPORTANT
    cmd.Parameters.AddWithValue("@UserId", users.ID);

    // VERY IMPORTANT
    cmd.Parameters.AddWithValue("@Type", users.Type);

    SqlDataAdapter da = new SqlDataAdapter(cmd);

    DataTable dt = new DataTable();

    da.Fill(dt);

    List<Orders> orderList = new List<Orders>();

    if (dt.Rows.Count > 0)
    {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            Orders order = new Orders();

            order.ID =
                dt.Rows[i]["ID"] != DBNull.Value
                ? Convert.ToInt32(dt.Rows[i]["ID"])
                : 0;

            order.UserID =
                dt.Rows[i]["UserID"] != DBNull.Value
                ? Convert.ToInt32(dt.Rows[i]["UserID"])
                : 0;

            order.OrderNo =
                dt.Rows[i]["OrderNo"]?.ToString() ?? "";

            order.OrderTotal =
                dt.Rows[i]["OrderTotal"] != DBNull.Value
                ? Convert.ToDecimal(dt.Rows[i]["OrderTotal"])
                : 0;

            order.OrderStatus =
                dt.Rows[i]["OrderStatus"]?.ToString() ?? "";

            order.ReceiverName =
                dt.Rows[i]["ReceiverName"]?.ToString() ?? "";

            order.Phone =
                dt.Rows[i]["Phone"]?.ToString() ?? "";

            order.AddressLine =
                dt.Rows[i]["AddressLine"]?.ToString() ?? "";

            order.District =
                dt.Rows[i]["District"]?.ToString() ?? "";

            order.State =
                dt.Rows[i]["State"]?.ToString() ?? "";

            order.Pincode =
                dt.Rows[i]["Pincode"]?.ToString() ?? "";

            order.RazorpayPaymentId =
                dt.Rows[i]["RazorpayPaymentId"]?.ToString() ?? "";

            order.RazorpayOrderId =
                dt.Rows[i]["RazorpayOrderId"]?.ToString() ?? "";

            orderList.Add(order);
        }

        response.StatusCode = 200;

        response.StatusMessage = "Orders Found";

        response.listOrders = orderList;
    }
    else
    {
        response.StatusCode = 100;

        response.StatusMessage = "No Orders Found";
    }

    return response;
}
    
    /* ADD UPDATE MEDICINES */

    public Response AddUpdateMedicine(Medicines medicine, SqlConnection connection)
    {
        Response response = new Response();

        try
        {
            SqlCommand cmd = new SqlCommand("sp_addUpdateMedicine", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", medicine.Id);

            cmd.Parameters.AddWithValue("@MedicineName", medicine.MedicineName);

            cmd.Parameters.AddWithValue("@Manufacturer", medicine.Manufacturer);

            cmd.Parameters.AddWithValue("@Category", medicine.Category);

            cmd.Parameters.AddWithValue("@UnitPrice", medicine.UnitPrice);

            cmd.Parameters.AddWithValue("@DiscountedPrice", medicine.DiscountedPrice);

            cmd.Parameters.AddWithValue("@Stock", medicine.Stock);

            cmd.Parameters.AddWithValue("@ExpiryDate", medicine.ExpiryDate);

            cmd.Parameters.AddWithValue("@Description", medicine.Description);

            cmd.Parameters.AddWithValue("@ImageUrl", medicine.ImageUrl);

            connection.Open();

            int i = cmd.ExecuteNonQuery();

            connection.Close();

            if (i > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Medicine Saved Successfully";
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Failed";
            }
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.StatusMessage = ex.Message;
        }

        return response;
    }
    
    /* MEDICINES LIST */
    public Response MedicineList(SqlConnection connection)
    {
        Response response = new Response();

        List<Medicines> listMedicines = new List<Medicines>();

        SqlDataAdapter da = new SqlDataAdapter(
            "sp_getMedicines",
            connection
        );

        da.SelectCommand.CommandType =
            CommandType.StoredProcedure;

        DataTable dt = new DataTable();

        da.Fill(dt);

        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Medicines medicine = new Medicines();

                medicine.Id =
                    Convert.ToInt32(dt.Rows[i]["Id"]);

                medicine.MedicineName =
                    Convert.ToString(dt.Rows[i]["MedicineName"]);

                medicine.Manufacturer =
                    Convert.ToString(dt.Rows[i]["Manufacturer"]);

                medicine.Category =
                    Convert.ToString(dt.Rows[i]["Category"]);

                medicine.UnitPrice =
                    Convert.ToDecimal(dt.Rows[i]["UnitPrice"]);

                medicine.DiscountedPrice =
                    Convert.ToDecimal(dt.Rows[i]["DiscountedPrice"]);

                medicine.Stock =
                    Convert.ToInt32(dt.Rows[i]["Stock"]);

                medicine.ExpiryDate =
                    Convert.ToString(dt.Rows[i]["ExpiryDate"]);

                medicine.Description =
                    Convert.ToString(dt.Rows[i]["Description"]);

                medicine.ImageUrl =
                    Convert.ToString(dt.Rows[i]["ImageUrl"]);

                listMedicines.Add(medicine);
            }

            response.StatusCode = 200;
            response.StatusMessage = "Medicines fetched";
            response.listMedicines = listMedicines;
        }
        else
        {
            response.StatusCode = 100;
            response.StatusMessage = "No medicines found";
            response.listMedicines = null;
        }

        return response;
    }
    
    
    /* GET ALL USERS */

    public Response GetUsers(SqlConnection connection)
    {
        Response response = new Response();

        SqlDataAdapter da = new SqlDataAdapter(
            "sp_getUsers",
            connection
        );

        da.SelectCommand.CommandType =
            CommandType.StoredProcedure;

        DataTable dt = new DataTable();

        da.Fill(dt);

        List<Users> listUsers = new List<Users>();

        foreach (DataRow row in dt.Rows)
        {
            Users user = new Users();

            user.ID = Convert.ToInt32(row["ID"]);
            user.FirstName = Convert.ToString(row["FirstName"]);
            user.LastName = Convert.ToString(row["LastName"]);
            user.Email = Convert.ToString(row["Email"]);
            user.Password = Convert.ToString(row["Password"]);
            user.Type = Convert.ToString(row["Type"]);
            user.Status = Convert.ToString(row["Status"]);

            listUsers.Add(user);
        }

        if (listUsers.Count > 0)
        {
            response.StatusCode = 200;
            response.StatusMessage = "Users Found";
            response.listUsers = listUsers;
        }
        else
        {
            response.StatusCode = 100;
            response.StatusMessage = "No Users Found";
            response.listUsers = null;
        }

        return response;
    }
    
    
    
    /* APPROVE USER */

    public Response ApproveUser(
        int id,
        SqlConnection connection
    )
    {
        Response response = new Response();

        SqlCommand cmd = new SqlCommand(
            "sp_approveUser",
            connection
        );

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@ID", id);

        connection.Open();

        int i = cmd.ExecuteNonQuery();

        connection.Close();

        if (i > 0)
        {
            response.StatusCode = 200;
            response.StatusMessage = "User Approved";
        }
        else
        {
            response.StatusCode = 100;
            response.StatusMessage = "User Not Approved";
        }

        return response;
    }
    
    
    
    /* ADMIN DETAILS */
    public Response GetAdminProfile(SqlConnection connection)
    {
        Response response = new Response();

        SqlCommand cmd = new SqlCommand(
            "SELECT TOP 1 * FROM Users WHERE Type = 'Admin'",
            connection
        );

        SqlDataAdapter da = new SqlDataAdapter(cmd);

        DataTable dt = new DataTable();

        da.Fill(dt);

        if (dt.Rows.Count > 0)
        {
            response.StatusCode = 200;
            response.StatusMessage = "Admin Found";

            response.Admin = new Users()
            {
                ID = Convert.ToInt32(dt.Rows[0]["ID"]),
                FirstName = Convert.ToString(dt.Rows[0]["FirstName"]),
                LastName = Convert.ToString(dt.Rows[0]["LastName"]),
                Email = Convert.ToString(dt.Rows[0]["Email"]),
                Type = Convert.ToString(dt.Rows[0]["Type"]),
                Status = Convert.ToString(dt.Rows[0]["Status"])
            };
        }
        else
        {
            response.StatusCode = 100;
            response.StatusMessage = "No Admin Found";
        }

        return response;
    }
    
    
    /* GETTING CART DETAILS */
    public Response GetCart(int userId, SqlConnection connection)
    {
        Response response = new Response();
        List<Cart> list = new List<Cart>();

        try
        {
            SqlDataAdapter da = new SqlDataAdapter("sp_getCart", connection);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@UserId", userId);

            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                Cart cart = new Cart
                {
                    ID = Convert.ToInt32(row["ID"]),
                    UserId = Convert.ToInt32(row["UserId"]),
                    MedicineID = Convert.ToInt32(row["MedicineID"]),

                    MedicineName = row["MedicineName"]?.ToString(),
                    Manufacturer = row["Manufacturer"]?.ToString(),
                    Category = row["Category"]?.ToString(),
                    ImageUrl = row["ImageUrl"]?.ToString(),

                    UnitPrice = Convert.ToDecimal(row["UnitPrice"]),
                    Discount = Convert.ToDecimal(row["Discount"]),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    TotalPrice = Convert.ToDecimal(row["TotalPrice"])
                };

                list.Add(cart);
            }

            response.StatusCode = 200;
            response.StatusMessage = "Cart fetched successfully";
            response.listCart = list;
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.StatusMessage = ex.Message;
        }

        return response;
    }
    
    
    /* DELETE CART ITEMS */
    public Response DeleteCartItem(int id, SqlConnection connection)
    {
        Response response = new Response();

        SqlCommand cmd = new SqlCommand("sp_deleteCartItem", connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@ID", id);

        connection.Open();
        int i = cmd.ExecuteNonQuery();
        connection.Close();

        if (i > 0)
        {
            response.StatusCode = 200;
            response.StatusMessage = "Item removed from cart";
        }
        else
        {
            response.StatusCode = 100;
            response.StatusMessage = "Failed to remove item";
        }

        return response;
    }
    
    
    /* UPDATE CART QUANTITY */
    public Response UpdateCartQuantity(Cart cart, SqlConnection connection)
    {
        Response response = new Response();

        SqlCommand cmd = new SqlCommand("sp_updateCartQuantity", connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@ID", cart.ID);
        cmd.Parameters.AddWithValue("@Quantity", cart.Quantity);

        connection.Open();
        int i = cmd.ExecuteNonQuery();
        connection.Close();

        if (i > 0)
        {
            response.StatusCode = 200;
            response.StatusMessage = "Cart updated";
        }
        else
        {
            response.StatusCode = 100;
            response.StatusMessage = "Update failed";
        }

        return response;
    }
    
    
    
    /* CREATE PAYMENT ORDER */
    public int CreatePaymentOrder(int userId, decimal amount, string razorpayOrderId, SqlConnection connection)
    {
        int paymentId = 0;

        SqlCommand cmd = new SqlCommand("sp_createPaymentOrder", connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@Amount", amount);
        cmd.Parameters.AddWithValue("@RazorpayOrderId", razorpayOrderId);

        connection.Open();

        var result = cmd.ExecuteScalar();

        if (result != null)
            paymentId = Convert.ToInt32(result);

        connection.Close();

        return paymentId;
    }
    
    
    /* UPDATE PAYMENT AFTER VERIFICATION */
    public void UpdatePaymentStatus(PaymentVerify req, string status, SqlConnection connection)
    {
        SqlCommand cmd = new SqlCommand("sp_updatePaymentStatus", connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@RazorpayOrderId", req.razorpay_order_id);
        cmd.Parameters.AddWithValue("@RazorpayPaymentId", req.razorpay_payment_id);
        cmd.Parameters.AddWithValue("@RazorpaySignature", req.razorpay_signature);
        cmd.Parameters.AddWithValue("@Status", status);

        connection.Open();
        cmd.ExecuteNonQuery();
        connection.Close();
    }
    
    
    
    /* CLEAR CART */
    public Response ClearCart(Users users, SqlConnection connection)
    {
        Response response = new Response();

        SqlCommand cmd = new SqlCommand(
            "DELETE FROM Cart WHERE UserId = @UserId",
            connection
        );

        cmd.Parameters.AddWithValue("@UserId", users.ID);

        connection.Open();

        int rows = cmd.ExecuteNonQuery();

        connection.Close();

        if (rows > 0)
        {
            response.StatusCode = 200;
            response.StatusMessage = "Cart Cleared Successfully";
        }
        else
        {
            response.StatusCode = 100;
            response.StatusMessage = "Cart Already Empty";
        }

        return response;
    }
    
}