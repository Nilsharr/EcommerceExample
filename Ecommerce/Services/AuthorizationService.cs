using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Ecommerce.Interfaces;
using Ecommerce.Settings;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuthorizationService(IOptions<JwtSettings> jwtSettings, IDateTimeProvider dateTimeProvider)
    {
        _jwtSettings = jwtSettings.Value;
        _dateTimeProvider = dateTimeProvider;
    }

    public Task<(string authToken, DateTime tokenExpireDate)> GenerateAuthToken(string id)
    {
        var symmetricSecurityKey =
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret));

        var dateTimeNow = _dateTimeProvider.Now;
        var expireDate = dateTimeNow.Add(TimeSpan.FromDays(3));

        var jwt = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: new List<Claim> {new(ClaimTypes.NameIdentifier, id)},
            notBefore: dateTimeNow,
            expires: expireDate,
            signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
        );

        return Task.FromResult((new JwtSecurityTokenHandler().WriteToken(jwt), expireDate));
    }

    public (string hashedPassword, string salt) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(128 / 8);
        return (GenerateHashedPassword(password, salt), Convert.ToHexString(salt));
    }

    public bool VerifyHashedPassword(string providedPassword, string hashedPassword, string salt)
    {
        return hashedPassword == GenerateHashedPassword(providedPassword, HexStringToBytes(salt));
    }

    private static string GenerateHashedPassword(string password, byte[] salt)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(password: password, salt: salt,
            prf: KeyDerivationPrf.HMACSHA256, iterationCount: 100000, numBytesRequested: 256 / 8));
    }

    private static byte[] HexStringToBytes(string hexString)
    {
        if (hexString is null)
        {
            throw new ArgumentNullException(nameof(hexString), "Argument cannot be null");
        }

        var bytes = new byte[hexString.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            var currentHex = hexString.Substring(i * 2, 2);
            bytes[i] = Convert.ToByte(currentHex, 16);
        }

        return bytes;
    }
}