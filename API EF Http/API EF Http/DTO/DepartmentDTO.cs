namespace API_EF_Http.DTO
{
    public class DepartmentDTO
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentType { get; set; }
        public virtual ICollection<EmployeeDTO> Employees { get; set; }
    }
}
