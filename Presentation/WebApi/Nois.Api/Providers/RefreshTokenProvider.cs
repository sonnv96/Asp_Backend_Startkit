using Microsoft.Owin.Security.Infrastructure;
using Nois.Services.Authentication;
using Nois.Framework.Infrastructure;
using Nois.WebApi.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nois.Api.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        public void Create(AuthenticationTokenCreateContext context)
        {
            CreateAsync(context).Wait();
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var refreshTokenId = context.Ticket.Properties.Dictionary["as:r_t_id"];
            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

            if (string.IsNullOrEmpty(refreshTokenId))
            {
                return;
            }
            var refreshTokenService = EngineContext.Instance.Resolve<IRefreshTokenService>();
            var token = refreshTokenService.GetByGuid(refreshTokenId);
       
            token.IssuedUtc = DateTime.UtcNow;
            token.ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime));

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            token.ProtectedTicket = context.SerializeTicket();

            await refreshTokenService.UpdateAsync(token);
            context.SetToken(token.TokenGuid);
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            ReceiveAsync(context).Wait();
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            string hashedTokenId = context.Token;

            var refreshTokenService = EngineContext.Instance.Resolve<IRefreshTokenService>();
            var refreshToken = await refreshTokenService.GetByGuidAsync(hashedTokenId);

            if (refreshToken != null)
            {
                //Get protectedTicket from refreshToken class
                context.DeserializeTicket(refreshToken.ProtectedTicket);
                //await refreshTokenService.DeleteAsync(refreshToken);
            }
        }
    }
}