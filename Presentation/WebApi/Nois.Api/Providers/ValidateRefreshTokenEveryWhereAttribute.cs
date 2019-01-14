using Nois.WebApi.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Security.Principal;
using System.Security.Claims;
using Nois.Services.Authentication;
using Nois.Services.Users;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nois.Helper;
using Nois.Framework.Infrastructure;
using Nois.Framework.Localization;

namespace Nois.Api.Providers
{
    /// <summary>
    /// Use for checking user has been sign out every where by owner
    /// </summary>
    public class ValidateRefreshTokenEveryWhereAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var localStringResourceService = EngineContext.Instance.Resolve<ILocalizationService>();
            var apiJsonResult = new ApiJsonResult
            {
                ErrorMessages = new List<string> { localStringResourceService.GetResource("Permission.NotPermission") }
            };
            var response = new HttpApiActionResult(HttpStatusCode.BadRequest, apiJsonResult);

            // The action filter logic.
            var currentRefreshToken = GetValueOfClaim(HttpContext.Current.User, ClaimName.RefreshTokenKey);

            var refreshTokenService = EngineContext.Instance.Resolve<IRefreshTokenService>();
            var actionNameService = EngineContext.Instance.Resolve<IActionNameService>();
            var refreshToken = refreshTokenService.GetByGuid(currentRefreshToken);
            if (refreshToken == null)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            else
            {
                if (DateTime.UtcNow > refreshToken.ExpiresUtc)
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                //if (refreshToken.)
                //    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;
                if (principal == null)
                {
                    actionContext.Response = response.ExecuteAsync(new CancellationToken()).Result;
                    return;
                }
                //get shared action name
                var sharedActionNames = actionNameService.GetSharedActionName();
                //get current controller and action
                var controllerName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                var actionName = actionContext.ActionDescriptor.ActionName;

                if (sharedActionNames.Any(x => string.Equals(controllerName,x.Controller, StringComparison.OrdinalIgnoreCase) && string.Equals(actionName,x.Name,StringComparison.OrdinalIgnoreCase)))
                {
                    return;
                }
                // get action name for permission from claim
                var userId = HttpContext.Current.User.GetValueOfClaim("userid");
                var result = 0;
                Int32.TryParse(userId, out result);
                var listActionName = actionNameService.GetByUser(result);

                if(!listActionName.Any(x =>x.Controller == controllerName && x.Name == actionName))
                {
                    actionContext.Response = response.ExecuteAsync(new CancellationToken()).Result;
                    return;
                }
            }
        }
        private string GetValueOfClaim(IPrincipal user, string claimName)
        {
            if (user.Identity.IsAuthenticated)
            {
                var claimsIdentity = user.Identity as ClaimsIdentity;
                if (claimsIdentity == null) return "";
                foreach (var claim in claimsIdentity.Claims)
                {
                    if (claim.Type == claimName)
                        return claim.Value;
                }
                return "";
            }
            else
                return "";
        }
    }
}