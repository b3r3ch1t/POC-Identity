using System.Security.Claims;
using Poc.Infra.CrossCutting.Identity.Models;

namespace Poc.Infra.CrossCutting.Identity.Interfaces
{
    public interface ITokenService
    {
        string GenerateRefreshToken();
        ClaimsPrincipal GetUserIdFromExpiredToken(string token);
        string CreateTokenUser(string username);
        ResponseTokenRefreshViewModel ValidateToken(string refreshToken);
    }
}
