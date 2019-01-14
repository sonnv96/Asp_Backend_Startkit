using Nois.Core.Domain.Users;
using Nois.Helper;
using Nois.Framework.Caching;
using Nois.Framework.Data;
using Nois.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nois.Services.Users
{
    public partial class PermissionService : BaseService<Permission>, IPermissionService
    {

        private const string PERMISSIONS_ALLOWED_KEY = "Nois.permission.allowed-{0}-{1}";
        protected override string PatternKey
        {
            get
            {
                return "Nois.permission.";
            }
        }

        public PermissionService(IRepository<Permission> permissionRepository,
            ICacheManager cacheManager)
            : base(permissionRepository, cacheManager)
        {
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="customerRole">Customer role</param>
        /// <returns>true - authorized; otherwise, false</returns>
        protected virtual bool Authorize(string permissionRecordSystemName, UserRole userRole)
        {
            if (String.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            string key = string.Format(PERMISSIONS_ALLOWED_KEY, userRole.Id, permissionRecordSystemName);
            return _cacheManager.Get(key, () =>
            {
                foreach (var permission1 in userRole.Permissions)
                    if (permission1.SystemName.Equals(permissionRecordSystemName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                return false;
            });
        }
        
        public bool Authorize(Permission permission, User user)
        {
            if (permission == null)
                return false;

            if (user == null)
                return false;

            return Authorize(permission.SystemName, user);
        }

        public bool Authorize(string permissionRecordSystemName, User user)
        {
            if (String.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var userRoles = user.UserRoles.Where(cr => cr.IsActive);
            foreach (var role in userRoles)
                if (Authorize(permissionRecordSystemName, role))
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }

        public List<Permission> GetByRoles(List<int> roleIds)
        {
            var permission = _tRepository.Table.Where(x => x.UserRoles.Any(a => roleIds.Contains(a.Id)));
            return permission.ToList();
        }

        public IPagedList<Permission> Search(int pageIndex = 0, int pageSize = int.MaxValue, string textSearch = null, string propertySorting = null, bool orderDescending = false)
        {
            var query = _tRepository.Table;
            query = query.Where(x => !x.SystemName.ToLower().Equals("system.admin"));
            if (!string.IsNullOrEmpty(textSearch))
                query = query.Where(x => x.Name.Contains(textSearch) || x.SystemName.Contains(textSearch));
            return new PagedList<Permission>(query.OrderByDynamic(propertySorting, "Name", orderDescending), pageIndex, pageSize) as IPagedList<Permission>;
        }
    }
}
