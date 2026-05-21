namespace PharmaNestBackend.Models;

public class OrderRequest
{
    public decimal amount { get; set; }
    public int userId { get; set; }
}