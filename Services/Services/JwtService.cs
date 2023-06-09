using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Core.Entities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Core.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    private readonly byte[] _secretKeyBytes;

    public JwtService(IConfiguration config)
    {
        _config = config;
        _secretKeyBytes = Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]);
    }

    public string GenerateAccessToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(_secretKeyBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expirationTimeStamp = DateTime.Now.AddMinutes(30);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(ClaimTypes.Role, Enum.GetName(user.Role)!)
        };

        var token = new JwtSecurityToken(
            _config["JwtSettings:Issuer"],
            _config["JwtSettings:Audience"],
            claims,
            expires: expirationTimeStamp,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }

    public RefreshToken GenerateRefreshToken(User user)
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);

            return new RefreshToken
            {
                UserId = user.Id,
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(10),
                CreatedAt = DateTime.UtcNow
            };
        }
    }

    public bool Verify(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config["JwtSettings:Issuer"],
            ValidAudience = _config["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(_secretKeyBytes),
            ClockSkew = TimeSpan.FromSeconds(60)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken validatedToken;
        
        try
        {
            tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
        }
        catch (SecurityTokenException)
        {
            return false;
        }

        return validatedToken != null;
    }
}