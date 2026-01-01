
using Microsoft.EntityFrameworkCore;
using store_app_apis.Helper;
using store_app_apis.Modal;
using store_app_apis.Service;
public interface IMasterContainer{
      
    ///
    Task<List<CategoryDTO>> CatGetall();
    Task<CategoryDTO> CatGetbycode(string UKID);
    Task<List<CategoryDTO>> CatGetbycategory(string CategoryName);
    Task<APIResponse> SaveCategory(CategoryDTO category);
    Task<APIResponse> Remove(string UKID); // -> This is Asynchronuous Programming

    Task<List<MeasurementDto>> MeasurmentGetall();
}