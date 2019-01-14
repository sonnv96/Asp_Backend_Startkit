using Autofac;
using Nois.Services.Authentication;
using Nois.Services.Users;
using Nois.Services.Histories;
using Nois.WebApi.Framework;
using Nois.Framework.Histories;
using Nois.Services.Data;

namespace Nois.Api
{
    /// <summary>
    /// Test module service registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Order loading
        /// </summary>
        public int Order
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Register services here
        /// </summary>
        /// <param name="builder"></param>
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<ClientService>().As<IClientService>().InstancePerLifetimeScope();
            builder.RegisterType<RefreshTokenService>().As<IRefreshTokenService>().InstancePerLifetimeScope();

            // identity
            builder.RegisterType<PermissionService>().As<IPermissionService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<UserRoleService>().As<IUserRoleService>().InstancePerLifetimeScope();
            builder.RegisterType<UserRegistrationService>().As<IUserRegistrationService>().InstancePerLifetimeScope();
            builder.RegisterType<ActionNameService>().As<IActionNameService>().InstancePerLifetimeScope();
            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerLifetimeScope();

            //history
            builder.RegisterType<CustomHistoryService>().As<IHistoryService>().InstancePerLifetimeScope();
        }
    }
}