using MessengerAPI.Data.Models;
using MessengerAPI.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MessengerAPI.Services
{
    public class AuthManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationSettings _appSettings;

        public AuthManager(IOptions<ApplicationSettings> appSettings, UserManager<ApplicationUser> userManager)
        {
            _appSettings = appSettings.Value;
            _userManager = userManager;
        }

        /// <summary>
        /// Perform authorization on
        /// application user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> AuthorizeUser(ApplicationUser user, string password)
        {
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return false;

            return true;
        }

        /// <summary>
        /// Generate JWT for 
        /// authentication.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var tokenDescriptor = await SetTokenDescriptor(user);

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }
        
        /// <summary>
        /// Find application user
        /// by username.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> FindByUserName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            return user;
        }

        /// <summary>
        /// Set descriptor for generating
        /// JWT token.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<SecurityTokenDescriptor> SetTokenDescriptor(ApplicationUser user)
        {
            IdentityOptions _roleOptions = new IdentityOptions();

            var secretKey = Encoding.UTF8.GetBytes(_appSettings.JWTSecretKey);

            return new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(_roleOptions.ClaimsIdentity.RoleClaimType, await GetRoleOptions(user))
                }),

                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };
        }

        /// <summary>
        /// Get list of roles
        /// for user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<string> GetRoleOptions(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }
    }
}
