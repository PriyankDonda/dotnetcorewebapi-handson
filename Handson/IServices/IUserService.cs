using Handson.RequestResponseModel;

namespace Handson.IServices;

public interface IUserService
{
    Task<UserBOResponse?> GetUserByIdAsync(int id);
    Task<IEnumerable<UserBOResponse>> GetAllUsersAsync();
    Task<bool> RegisterUserAsync(RegisterBORequest request);
    Task<string?> AuthenticateAsync(LoginBORequest request);
}