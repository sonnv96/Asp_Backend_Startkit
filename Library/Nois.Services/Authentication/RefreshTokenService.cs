using System.Collections.Generic;
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
    public class RefreshTokenService :  BaseService<RefreshToken>, IRefreshTokenService
    {
        public RefreshTokenService(IRepository<RefreshToken> refreshTokenRepository,
            ICacheManager cacheManager) : base(refreshTokenRepository, cacheManager)
        {
        }

        protected override string PatternKey
        {
            get
            {
                return "Nois.RefreshToken.";
            }
        }

        public virtual RefreshToken GetByGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;

            var key = string.Format(PatternKey + "byguid-{0}", guid);

            return _cacheManager.Get(key, () => _tRepository.Table.FirstOrDefault(x => x.TokenGuid == guid));
        }
        public virtual async Task<RefreshToken> GetByGuidAsync(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;

            var key = string.Format(PatternKey + "byguidAsync-{0}", guid);

            return await _cacheManager.Get(key, async () => {
                return await _tRepository.Table.FirstOrDefaultAsync(x => x.TokenGuid == guid);
            });
        }

        public List<RefreshToken> GetListBySubject(string subject)
        {
            if (string.IsNullOrEmpty(subject))
                return new List<RefreshToken>();

            var key = string.Format(PatternKey + "bysubject-{0}", subject);

            return _cacheManager.Get(key, () => _tRepository.Table.Where(x => x.Subject == subject).ToList());
        }
        public async Task<List<RefreshToken>> GeListBySubjectAsync(string subject)
        {
            if (string.IsNullOrEmpty(subject))
                return new List<RefreshToken>();

            var key = string.Format(PatternKey + "bysubjectAsync-{0}", subject);

            return await _cacheManager.Get(key, async() => await _tRepository.Table.Where(x => x.Subject == subject).ToListAsync());
        }
    }
}
