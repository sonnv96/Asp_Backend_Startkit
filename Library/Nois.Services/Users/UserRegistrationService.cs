using Nois.Core.Domain.Users;
using Nois.Framework.Services;
using System.Threading.Tasks;

namespace Nois.Services.Users
{
    /// <summary>
    /// 
    /// </summary>
    public class UserRegistrationService : IUserRegistrationService
    {
        #region variables

        private readonly IUserService _userService;
        private readonly IEncryptionService _encryptionService;

        #endregion

        #region constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="encryptionService"></param>
        public UserRegistrationService(IUserService userService,
            IEncryptionService encryptionService)
        {
            _userService = userService;
            _encryptionService = encryptionService;
        }


        #endregion

        #region methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserLoginResults ValidateUser(string username, string password)
        {
            var res = new UserLoginResults();
            User user = _userService.GetUserByUsername(username);
            if (user == null)
            {
                res.UserLoginResult = UserLoginResult.UserNotExist;
                return res;
            }
            if (user == null)
            {
                res.UserLoginResult = UserLoginResult.Deleted;
                return res;
            }
            if (!user.IsActive)
            {
                res.UserLoginResult = UserLoginResult.NotActive;
                return res;
            }
            //only registered can login
            var pwd = _encryptionService.CreatePasswordHash(password, user.PasswordSalt);
            bool isValid = pwd == user.Password;
            if (!isValid)
            {
                res.UserLoginResult = UserLoginResult.WrongPassword;
                return res;
            }
            res.UserId = user.Id;
            res.UserGuid = user.UserGuid;
            res.FullName = user.FullName;
            res.UserLoginResult = UserLoginResult.Successful;
            //TODO:save last login date
            return res;
        }

        /// <summary>
        /// Check user login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<UserLoginResults> ValidateUserAsync(string username, string password)
        {
            var res = new UserLoginResults();

            // find user from db by username
            var user = await _userService.GetUserByUsernameAsync(username);

            // user cannot not be found
            if (user == null)
            {
                res.UserLoginResult = UserLoginResult.UserNotExist;
                return res;
            }

            // user has deleted
            if (user.Deleted)
            {
                res.UserLoginResult = UserLoginResult.Deleted;
                return res;
            }

            // user not active
            if (!user.IsActive)
            {
                res.UserLoginResult = UserLoginResult.NotActive;
                return res;
            }

            // check password login
            if (_encryptionService.CreatePasswordHash(password, user.PasswordSalt) != user.Password)
            {
                res.UserLoginResult = UserLoginResult.WrongPassword;
                return res;
            }

            // return user data
            res.UserId = user.Id;
            res.UserGuid = user.UserGuid;
            res.FullName = user.FullName;
            res.UserLoginResult = UserLoginResult.Successful;

            //TODO:save last login date
            return res;
        }
        #endregion

    }
}