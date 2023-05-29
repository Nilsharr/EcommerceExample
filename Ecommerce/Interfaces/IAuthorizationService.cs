using Ecommerce.Dtos;

namespace Ecommerce.Interfaces;

public interface IAuthorizationService
{
    Task<(string authToken, DateTime tokenExpireDate)> GenerateAuthToken(string id);
    (string hashedPassword, string salt) HashPassword(string password);
    bool VerifyHashedPassword(string providedPassword, string hashedPassword, string salt);
}