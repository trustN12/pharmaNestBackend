/*
namespace PharmaNestBackend.Models;

public class Medicines
{
    public int ID { get; set; }

    public string? Name { get; set; }

    public string? Manufacturer { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Discount  { get; set; }

    public int Quantity { get; set; }

    public DateTime  ExpDate { get; set; }

    public string? ImageUrl  { get; set; }

    public int Status { get; set; }

    public string? Type { get; set; }
}
*/



namespace PharmaNestBackend.Models
{
    public class Medicines
    {
        public int Id { get; set; }

        public string? MedicineName { get; set; }

        public string? Manufacturer { get; set; }

        public string? Category { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal DiscountedPrice { get; set; }

        public int Stock { get; set; }

        public string? ExpiryDate { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}

