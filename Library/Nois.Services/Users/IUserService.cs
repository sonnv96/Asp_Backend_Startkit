using Nois.Core.Domain.Users;
using Nois.Framework.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nois.Services.Users
{
    /// <summary>
    /// User service
    /// </summary>
    public partial interface IUserService : IBaseService<User>
    {
        #region Synchronous
        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">user email address</param>
        /// <returns></returns>
        User GetUserByEmail(string email);
        
        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">username address</param>
        /// <returns></returns>
        User GetUserByUsername(string username);
        /// <summary>
        /// Get user by barcode
        /// </summary>
        /// <param name="barcode">barcode</param>
        /// <returns></returns>
        //User GetUserByBarcode(string barcode);
        /// <summary>
        /// get users
        /// </summary>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="textSearch">text search</param>
        /// <param name="propertySorting">property sorting</param>
        /// <param name="orderDescending">order by descending</param>
        /// <param name="includeCurrentUser">includeCurrentUser</param>
        /// <returns></returns>
        IPagedList<User> Search(int pageIndex = 0, int pageSize = int.MaxValue, string textSearch = null, string propertySorting = null, bool orderDescending = false, bool includeCurrentUser = true);
        /// <summary>
        /// Import user from csv file
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        void ImportUser(Stream stream, string fileName);
        #endregion
        #region Asynchronous
        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">user email address</param>
        /// <returns></returns>
        Task<User> GetUserByEmailAsync(string email);
        /// <summary>
        /// Get user by username async
        /// </summary>
        /// <param name="username">username address</param>
        /// <returns></returns>
        Task<User> GetUserByUsernameAsync(string username);
        /// <summary>
        /// Get user by barcode async
        /// </summary>
        /// <param name="barcode">barcode</param>
        /// <returns></returns>
        //Task<User> GetUserByBarcodeAsync(string barcode);
        /// <summary>
        /// get users async
        /// </summary>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="textSearch">text search</param>
        /// <param name="propertySorting">property sorting</param>
        /// <param name="orderDescending">order by descending</param>
        /// <returns></returns>
        Task<IPagedList<User>> SearchAsync(int pageIndex = 0, int pageSize = int.MaxValue, string textSearch = null, string propertySorting = null, bool orderDescending = false);
        #endregion
    }
}
