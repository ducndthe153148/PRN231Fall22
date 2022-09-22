namespace API_EF_Http.DTO
{
    public class DTOCusTom
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string CustomerName { get; set; } // Customer
        public string ProductName { get; set; } // Product
        public decimal Price { get; set; } // Orderdetail
        public short Quantity { get; set; } // Orderdetail

    }
}
