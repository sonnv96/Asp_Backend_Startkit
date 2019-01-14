using Nois.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nois.Services.Users
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserRegistrationService
    {
        /// <summary>
        /// validate user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserLoginResults ValidateUser(string email, string password);
        /// <summary>
        /// validate user async
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<UserLoginResults> ValidateUserAsync(string email, string password);
    }
}
