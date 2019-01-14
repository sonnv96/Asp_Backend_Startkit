using Nois.Core.Domain.Authentication;
using Nois.Framework.Services;
using System.Threading.Tasks;

namespace Nois.Services.Authentication
{
    public interface IClientService : IBaseService<Client>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        Client GetByGuid(string guid);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task<Client> GetByGuidAsync(string guid);
    }
}
