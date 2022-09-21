using API_EF_Http.DataAccess;
using AutoMapper;

namespace API_EF_Http.DTO
{
    public class MappingDTO: Profile
    {
        public MappingDTO()
        {
            CreateMap<Employee, EmployeeDTO>();
            CreateMap<Employee, EmployeeDTO>(); 
        }
    }
}
