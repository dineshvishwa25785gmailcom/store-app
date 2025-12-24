using store_app_apis.Helper;
using store_app_apis.Modal;
using store_app_apis.Repos.Models;

namespace store_app_apis.Service
{
    public interface IUserRoleService
    {
        Task<APIResponse> AssignRolePermission(List<TblRolepermission> _data);
        Task<List<TblRole>> GetAllRoles();
        Task<List<TblMenu>> GetAllMenus();
        Task<List<AppMenus>> GetAllMenusByRole(string userrole);
        Task<MenuPermission> GetAllMenusPermissionByRole(string userrole, string menucode);

    }
}
