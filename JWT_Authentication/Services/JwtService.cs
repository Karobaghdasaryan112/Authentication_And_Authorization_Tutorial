using JWT_Authentication.Entities;
using JWT_Authentication.Interfaces.Services;
using JWT_Authentication.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_Authentication.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtOptions _jwtOptions;

        public JwtService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }
        public async Task<string> GenerateToken(User user)
        {
            if (user is null)
            {
                return await Task.FromResult(string.Empty).ConfigureAwait(false);
            }

            var SigninCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user?.UserName!),
                new Claim(ClaimTypes.Email, user?.Email!),
                new Claim(ClaimTypes.NameIdentifier, user?.Id.ToString()!)
            };

            foreach (var role in user?.Roles!)
            {
                claims.Add(new Claim(ClaimTypes.Role, role?.ToString()!));
            }

            var Token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                expires: DateTime.Now.AddHours(Convert.ToDouble(_jwtOptions.ExpiresHours)),
                signingCredentials: SigninCredentials,
                claims: claims
            );

            var TokenValue = new JwtSecurityTokenHandler().WriteToken(Token);

            return await Task.FromResult(TokenValue).ConfigureAwait(false);
        }

        public async Task<bool> VerifyToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var SecretKey = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);

            var ValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(SecretKey)
            };

            try
            {
                tokenHandler.ValidateToken(token, ValidationParameters, out SecurityToken validatedToken);
                return await Task.FromResult(true).ConfigureAwait(false);
            }
            catch
            {
                return await Task.FromResult(false).ConfigureAwait(false);
            }
            
        }
    }
}
