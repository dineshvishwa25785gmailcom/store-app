using store_app_apis.Helper;
using store_app_apis.Modal;

namespace store_app_apis.Service
{
    public interface IUserService

    {
        Task<APIResponse> UserRegistration(UserRegister userRegister);
        Task<APIResponse> ConfirmRegister(int userid, string username, string otptext);

        Task<APIResponse> ResetPassword(string username, string oldpassword,string newpassword);
        Task<APIResponse> ForgotPassword(string username);
        Task<APIResponse> UpdatePassword(string username, string  password, string otptext);
        Task<APIResponse> UpdateStatus(string username, bool userstatus);
        Task<APIResponse> UpdateRole(string username, string userrole);
        Task<List<UserModel>> Getall();
        Task<UserModel> Getbycode(string code);
    }
}
