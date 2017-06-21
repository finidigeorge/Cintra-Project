using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Controllers.Auth;
using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Repositories.Interfaces;
using Shared.Dto;
using Shared.Interfaces;

namespace Controllers
{        
    public class AuthController : Controller, IAuthController
    {
        private readonly JwtTokenOptions _jwtOptions;
        private readonly ILogger _logger;
        private readonly IUsersRepository _userRepository;        
        private static string JwtRoleToMsIdentiryMapKey = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        public AuthController(IOptions<JwtTokenOptions> jwtOptions, IUsersRepository userRepository, ILoggerFactory loggerFactory)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _logger = loggerFactory.CreateLogger<AuthController>();
            _userRepository = userRepository;            
        }

        #region passwords utility

        private const int SALT_LENGTH = 16;

        private static string GetSalt()
        {
            byte[] bytes = new byte[SALT_LENGTH];
            using (var keyGenerator = RandomNumberGenerator.Create())
            {
                keyGenerator.GetBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private static string GetSaltFromStoredPassword(string password)
        {
            return password.Substring(password.Length - SALT_LENGTH * 2);
        }

        private static string GeneratePassword(string plainPassword, string salt = null)
        {
            using (var sha256 = SHA256.Create())
            {
                if (salt == null)
                    salt = GetSalt();

                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainPassword + salt));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower() + salt;
            }
        }

        private static bool IsPasswordValid(User user, string password)
        {
            var testPassword = GeneratePassword(password, GetSaltFromStoredPassword(user.Password));
            return testPassword == user.Password;
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(UserDto userDto)
        {
            var user = (await _userRepository.GetByParamsWithRoles(x => x.Login == userDto.Login)).FirstOrDefault();
            if (user != null && IsPasswordValid(user, userDto.Password))
            {
                return new ClaimsIdentity(
                    new GenericIdentity(user.Login, "Token"),
                    new[]
                    {
                        new Claim("User", user.Login),
                        new Claim(JwtRoleToMsIdentiryMapKey, user.user_roles.Name)
                    });
            }

            return null;
        }

        #endregion

        [HttpGet]
        [AllowAnonymous]
        [Route("/api/auth/password/{password}")]
        public string GetPassword(string password)
        {
            return GeneratePassword(password);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/api/auth/login")]
        public async Task<JwtTokenDto> Login([FromBody]UserDto applicationUser)
        {
            var identity = await GetClaimsIdentity(applicationUser);
            if (identity == null)
            {
                _logger.LogInformation($"Invalid login ({applicationUser.Login}) or password ({applicationUser.Password})");
                throw new AuthenticationException($"Invalid login or password");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Login),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                identity.FindFirst("User"),
                identity.FindFirst(JwtRoleToMsIdentiryMapKey),
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,                
                expires: _jwtOptions.ExpiredAt,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            
            return new JwtTokenDto
            {
                AccessToken = encodedJwt,
                ExpiresIn = (int)_jwtOptions.ValidFor.TotalSeconds
            };
        }

                

        private static void ThrowIfInvalidOptions(JwtTokenOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtTokenOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtTokenOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtTokenOptions.JtiGenerator));
            }
        }

        // Date converted to seconds since Jan 1, 1970, midnight UTC
        private static long ToUnixDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() -
                                 new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalSeconds);        
    }
}
