using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace UdemyApi.Services
{
    // Interface declaration defining the contract for token generation
    public interface IJwtService
    {
        string GenerateToken(string userId, string email, int role, string? instructorId = null);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string userId, string email, int role, string? instructorId = null)
        {
            // Pull settings out of your appsettings.json configuration file
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "FallbackSecretKeyIfAppsettingsIsMissing2026!";
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Establish the mandatory base identity claims payload
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role.ToString()) // Accommodates your (0, 1, 2) role management structure
            };

            // Dynamically inject the InstructorId claim if it is provided
            if (!string.IsNullOrEmpty(instructorId))
            {
                claims.Add(new Claim("InstructorId", instructorId));
            }

            var expiryMinutes = double.TryParse(jwtSettings["ExpiryInMinutes"], out var minutes) ? minutes : 60;

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"] ?? "UdemyCloneBackend",
                audience: jwtSettings["Audience"] ?? "UdemyCloneFrontend",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}