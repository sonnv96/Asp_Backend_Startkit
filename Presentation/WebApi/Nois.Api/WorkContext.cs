using Nois.Core.Domain.Users;
using Nois.Framework.Data;
using Nois.Helper;
using Nois.Services.Data;
using System;
using System.Web;

namespace Nois.Api
{
    /// <summary>
    /// WorkContext service
    /// </summary>
    public class WorkContext : IWorkContext
    {
        private HttpContextBase _httpContext;
        private IRepository<User> _userRepository;
        private User _cachedUser;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="userRepository"></param>
        public WorkContext(HttpContextBase httpContext, IRepository<User> userRepository)
        {
            _httpContext = httpContext;
            _userRepository = userRepository;
        }

        /// <summary>
        /// get current user identity
        /// </summary>
        public int CurrentUserId
        {
            get
            {
                //get user guid from http context
                var userId = _httpContext.User.GetValueOfClaim("userid");
                var result = 0;
                Int32.TryParse(userId, out result);

                return result;
            }
        }

        /// <summary>
        /// get current user
        /// </summary>
        public User CurrentUser
        {
            get
            {
                //check user not null
                if (_cachedUser != null)
                    return _cachedUser;
                if (_httpContext.User == null)
                    return null;
                var userId = _httpContext.User.GetValueOfClaim("userid");
                var result = 0;
                Int32.TryParse(userId, out result);

                _cachedUser = _userRepository.GetById(result);

                return _cachedUser;
            }
        }
    }
}
