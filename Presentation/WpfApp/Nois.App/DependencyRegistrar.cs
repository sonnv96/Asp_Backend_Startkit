using Nois.Framework.Caching;
using Nois.Framework.Services;
using Nois.Services.Users;
using Nois.WpfApp.Framework;
using Nois.WpfApp.Framework.Infrastructure;

namespace Nois.App
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<NullCacheManager>().As<ICacheManager>();
            builder.RegisterType<UserRegistrationService>().As<IUserRegistrationService>();
            builder.RegisterType<EncryptionService>().As<IEncryptionService>();
            
        }
    }
}
