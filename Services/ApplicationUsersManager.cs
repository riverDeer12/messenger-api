using AutoMapper;
using MessengerAPI.Constants;
using MessengerAPI.Data;
using MessengerAPI.Data.DataTransferObjects.ApplicationUsers;
using MessengerAPI.Data.DataTransferObjects.Messages;
using MessengerAPI.Data.Models;
using MessengerAPI.Helpers;
using MessengerAPI.Services.HelperClasses;
using MessengerAPI.Services.ResponseClasses;
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
        /// Get all active users
        /// excluding logged user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserResponse> GetSearchUsers(string userId)
        {
            var loggedUser = await _userManager.FindByIdAsync(userId);

            if (loggedUser == null) return UserResponse.Unsuccessful("Logged user not found.");

            var activeUsers = await GetSearchActiveUsers(loggedUser);

            return UserResponse.Successful(activeUsers);
        }

        /// <summary>
        /// Get all active users
        /// for search with excluded
        /// currently logged user.
        /// </summary>
        /// <param name="loggedUser"></param>
        /// <returns></returns>
        private async Task<List<UserDetailsDto>> GetSearchActiveUsers(ApplicationUser loggedUser)
        {
            var activeUsers = await _db.ApplicationUsers.Where(x => x.Active).ToListAsync();

            if (activeUsers.Contains(loggedUser)) activeUsers.Remove(loggedUser);

            var userDetailsList = _mapper.Map<List<UserDetailsDto>>(activeUsers);

            return userDetailsList;
        }

        /// <summary>
        /// Get user details by
        /// claims principal.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<UserDetailsDto> GetUserDetails(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var userDetails = _mapper.Map<UserDetailsDto>(user);

            return userDetails;
        }

        /// <summary>
        /// Find user by sent id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// User object if successfully found.
        /// If not returns null.
        /// </returns>
        public async Task<ApplicationUser> FindUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return null;

            return user;
        }
    }
}
