using System.Security.Claims;

namespace JwtAuthApi.Services
{
    public interface ITokenService
    {
        public string GenerateAccessToken(IEnumerable<Claim> claims);
        public string GenerateRefreshToken();
        public ClaimsPrincipal GetPrincipleFromExpiredToken(string token);
    }
}