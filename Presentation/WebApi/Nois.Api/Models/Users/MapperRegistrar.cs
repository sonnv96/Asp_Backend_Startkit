using AutoMapper;
using Nois.Core.Domain.Users;
using Nois.WebApi.Framework;

namespace Nois.Api.Models.Users
{
    /// <summary>
    /// Implement Users Map
    /// </summary>
    public class MapperRegistrar : IMapperRegistrar
    {
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public void Register(IMapperConfigurationExpression config)
        {
            config.CreateMap<Permission, PermissionModel>();
            config.CreateMap<PermissionModel, Permission>();
            config.CreateMap<Permission, EditPermissionModel>();
        }
    }
}