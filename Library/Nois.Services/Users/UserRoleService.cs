using Nois.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Nois.Framework.Caching;
using Nois.Framework.Data;
using Nois.Core.Domain.Users;
using System.Threading.Tasks;
using Nois.Helper;

namespace Nois.Services.Users
{
    /// <summary>
    /// Implement User Role service
    /// </summary>
    public partial class UserRoleService : BaseService<UserRole>, IUserRoleService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRoleRepository"></param>
        /// <param name="cacheManager"></param>
        public UserRoleService(IRepository<UserRole> userRoleRepository, ICacheManager cacheManager) 
            : base(userRoleRepository, cacheManager)
        {
        }
        /// <summary>
        /// Pattern key
        /// </summary>
        protected override string PatternKey
        {
            get
            {
                return "Nois.UserRole.";
            }
        }

        public async Task<IPagedList<UserRole>> SearchAsync(int pageIndex = 0, int pageSize = int.MaxValue,
            string textSearch = null,
            string propertySorting = null, bool orderDescending = false)
        {
            var query = _tRepository.Table;
            if (!string.IsNullOrEmpty(textSearch))
                query = query.Where(x => x.Name.Contains(textSearch) || x.SystemName.Contains(textSearch));
            return await Task.FromResult(new PagedList<UserRole>(query.OrderByDynamic(propertySorting, "Name", orderDescending), pageIndex, pageSize) as IPagedList<UserRole>);
        }

        public IPagedList<UserRole> Search(int pageIndex = 0, int pageSize = int.MaxValue,
            string textSearch = null,
            string propertySorting = null, bool orderDescending = false,
            bool? isActive = null, bool? isDelete = null, List<int> exceptRoleIds = null)
        {
            var query = _tRepository.Table;
            query = query.Where(x => !x.SystemName.ToLower().Equals("system"));

            if (exceptRoleIds != null && exceptRoleIds.Any())
                query = query.Where(r => !exceptRoleIds.Contains(r.Id));

            if (!string.IsNullOrEmpty(textSearch))
                query = query.Where(x => x.Name.Contains(textSearch) || x.SystemName.Contains(textSearch));

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive);

            if (isDelete.HasValue)
                query = query.Where(x => x.Deleted == isDelete);

            return new PagedList<UserRole>(query.OrderByDynamic(propertySorting, "Name", orderDescending), pageIndex, pageSize) as IPagedList<UserRole>;
        }

        public List<UserRole> GetByUserId(int userId)
        {
            var roles = _tRepository.Table.Where(x => x.Users.Any(a => a.Id == userId) && x.IsActive && !x.Deleted);
            return roles.ToList();
        }

        public UserRole GetBySystemName(string systemName)
        {
            return _tRepository.Table.FirstOrDefault(ur => ur.SystemName == systemName);
        }
    }
}