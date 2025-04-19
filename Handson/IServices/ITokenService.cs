using Handson.Models;

namespace Handson.IServices;

public interface ITokenService
{
    string GenerateToken(User user);
}