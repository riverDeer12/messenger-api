using AutoMapper;
using MessengerAPI.Constants;
using MessengerAPI.Data;
using MessengerAPI.Data.DataTransferObjects.ApplicationUsers;
using MessengerAPI.Data.Models;
using MessengerAPI.Helpers;
using MessengerAPI.Services.HelperClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Services
{
    public class ApplicationUsersManager
    {
        private readonly Model _db;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LogsManager _logsManager;
        private readonly AuthManager _authManager;

        public ApplicationUsersManager(Model context, IMapper mapper, UserManager<ApplicationUser> userManager, IOptions<ApplicationSettings> appSettings)
        {
            _db = context;
            _mapper = mapper;
            _userManager = userManager;
            _logsManager = new LogsManager(context);
            _authManager = new AuthManager(appSettings, userManager);
        }

        /// <summary>
        /// Get all application users
        /// with user role.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<UserDetailsDto>> GetUsers()
        {
            var finalUsers = new List<ApplicationUser>();

            var dbUsers = await _db.ApplicationUsers.Where(x => x.Active).ToListAsync();

            foreach (var user in dbUsers)
            {
                if (await _userManager.IsInRoleAsync(user, Roles.User))
                {
                    finalUsers.Add(user);
                }
            }

            return _mapper.Map<IEnumerable<UserDetailsDto>>(finalUsers);
        }

        /// <summary>
        /// Use registration data to register
        /// new application admin.
        /// </summary>
        /// <param name="registrationData"></param>
        /// <returns></returns>
        public async Task<bool> RegisterAdmin(RegistrationDto registrationData)
        {
            var applicationAdmin = _mapper.Map<ApplicationUser>(registrationData);

            try
            {
                var adminCreationResult = await _userManager.CreateAsync(applicationAdmin, registrationData.Password);

                if (adminCreationResult.Succeeded) await _userManager.AddToRoleAsync(applicationAdmin, Roles.Admin);

                return true;
            }
            catch (Exception ex)
            {
                _logsManager.SaveLog(registrationData, ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Use registration data and
        /// save new user to db.
        /// </summary>
        /// <param name="registrationData"></param>
        /// <returns></returns>
        public async Task<UserRegistrationResponse> RegisterUser(RegistrationDto registrationData)
        {
            var user = _mapper.Map<ApplicationUser>(registrationData);

            try
            {
                var creationResult = await _userManager.CreateAsync(user, registrationData.Password);

                if (!creationResult.Succeeded) return UserRegistrationResponse.Unsuccessful(creationResult); 

                await _userManager.AddToRoleAsync(user, Roles.User);
            }
            catch (Exception ex)
            {
                _logsManager.SaveLog(registrationData, ex.Message);
            }

            var token = await _authManager.GenerateJwtToken(user);

            return UserRegistrationResponse.Successfull(token);
        }

        /// <summary>
        /// Get user details by
        /// user id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserDetailsDto> GetUserDetails(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var userDetails = _mapper.Map<UserDetailsDto>(user);

            return userDetails;
        }
    }
}
