using API_EF_Http.DataAccess;
using AutoMapper;

namespace API_EF_Http.DTO
{
    public class MappingDTO: Profile
    {
        public MappingDTO()
        {
            CreateMap<Employee, EmployeeDTO>().ReverseMap(); ;
            CreateMap<Department, DepartmentDTO>().ReverseMap();
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<Customer, CustomerDTO>().ReverseMap();
            //    .ForMember(de => de.DepartmentName, opt => opt.MapFrom(src => src.DepartmentNew));
            CreateMap<Order, DTOCusTom>();
            //    .ForMember(o => o.OrderId, opt => opt.MapFrom(src => src.OrderId));
            //CreateMap<Order, DTOCusTom>()
            //    .ForMember(o => o.OrderDate, opt => opt.MapFrom(src => src.OrderDate));
            //CreateMap<Customer, DTOCusTom>()
            //    .ForMember(c => c., opt => opt.MapFrom(src => src.OrderDate));
        }
    }
}
