using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nois.Core.Domain.Users
{
    /// <summary>
    /// Represents the user login result
    /// </summary>
    public class UserLoginResults
    {
        /// <summary>
        /// UserId
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// UserGuid
        /// </summary>
        public Guid UserGuid { get; set; }
        /// <summary>
        /// FullName
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// user login result
        /// </summary>
        public UserLoginResult UserLoginResult { get; set; }
    }
    /// <summary>
    /// Represents the user login result enumeration
    /// </summary>
    public enum UserLoginResult
    {
        /// <summary>
        /// Login successful
        /// </summary>
        Successful = 1,
        /// <summary>
        ///  User dies not exist (email or username)
        /// </summary>
        UserNotExist = 2,
        /// <summary>
        /// Wrong password
        /// </summary>
        WrongPassword = 3,
        /// <summary>
        /// Account have not been activated
        /// </summary>
        NotActive = 4,
        /// <summary>
        ///  User has been deleted 
        /// </summary>
        Deleted = 5,
        /// <summary>
        /// Wrong PIN
        /// </summary>
        WrongPIN = 6,
    }
}