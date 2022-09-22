using API_EF_Http.DataAccess;
using AutoMapper;

namespace API_EF_Http.DTO
{
    public class MappingDTO: Profile
    {
        public MappingDTO()
        {
            CreateMap<Employee, EmployeeDTO>().ReverseMap(); ;
            CreateMap<Department, DepartmentDTO>();
            //    .ForMember(de => de.DepartmentName, opt => opt.MapFrom(src => src.DepartmentNew)); 
        }
    }
}
