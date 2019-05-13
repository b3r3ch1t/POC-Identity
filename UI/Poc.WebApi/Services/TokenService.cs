using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Poc.Infra.CrossCutting.Identity.Authorization;
using Poc.Infra.CrossCutting.Identity.Interfaces;
using Poc.Infra.CrossCutting.Identity.Models;
using Poc.Infra.CrossCutting.Identity.Repository;
using POC.Domain.Core.Extensions;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Poc.WebApi.Services
{
    public class TokenService : ITokenService
    {

        private readonly IUserIdentityRepository _userIdentityRepository;
        private readonly JwtTokenOptions _jwtTokenOptions;

        private readonly IConfiguration _configuration;

        public TokenService(IUserIdentityRepository userIdentityRepository,
            JwtTokenOptions jwtTokenOptions, IConfiguration configuration)
        {
            _userIdentityRepository = userIdentityRepository;
            _jwtTokenOptions = jwtTokenOptions;
            _configuration = configuration;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetUserIdFromExpiredToken(string token)
        {

            var key = _configuration["AppSettings:Secret"];

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new ClaimsPrincipal();
            }

        }

        public string CreateTokenUser(string username)
        {
            var user = _userIdentityRepository.GetUserByUsername(username);

            var userClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,   _jwtTokenOptions.JtiGenerator().Result ),
                new Claim(JwtRegisteredClaimNames.Iat, _jwtTokenOptions.IssuedAt.ToUnixTimeString())
            };

            #region RefreshToken 

            var token = _userIdentityRepository.GetTokenByUserId(user.Id);
            if (string.IsNullOrEmpty(token))
            {
                token = GenerateRefreshToken();
            }

            userClaims.Add(new Claim(type: "TokenRefresh", value: token));

            _userIdentityRepository.AddRefreshToken(user, token);


            #endregion

            #region Add User_ID

            userClaims.Add(new Claim(type: "User_Id", value: user.Id.ToString()));


            #endregion

            var claims = _userIdentityRepository.GetClaimsAsync(user).Result;

            userClaims.AddRange(claims.Select(x => new Claim(x.Type, x.Value)));

            var expires = _jwtTokenOptions.Expiration;

            userClaims.Add(new Claim("Expire", expires.ToStringDataComplete()));

            var jwt = new JwtSecurityToken(
                _jwtTokenOptions.Issuer,
                _jwtTokenOptions.Audience,
                userClaims,
                _jwtTokenOptions.NotBefore,
                expires,
                _jwtTokenOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        public ResponseTokenRefreshViewModel ValidateToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return new ResponseTokenRefreshViewModel()
                {
                    Message = "Refresh Token invalid !!!",
                    Valid = false
                };
            }

            var listClaims = GetUserIdFromExpiredToken(refreshToken);

            var claim = listClaims.Claims.FirstOrDefault(x => x.Type == "TokenRefresh");
            if (claim == null)
            {
                return new ResponseTokenRefreshViewModel()
                {
                    Message = "Refresh Token invalid !!!",
                    Valid = false
                };
            }

            var userId = listClaims.Claims.FirstOrDefault(x => x.Type == "User_Id");

            if (userId == null)
            {
                return new ResponseTokenRefreshViewModel()
                {
                    Message = "Refresh Token invalid !!!",
                    Valid = false
                };
            }

            var user = _userIdentityRepository.GetUserByIdAsync(new Guid(userId.Value)).Result;
            if (user == null)
            {
                return new ResponseTokenRefreshViewModel()
                {
                    Message = "Refresh Token invalid !!!",
                    Valid = false
                };
            }

            var token = _userIdentityRepository.GetTokenByUserId(new Guid(userId.Value));

            if (token == null)
            {
                return new ResponseTokenRefreshViewModel()
                {
                    Message = "Refresh Token invalid !!!",
                    Valid = false
                };
            }

            var result = CreateTokenUser(user.UserName);


            return new ResponseTokenRefreshViewModel()
            {
                Message = result,
                Valid = true
            };

        }
    }
}
