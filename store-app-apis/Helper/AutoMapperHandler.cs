using AutoMapper;
using store_app_apis.Modal;
using store_app_apis.Models;
using store_app_apis.Repos.Models;
using System.Diagnostics.Metrics;
namespace store_app_apis.Helper
{
    public class AutoMapperHandler : Profile
    {
        public AutoMapperHandler()
        {

            CreateMap<TblProduct, ProductDTO>().ReverseMap();

            CreateMap<TblUser, UserModel>().ForMember(item => item.Statusname, opt => opt.MapFrom(
                item => (item.Isactive != null && item.Isactive.Value) ? "Active" : "In active")).ReverseMap();
            CreateMap<UserModel, TblUser>();
            CreateMap<TblUser, UserModel>().ReverseMap();

            CreateMap<TblInvoiceHeader, Invoice_Header_DTO>().ReverseMap();
             
            ////   25-10-2025
            CreateMap<InvoiceCreateDTO, TblInvoiceHeader>();
            CreateMap<InvoiceItemCreateDTO, TblSalesProductinfo>();
            ////   25-10-2025

            CreateMap<TblSalesProductinfo, SalesProductDTO>().ReverseMap();

            CreateMap<TblCategory, CategoryDTO>().ReverseMap();
            CreateMap<TblMeasurement, MeasurementDto>().ReverseMap();

            CreateMap<CustomerDTO, TblCustomer>().ReverseMap();
        }
    }
}
