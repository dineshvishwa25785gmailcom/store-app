using store_app_apis.Helper;
using store_app_apis.Modal;
using store_app_apis.Repos.Models;

namespace store_app_apis.Service
{
    public interface ICustomerService
    {
        //List<TblCustomer> Getall();
        // List<Customermodal> Getall();  //-> This is Synchronuous Programming 
        Task<List<Customermodal>> Getall(); // -> This is Asynchronuous Programming
        Task<Customermodal> Getbycode(string code); // -> This is Asynchronuous Programming : Returning single value
        Task<APIResponse> Remove(string code); // -> This is Asynchronuous Programming
        Task<APIResponse> Create(Customermodal data); // -> POST Method to cteate a record
                                                      //

        Task<APIResponse> Update(Customermodal data,string code); // -> PUT Method to Update a record


    }
}
