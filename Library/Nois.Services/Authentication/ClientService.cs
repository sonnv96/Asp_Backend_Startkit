using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Nois.Framework.Services;
using Nois.Framework.Data;
using Nois.Framework.Caching;
using Nois.Core.Domain.Authentication;

namespace Nois.Services.Authentication
{
    /// <summary>
    /// Authentication service
    /// </summary>
    public class ClientService : BaseService<Client>, IClientService
    {
        public ClientService(IRepository<Client> tRepository,
            ICacheManager cacheManager) : base(tRepository, cacheManager)
        {
        }

        protected override string PatternKey
        {
            get
            {
                return "Nois.Client.";
            }
        }

        public virtual Client GetByGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;

            var key = string.Format(PatternKey + "byguid-{0}", guid);

            return _cacheManager.Get(key, () => _tRepository.Table.FirstOrDefault(x=>x.ClientGuid == guid));
        }
        public virtual async Task<Client> GetByGuidAsync(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;

            var key = string.Format(PatternKey + "byguidAsync-{0}", guid); 

            return await _cacheManager.Get(key, async () => {
                return await _tRepository.Table.FirstOrDefaultAsync(x => x.ClientGuid == guid);
            });
        }
    }
}
