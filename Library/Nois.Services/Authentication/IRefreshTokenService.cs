using Nois.Core.Domain.Authentication;
using Nois.Framework.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nois.Services.Authentication
{
    public interface IRefreshTokenService : IBaseService<RefreshToken>
    {
        /// <summary>
        /// Get refresh token by TokenGuid
        /// </summary>
        /// <param name="guid">TokenGuid</param>
        /// <returns></returns>
        RefreshToken GetByGuid(string guid);

        /// <summary>
        /// Get refresh token by TokenGuid
        /// </summary>
        /// <param name="guid">TokenGuid</param>
        /// <returns></returns>
        Task<RefreshToken> GetByGuidAsync(string guid);
        /// <summary>
        /// Get list refresh token by Subject
        /// </summary>
        /// <param name="subject">subject</param>
        /// <returns></returns>
        List<RefreshToken> GetListBySubject(string subject);
        /// <summary>
        /// Get list refresh token by Subject
        /// </summary>
        /// <param name="subject">subject</param>
        /// <returns></returns>
        Task<List<RefreshToken>> GeListBySubjectAsync(string subject);
    }
}
