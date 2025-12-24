using store_app_apis.Helper;
using store_app_apis.Modal;
using store_app_apis.Repos.Models;

namespace store_app_apis.Service
{
    public interface ICustomerService
    {
        Task<List<CustomerDTO>> Getall(); // -> This is Asynchronuous Programming
        Task<CustomerDTO> GetByUniqueKeyID(string UKID); // -> This is Asynchronuous Programming : Returning single value
        Task<APIResponse> Remove(string UKID); // -> This is Asynchronuous Programming
        Task<APIResponse> Create(CustomerDTO data); // -> POST Method to cteate a record
        Task<APIResponse> Update(CustomerDTO data,string UKID); // -> PUT Method to Update a record
    }
}
