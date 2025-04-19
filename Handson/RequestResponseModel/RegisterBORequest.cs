using Handson.Models;

namespace Handson.RequestResponseModel;

public class RegisterBORequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    
    public List<string> Roles { get; set; } = new List<string>();

    public static User Create(RegisterBORequest request)
    {
        User user = new User();
        
        user.Username = request.Username;
        user.Email = request.Email;
        user.Roles = request.Roles;
        
        return user;
    }
}