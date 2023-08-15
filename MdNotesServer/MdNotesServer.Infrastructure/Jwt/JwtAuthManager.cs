using MdNotesServer.Core.Jwt;
using MdNotesServer.Infrastructure.Entities;
using MdNotesServer.Infrastructure.Jwt.JwtConfiguration;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MdNotesServer.Infrastructure.Jwt
{
    public class JwtAuthManager : IJwtAuthManager
    {
        private readonly JwtTokenOptions _jwtTokenConfig;
        private readonly byte[] _secret;
        private readonly UserManager<UserEntity> _usersManager;
        
        public JwtAuthManager(JwtTokenOptions jwtTokenConfig, UserManager<UserEntity> usersManager)
        {
            _jwtTokenConfig = jwtTokenConfig;
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
            _usersManager = usersManager;
            
        }

        public async Task<bool> RemoveExpiredTokens(UserEntity user) =>
           (await _usersManager.RemoveAuthenticationTokenAsync(user, "Default", "AccessToken")).Succeeded;
        

        public async Task<JwtAuthResult> GenerateTokens(UserEntity user, Claim[] claims, DateTime now)
        {
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
            var jwtToken = new JwtSecurityToken(
                _jwtTokenConfig.Issuer,
                shouldAddAudienceClaim ? _jwtTokenConfig.Audience : string.Empty,
                claims,
                expires: now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            await _usersManager.SetAuthenticationTokenAsync(user, "Default", "AccessToken", jwtToken.ToString());

            return new JwtAuthResult
            {
                UserId = user.Id,
                AccessToken = accessToken,
            };
        }

        public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = _jwtTokenConfig.Issuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(_secret),
                        ValidAudience = _jwtTokenConfig.Audience,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    },
                    out var validatedToken);

            return (principal, validatedToken as JwtSecurityToken);
        }

        private static string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
