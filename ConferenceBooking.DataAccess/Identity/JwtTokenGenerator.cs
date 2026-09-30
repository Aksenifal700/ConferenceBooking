using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ConferenceBooking.Application.DTOs.Auth;
using ConferenceBooking.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ConferenceBooking.DataAccess.Identity;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;
    private readonly TimeSpan _tokenLifetime;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
        _tokenLifetime = TimeSpan.FromMinutes(
            int.Parse(configuration["Jwt:ExpirationMinutes"]!));
    }

    public AuthTokenDto GenerateToken(TokenGenerationDto dto)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!);
        var expiresAt = DateTimeOffset.UtcNow.Add(_tokenLifetime);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, dto.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, dto.Email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt.UtcDateTime,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new AuthTokenDto
        {
            AccessToken = tokenHandler.WriteToken(token),
            ExpiresAt = expiresAt
        };
    }
}
