using MyApiApp.Models;

namespace MyApiApp.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
        string GetUserIdFromToken(string token);
    }
}
