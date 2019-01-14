using Nois.Core.Domain.Users;
using Nois.Framework.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nois.Services.Users
{
    /// <summary>
    /// User service
    /// </summary>
    public partial interface IUserRoleService : IBaseService<UserRole>
    {
        /// <summary>
        /// get user roles async
        /// </summary>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="textSearch">text search</param>
        /// <param name="propertySorting">property sorting</param>
        /// <param name="orderDescending">order by descending</param>
        /// <returns></returns>
        Task<IPagedList<UserRole>> SearchAsync(int pageIndex = 0, int pageSize = int.MaxValue, string textSearch = null, string propertySorting = null, bool orderDescending = false);
        /// <summary>
        /// get user roles
        /// </summary>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="textSearch">text search</param>
        /// <param name="propertySorting">property sorting</param>
        /// <param name="orderDescending">order by descending</param>
        /// <param name="isActive">order by descending</param>
        /// <param name="isDelete">order by descending</param>
        /// <param name="exceptRoleIds">order by descending</param>
        /// <returns></returns>
        IPagedList<UserRole> Search(int pageIndex = 0, int pageSize = int.MaxValue,
            string textSearch = null,
            string propertySorting = null, bool orderDescending = false,
            bool? isActive = null, bool? isDelete = null, List<int> exceptRoleIds = null);
        /// <summary>
        /// get by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<UserRole> GetByUserId(int userId);
        /// <summary>
        /// Get by system name;
        /// </summary>
        /// <param name="systemName"></param>
        /// <returns></returns>
        UserRole GetBySystemName(string systemName);
    }
}
