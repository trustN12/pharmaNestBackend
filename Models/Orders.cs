namespace PharmaNestBackend.Models;

public class Orders
{
    public int ID { get; set; }

    public int UserID { get; set; }

    public string? OrderNo { get; set; }

    public decimal OrderTotal { get; set; }

    public string? OrderStatus { get; set; }

    public string? ReceiverName { get; set; }

    public string? Phone { get; set; }

    public string? AddressLine { get; set; }

    public string? District { get; set; }

    public string? State { get; set; }

    public string? Pincode { get; set; }

    public string? RazorpayPaymentId { get; set; }

    public string? RazorpayOrderId { get; set; }
}