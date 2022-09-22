using API_EF_Http.DataAccess;

namespace API_EF_Http.DTO
{
    public class EmployeeDTO
    {
        public EmployeeDTO()
        {
            Orders = new HashSet<Order>();
        }

        public int EmployeeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int? DepartmentId { get; set; }
        public string Title { get; set; }
        public string TitleOfCourtesy { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public string Address { get; set; }
        public virtual DepartmentDTO Department { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
