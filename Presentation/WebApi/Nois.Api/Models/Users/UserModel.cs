using System;
using System.Collections.Generic;
using Nois.WebApi.Framework;
using FluentValidation.Attributes;
using AutoMapper;
using FluentValidation;
using Nois.Core.Domain.Users;

namespace Nois.Api.Models.Users
{
    #region Models

    /// <summary>
    /// user list model
    /// </summary>
    public class UserListModel : ApiJsonResult
    {
        /// <summary>
        /// user list
        /// </summary>
        public UserListModel()
        {
            Users = new List<UserModel>();
            Total = 0;
        }
        /// <summary>
        /// List of user roles
        /// </summary>
        public List<UserModel> Users { get; set; }
        /// <summary>
        /// get or set total
        /// </summary>
        public int Total { get; set; }
    }

    /// <summary>
    /// User object model
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// User identify
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// User Guid
        /// </summary>
        public Guid UserGuid { get; set; }
        /// <summary>
        /// User name
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Position
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Get or set isActive
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Get or set latest logged-in
        /// </summary>
        public DateTime LatestLoggedin { get; set; }
        /// <summary>
        /// User roles
        /// </summary>
        public List<UserRoleModel> UserRoles { get; set; }
    }

    /// <summary>
    /// User profile model
    /// </summary>
    public class UserProfileModel : ApiJsonResult
    {
        /// <summary>
        /// User identify
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// User Guid
        /// </summary>
        public Guid UserGuid { get; set; }
        /// <summary>
        /// User name
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Phone number
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Position
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// Gender identity : 1-Male; 2-Female
        /// </summary>
        public int GenderId { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Get or set isActive
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Date of birth
        /// </summary>
        public DateTime? BirthDate { get; set; }
        /// <summary>
        /// Get or set latest logged-in
        /// </summary>
        public DateTime LatestLoggedin { get; set; }
    }

    /// <summary>
    /// User object detail model
    /// </summary>
    public class UserDetailModel : ApiJsonResult
    {
        /// <summary>
        /// User identify
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// User Guid
        /// </summary>
        public Guid UserGuid { get; set; }
        /// <summary>
        /// User name
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Phone number
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Position
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// Gender identity : 1-Male; 2-Female
        /// </summary>
        public int GenderId { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Get or set isActive
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Date of birth
        /// </summary>
        public DateTime? BirthDate { get; set; }
        /// <summary>
        /// Get or set latest logged-in
        /// </summary>
        public DateTime LatestLoggedin { get; set; }
        /// <summary>
        /// User roles
        /// </summary>
        public List<UserRoleModel> UserRoles { get; set; }
    }

    /// <summary>
    /// User role object edit model
    /// </summary>
    [Validator(typeof(UserAddValidator))]
    public class UserAddModel
    {
        /// <summary>
        /// User name
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Phone number
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Gender identity : 1-Male; 2-Female
        /// </summary>
        public int GenderId { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Position
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// date of birth
        /// </summary>
        public DateTime? BirthDate { get; set; }
        /// <summary>
        /// Get or set IsActive
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// User roles
        /// </summary>
        public List<UserRoleEditModel> UserRoles { get; set; }
    }

    /// <summary>
    /// User role object edit model
    /// </summary>
    [Validator(typeof(UserEditValidator))]
    public class UserEditModel
    {
        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Phone number
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Gender identity : 1-Male; 2-Female
        /// </summary>
        public int GenderId { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Position
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// date of birth
        /// </summary>
        public DateTime? BirthDate { get; set; }
        /// <summary>
        /// Get or set IsActive
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// User roles
        /// </summary>
        public List<UserRoleEditModel> UserRoles { get; set; }
    }

    /// <summary>
    /// User role object edit model
    /// </summary>
    [Validator(typeof(UserProfileEditValidator))]
    public class UserProfileEditModel
    {
        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Phone number
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Gender identity : 1-Male; 2-Female
        /// </summary>
        public int GenderId { get; set; }
        /// <summary>
        /// Position
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// date of birth
        /// </summary>
        public DateTime? BirthDate { get; set; }
    }

    #endregion

    #region Mappings

    /// <summary>
    /// Implement UserRole Map
    /// </summary>
    public class UserMapperRegistrar : IMapperRegistrar
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
            config.CreateMap<User, UserModel>();
            config.CreateMap<User, UserDetailModel>();
            config.CreateMap<User, UserProfileModel>();
            config.CreateMap<User, UserEditModel>();

            config.CreateMap<UserEditModel, User>().ForMember(dest => dest.UserRoles,
               opts => opts.Ignore())
               .ForMember(dest => dest.UserGuid,
               opts => opts.Ignore())
               .ForMember(dest => dest.Password,
               opts => opts.Ignore())
               .ForMember(dest => dest.PasswordSalt,
               opts => opts.Ignore());

            config.CreateMap<UserAddModel, User>().ForMember(dest => dest.UserRoles,
               opts => opts.Ignore())
               .ForMember(dest => dest.UserGuid,
               opts => opts.Ignore())
               .ForMember(dest => dest.Password,
               opts => opts.Ignore())
               .ForMember(dest => dest.PasswordSalt,
               opts => opts.Ignore());

            // mapping between user profile edit model and user entity
            config.CreateMap<UserProfileEditModel, User>().ForMember(dest => dest.UserRoles,
               opts => opts.Ignore())
               .ForMember(dest => dest.UserGuid,
               opts => opts.Ignore())
               .ForMember(dest => dest.Password,
               opts => opts.Ignore())
               .ForMember(dest => dest.PasswordSalt,
               opts => opts.Ignore())
               .ForMember(dest => dest.UserRoles,
               opts => opts.Ignore());
        }
    }

    #endregion

    #region Validators

    /// <summary>
    /// Validate for UserEditModel
    /// </summary>
    public class UserEditValidator : AbstractValidator<UserEditModel>
    {
        /// <summary>
        /// Define validators here
        /// </summary>
        public UserEditValidator()
        {
            this.RuleFor(s => s.Email).EmailAddress().When(s => !string.IsNullOrEmpty(s.Email)).WithMessage("User.EmailNotFormat");
            this.RuleFor(s => s.GenderId).InclusiveBetween(0, 2).WithMessage("User.InvalidGender");
        }
    }

    /// <summary>
    /// Validate for UserEditModel
    /// </summary>
    public class UserProfileEditValidator : AbstractValidator<UserProfileEditModel>
    {
        /// <summary>
        /// Define validators here
        /// </summary>
        public UserProfileEditValidator()
        {
            this.RuleFor(s => s.GenderId).InclusiveBetween(0, 2).WithMessage("User.InvalidGender");
        }
    }

    /// <summary>
    /// Validate for UserAddModel
    /// </summary>
    public class UserAddValidator : AbstractValidator<UserAddModel>
    {
        /// <summary>
        /// Define validators here
        /// </summary>
        public UserAddValidator()
        {
            this.RuleFor(s => s.Username).NotEmpty().WithMessage("User.UsernameRequired")
                .Matches(@"^[a-zA-Z0-9._]*$").WithMessage("User.UsernameNotFormat");
            this.RuleFor(s => s.Email).EmailAddress().When(s => !string.IsNullOrEmpty(s.Email)).WithMessage("User.EmailNotFormat");
            this.RuleFor(s => s.GenderId).InclusiveBetween(0, 2).WithMessage("User.InvalidGender");
        }
    }

    #endregion
}