using Nois.Framework.Services;
using Nois.Core.Domain.Users;
using System.Collections.Generic;

namespace Nois.Services.Users
{
    /// <summary>
    /// ActionName service interface
    /// </summary>
    public interface IActionNameService : IBaseService<ActionName>
    {
        /// <summary>
        /// get action name by list permission id
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        List<ActionName> GetByPermissions(List<int> permissionId);
        /// <summary>
        /// get action name of allow anonymous
        /// </summary>
        /// <returns></returns>
        List<ActionName> GetSharedActionName();
        /// <summary>
        /// Get action name of user
        /// </summary>
        /// <param name="userId">User identity</param>
        /// <returns></returns>
        List<ActionName> GetByUser(int userId);
    }
}
