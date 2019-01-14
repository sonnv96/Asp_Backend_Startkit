using System.Collections.Generic;
using Nois.WebApi.Framework;
using FluentValidation.Attributes;
using FluentValidation;
using AutoMapper;
using Nois.Core.Domain.Users;

namespace Nois.Api.Models.Users
{
    #region Models
    /// <summary>
    /// user role list model
    /// </summary>
    public class UserRoleListModel : ApiJsonResult
    {
        public UserRoleListModel()
        {
            UserRoles = new List<UserRoleModel>();
            Total = 0;
        }
        /// <summary>
        /// List of user roles
        /// </summary>
        public List<UserRoleModel> UserRoles { get; set; }
        /// <summary>
        /// get or set total
        /// </summary>
        public int Total { get; set; }
    }
    /// <summary>
    /// User role object model
    /// </summary>
    public class UserRoleModel
    {
        /// <summary>
        /// User role identify
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// System name
        /// </summary>
        public string SystemName { get; set; }
        /// <summary>
        /// User role name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Is Active
        /// </summary>
        public bool IsActive { get; set; }
    }
    /// <summary>
    /// User role object detail model
    /// </summary>
    public class UserRoleDetailModel : ApiJsonResult
    {
        /// <summary>
        /// User role identify
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// System name
        /// </summary>
        public string SystemName { get; set; }
        /// <summary>
        /// User role name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Is Active
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Permissions
        /// </summary>
        public List<EditPermissionModel> Permissions { get; set; }
    }
    /// <summary>
    /// User role object edit model
    /// </summary>
    [Validator(typeof(UserRoleEditValidator))]
    public class UserRoleEditModel
    {
        /// <summary>
        /// User role identify
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// System name
        /// </summary>
        public string SystemName { get; set; }
        /// <summary>
        /// User role name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Is Active
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Permissions
        /// </summary>
        public List<EditPermissionModel> Permissions { get; set; }
    }
    #endregion
    #region Mappings
    /// <summary>
    /// Implement UserRole Map
    /// </summary>
    public class UserRoleMapperRegistrar : IMapperRegistrar
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
            config.CreateMap<UserRole, UserRoleModel>();
            config.CreateMap<UserRole, UserRoleDetailModel>();
            config.CreateMap<UserRole, UserRoleEditModel>();
            config.CreateMap<UserRoleEditModel, UserRole>().ForMember(dest => dest.Permissions,
               opts => opts.Ignore());
        }
    }
    #endregion
    #region Validators

    /// <summary>
    /// Validate for UserRoleEditModel
    /// </summary>
    public class UserRoleEditValidator : AbstractValidator<UserRoleEditModel>
    {
        /// <summary>
        /// Define validators here
        /// </summary>
        public UserRoleEditValidator()
        {
            this.RuleFor(s => s.Name).NotEmpty().WithMessage("UserRole.NameRequired");
            //this.RuleFor(s => s.SystemName).NotEmpty().WithMessage("UserRole.SystemNameRequired");
            this.RuleFor(s => s.SystemName).Matches(@"^[a-zA-Z]*$").WithMessage("UserRole.SystemNameNotFormat");
        }
    }

    #endregion

}