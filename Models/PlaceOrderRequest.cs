namespace PharmaNestBackend.Models;

public class PlaceOrderRequest
{
    public int userId { get; set; }

    public string receiverName { get; set; }
    public string phone { get; set; }

    public string addressLine { get; set; }
    public string district { get; set; }
    public string state { get; set; }
    public string pincode { get; set; }

    public string razorpayPaymentId { get; set; }
    public string razorpayOrderId { get; set; }
}