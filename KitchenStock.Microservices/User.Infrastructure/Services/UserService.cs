using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using User.Application.Dtos;
using User.Application.Services.Abstractions;
using User.Domain.Entities;

namespace User.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> CanCreateKitchenAsync(Guid userId, int currentKitchenCount)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null && currentKitchenCount < user.MaxKitchens;
    }

    public async Task<UserResponseDto?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null) return null;

        return new UserResponseDto(
            user.Id,
            user.Name,
            user.Email,
            user.Plan,
            user.MaxKitchens,
            user.CreatedAt
        );
    }

    public async Task<UserResponseDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null) return null;

        return new UserResponseDto(
            user.Id,
            user.Name,
            user.Email,
            user.Plan,
            user.MaxKitchens,
            user.CreatedAt
        );
    }

    public async Task<string?> LoginAsync(LoginUserDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        throw new NotImplementedException();

        //return GenerateJwtToken(user);
    }

    public async Task<UserResponseDto?> RegisterAsync(RegisterUserDto dto)
    {
        if (await _userRepository.ExistsAsync(dto.Email))
            return null; // Email já existe

        var user = new UserEntity
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        var createdUser = await _userRepository.CreateAsync(user);

        return new UserResponseDto(
            createdUser.Id,
            createdUser.Name,
            createdUser.Email,
            createdUser.Plan,
            createdUser.MaxKitchens,
            createdUser.CreatedAt
        );
    }

    //private string GenerateJwtToken(UserEntity user)
    //{
    //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
    //    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    var claims = new[]
    //    {
    //        new Claim("userId", user.Id.ToString()),
    //        new Claim("email", user.Email),
    //        new Claim("plan", user.Plan.ToString()),
    //        new Claim("maxKitchens", user.MaxKitchens.ToString())
    //    };

    //    var token = new JwtSecurityToken(
    //        issuer: _configuration["Jwt:Issuer"],
    //        audience: _configuration["Jwt:Audience"],
    //        claims: claims,
    //        expires: DateTime.UtcNow.AddHours(24),
    //        signingCredentials: credentials
    //    );

    //    return new JwtSecurityTokenHandler().WriteToken(token);
    //}
}
