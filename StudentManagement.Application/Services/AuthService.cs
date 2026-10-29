using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudentManagement.Application.DTOs.Auth;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthService(
        IAdminRepository adminRepository,
        IPasswordHasher passwordHasher,
        IConfiguration configuration)
    {
        _adminRepository = adminRepository;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task RegisterAsync(
        RegisterRequest request)
    {
        var existingAdmin =
            await _adminRepository.GetByEmailAsync(
                request.Email);

        if (existingAdmin != null)
        {
            throw new InvalidOperationException(
                "Admin with this email already exists.");
        }

        var admin = new Admin
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(
                request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _adminRepository.AddAsync(admin);
        await _adminRepository.SaveChangesAsync();
    }

    public async Task<LoginResponse?> LoginAsync(
        LoginRequest request)
    {
        var admin =
            await _adminRepository.GetByEmailAsync(
                request.Email);

        if (admin == null)
        {
            return null;
        }

        var passwordValid =
            _passwordHasher.Verify(
                request.Password,
                admin.PasswordHash);

        if (!passwordValid)
        {
            return null;
        }

        return GenerateToken(admin);
    }

    private LoginResponse GenerateToken(Admin admin)
    {
        var jwtSection =
            _configuration.GetSection("Jwt");

        var key = jwtSection["Key"]!;

        var issuer = jwtSection["Issuer"]!;

        var audience = jwtSection["Audience"]!;

        var expirationMinutes =
            int.Parse(
                jwtSection["ExpirationMinutes"]!);

        var expiresAt =
            DateTime.UtcNow.AddMinutes(
                expirationMinutes);

        var claims = new List<Claim>
        {
            new(
                ClaimTypes.NameIdentifier,
                admin.Id.ToString()),

            new(
                ClaimTypes.Name,
                admin.Name),

            new(
                ClaimTypes.Email,
                admin.Email),

            new(
                ClaimTypes.Role,
                "Admin")
        };

        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler()
                .WriteToken(token),

            ExpiresAt = expiresAt
        };
    }
}