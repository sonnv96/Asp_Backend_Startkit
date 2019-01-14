using System.Linq;
using Nois.Framework.Services;
using Nois.Framework.Data;
using Nois.Framework.Caching;
using Nois.Core.Domain.Users;
using System.Collections.Generic;

namespace Nois.Services.Users
{
    /// <summary>
    /// ActionName service
    /// </summary>
    public class ActionNameService : BaseService<ActionName>, IActionNameService
    {
        private readonly IRepository<User> _userRepository;

        public ActionNameService(IRepository<ActionName> tRepository,
            IRepository<User> userRepository,
            ICacheManager cacheManager) : base(tRepository, cacheManager)
        {
            this._userRepository = userRepository;
        }

        protected override string PatternKey
        {
            get
            {
                return "Nois.ActionName.";
            }
        }

        public List<ActionName> GetSharedActionName()
        {
            var actionNames = _tRepository.Table.Where(x => !x.Permissions.Any());
            return actionNames.ToList();
        }

        public List<ActionName> GetByPermissions(List<int> permissionIds)
        {
            var actionNames = _tRepository.Table.Where(x => x.Permissions.Any(a => permissionIds.Contains(a.Id)));
            return actionNames.ToList();
        }

        public List<ActionName> GetByUser(int userId)
        {
            var user = _userRepository.GetById(userId);
            return user.UserRoles.Where(x => x.IsActive && !x.Deleted).SelectMany(ur => ur.Permissions.SelectMany(p => p.ActionNames)).ToList();
        }
    }
}
