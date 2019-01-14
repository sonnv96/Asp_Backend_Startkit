using Nois.Core.Domain.Users;
using Nois.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nois.Services.Users
{
    public partial interface IPermissionService : IBaseService<Permission>
    {
        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="user">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize(Permission permission, User user);
        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="user">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize(string permissionRecordSystemName, User user);
        /// <summary>
        /// get permission by list role id
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        List<Permission> GetByRoles(List<int> roleIds);
        /// <summary>
        /// get Permissions
        /// </summary>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="textSearch">text search</param>
        /// <param name="propertySorting">property sorting</param>
        /// <param name="orderDescending">order by descending</param>
        /// <returns></returns>
        IPagedList<Permission> Search(int pageIndex = 0, int pageSize = int.MaxValue, string textSearch = null, string propertySorting = null, bool orderDescending = false);
    }
}
