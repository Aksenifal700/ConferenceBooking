using ConferenceBooking.Application.DTOs.Auth;
using ConferenceBooking.Application.Exceptions;
using ConferenceBooking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace ConferenceBooking.DataAccess.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Guid> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim();

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);

        if (!createResult.Succeeded)
        {
            throw new BadRequestException(string.Join("; ", createResult.Errors.Select(error => error.Description)));
        }

        return user.Id;
    }

    public async Task<AuthTokenDto> LoginAsync(LoginDto dto)
    {
        const string invalidCredentialsMessage = "Invalid email or password.";

        var user = await _userManager.FindByEmailAsync(dto.Email.Trim());

        if (user is null)
        {
            throw new InvalidCredentialsException(invalidCredentialsMessage);
        }

        var passwordIsValid = await _userManager.CheckPasswordAsync(
            user,
            dto.Password);

        if (!passwordIsValid)
        {
            throw new InvalidCredentialsException(invalidCredentialsMessage);
        }

        return _jwtTokenGenerator.GenerateToken(new TokenGenerationDto
        {
            UserId = user.Id,
            Email = user.Email!
        });
    }
}