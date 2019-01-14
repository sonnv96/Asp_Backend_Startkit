using Nois.Core.Domain.Users;
using Nois.Framework.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Nois.Services.Data
{
    /// <summary>
    /// WorkContext service interface
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// get current user identity
        /// </summary>
        int CurrentUserId { get; }
        /// <summary>
        /// get current user
        /// </summary>
        User CurrentUser { get; }
    }
}
