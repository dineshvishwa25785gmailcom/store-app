
using Microsoft.EntityFrameworkCore;
using store_app_apis.Helper;
using store_app_apis.Repos.Models;
using store_app_apis.Service;
public interface IProductService
{
    Task<List<ProductDTO>> Getall();
    Task<ProductDTO> Getbycode(string code);
    Task<List<ProductDTO>> Getbyname(string Category);
    Task<APIResponse> SaveProduct(ProductDTO product);

    Task<APIResponse> RemoveProduct(string code); // -> This is Asynchronuous Programming

}