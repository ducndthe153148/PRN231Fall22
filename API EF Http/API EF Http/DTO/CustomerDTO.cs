using API_EF_Http.DataAccess;

namespace API_EF_Http.DTO
{
    public class CustomerDTO
    {
        public CustomerDTO()
        {
            Orders = new HashSet<OrderDTO>();
        }

        public string CustomerId { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Address { get; set; }

        public virtual ICollection<OrderDTO> Orders { get; set; }
    }
}
