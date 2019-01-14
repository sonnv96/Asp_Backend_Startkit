using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Linq;
using Nois.Framework.Infrastructure;
using Nois.Core.Domain.Authentication;
using Nois.Services.Authentication;
using Nois.Services.Users;
using Nois.Core.Domain.Users;
using Nois.Framework.Services;

namespace Nois.Api.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            Client client = null;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                //context.Validated();
                context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult<object>(null);
            }

            var clientService = EngineContext.Instance.Resolve<IClientService>();
            client = clientService.GetByGuid(context.ClientId);

            if (client == null)
            {
                context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                return Task.FromResult<object>(null);
            }

            if (client.ApplicationType == ApplicationTypes.NativeConfidential)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret should be sent.");
                    return Task.FromResult<object>(null);
                }
                else
                {
                    var encryptionService = EngineContext.Instance.Resolve<IEncryptionService>();
                    if (client.Secret != encryptionService.GetHash(clientSecret))
                    {
                        context.SetError("invalid_clientId", "Client secret is invalid.");
                        return Task.FromResult<object>(null);
                    }
                }
            }

            if (!client.Active)
            {
                context.SetError("invalid_clientId", "Client is inactive.");
                return Task.FromResult<object>(null);
            }

            context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated();
            return Task.FromResult<object>(null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

            if (allowedOrigin == null) allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var userRegistrationService = EngineContext.Instance.Resolve<IUserRegistrationService>();
            var loginResult = await userRegistrationService.ValidateUserAsync(context.UserName, context.Password);

            if (loginResult.UserLoginResult != UserLoginResult.Successful)
            {
                switch (loginResult.UserLoginResult)
                {
                    case UserLoginResult.UserNotExist:
                        context.SetError("invalid_grant", "User is not exist");
                        break;
                    case UserLoginResult.Deleted:
                        context.SetError("invalid_grant", "User was deleted");
                        break;
                    case UserLoginResult.NotActive:
                        context.SetError("invalid_grant", "User is not actived");
                        break;
                    case UserLoginResult.WrongPIN:
                        context.SetError("invalid_grant", "Wrong PIN");
                        break;
                    case UserLoginResult.WrongPassword:
                    default:
                        context.SetError("invalid_grant", "Wrong Credentials");
                        break;
                }
                return;
            }

            var clientService = EngineContext.Instance.Resolve<IClientService>();
            var client = await clientService.GetByGuidAsync(context.ClientId);

            var refreshTokenId = Guid.NewGuid().ToString("n");
            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

            var token = new RefreshToken()
            {
                TokenGuid = refreshTokenId,
                ClientId = client.Id,
                Subject = context.UserName,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
            };

            var refreshTokenService = EngineContext.Instance.Resolve<IRefreshTokenService>();
            var userService = EngineContext.Instance.Resolve<IUserService>();

            var user = userService.GetById(loginResult.UserId);
            user.LatestLoggedin = DateTime.Now;
            user.ActionActor = user.Email;
            user.ActionDescription = "loged-in at";
            userService.Update(user);

            await refreshTokenService.InsertAsync(token);

            var permissionService = EngineContext.Instance.Resolve<IPermissionService>();
            var actionNameService = EngineContext.Instance.Resolve<IActionNameService>();
            var roleService = EngineContext.Instance.Resolve<IUserRoleService>();
            var roles = roleService.GetByUserId(loginResult.UserId);
            var permissions = permissionService.GetByRoles(roles.Select(x => x.Id).ToList());
            var actionNames = actionNameService.GetByPermissions(permissions.Select(x => x.Id).ToList());
            //convert to string
            var listPermission = String.Join(",", permissions.Select(x=>x.SystemName).ToArray());
            var listRole = String.Join(",", roles.Select(x => x.SystemName).ToArray());
            var listActionName= String.Join(",", actionNames.Select(x =>x.Controller+"."+x.Name).ToArray());

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimName.UsernameKey, context.UserName));
            identity.AddClaim(new Claim(ClaimName.RefreshTokenKey, token.TokenGuid));
            identity.AddClaim(new Claim(ClaimName.ActionName, listActionName));
            identity.AddClaim(new Claim(ClaimName.UserIdKey, loginResult.UserId.ToString()));
            //identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    {
                        "userName", context.UserName
                    },
                    {
                    //get user roles of user
                      "userRoles", listRole
                    },
                    {
                        "userId", loginResult.UserId.ToString()
                    },{
                        "userGuid", loginResult.UserGuid.ToString()
                    },
                     {
                        "fullName", loginResult.FullName
                    },  {
                        "permission", listPermission
                    },
                     {
                        "as:r_t_id", token.TokenGuid
                    }
                });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }
    }
}