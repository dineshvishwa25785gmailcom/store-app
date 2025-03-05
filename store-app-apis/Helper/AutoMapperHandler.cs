using AutoMapper;
using store_app_apis.Modal;
using store_app_apis.Repos.Models;
namespace store_app_apis.Helper
{
    public class AutoMapperHandler : Profile
    {
        //public AutoMapperHandler() { CreateMap<TblCustomer, Customermodal>().ForMember(item => item.Statusname, opt => opt.MapFrom(item => (item.IsActive&&item.IsActive.Value)?"Active" : "Inactive")); }

        // Before Reverse Mapping
        //public AutoMapperHandler()
        //{
        //    CreateMap<TblCustomer, Customermodal>()
        //        .ForMember(dest => dest.Statusname, opt =>
        //            opt.MapFrom(src => src.IsActive.HasValue && src.IsActive.Value ? "Active" : "Inactive"));
        //}

        // We are getting input in customer model and we are converting it into TblCustomer so in this case we have a small change in our mapper handler    class. So as per the concept of automapper we have to create a map for both the classes.We can incluude reverse manner  also.   
        
        
        // After Reverse Mapping
        public AutoMapperHandler()
        {
            CreateMap<TblCustomer, Customermodal>()
                .ForMember(dest => dest.Statusname, opt =>
                    opt.MapFrom(src => src.IsActive.HasValue && src.IsActive.Value ? "Active" : "Inactive")).ReverseMap();
        }
    }
}
