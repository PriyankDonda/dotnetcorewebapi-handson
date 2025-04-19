using Handson.Models;

namespace Handson.RequestResponseModel;

public class UserBOResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; }

    public static UserBOResponse Create(User user)
    {
        if (user == null)
            return null;

        UserBOResponse response = new UserBOResponse();
        response.Id = user.Id;
        response.Email = user.Email;
        response.Username = user.Username;
        response.CreatedAt = user.CreatedAt;
        response.Roles = user.Roles;
        
        return response;
    }
}