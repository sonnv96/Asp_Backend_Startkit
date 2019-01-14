using FluentValidation;
using FluentValidation.Attributes;
using System.Collections.Generic;
using Nois.WebApi.Framework;
using AutoMapper;
using Nois.Framework.Localization;

namespace Nois.Api.Models.LocalStringResources
{

    #region Models

    /// <summary>
    /// LocaleStringResource item model
    /// </summary>
    public class LocaleStringResourceModel
    {
        /// <summary>
        /// LocaleStringResource identity
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ResourceName
        /// </summary>
        public string ResourceName { get; set; }
        /// <summary>
        /// ResourceValue
        /// </summary>
        public string ResourceValue { get; set; }
    }

    /// <summary>
    /// LocalStringResource list model
    /// </summary>
    public class LocaleStringResourceListModel : ApiJsonResult
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public LocaleStringResourceListModel()
        {
            LocalStringResourceList = new List<LocaleStringResourceModel>();
        }
        /// <summary>
        /// List of user roles
        /// </summary>
        public List<LocaleStringResourceModel> LocalStringResourceList { get; set; }
        /// <summary>
        /// get or set total
        /// </summary>
        public int Total { get; set; }
    }

    /// <summary>
    /// LocaleStringResource detail model
    /// </summary>
    public class LocaleStringResourceDetailModel : ApiJsonResult
    {
        /// <summary>
        /// LocaleStringResource identity
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ResourceName
        /// </summary>
        public string ResourceName { get; set; }
        /// <summary>
        /// ResourceValue
        /// </summary>
        public string ResourceValue { get; set; }
        /// <summary>
        /// localization name
        /// </summary>
        public int LanguageId { get; set; }
    }

    /// <summary>
    /// LocaleStringResource edit model
    /// </summary>
    [Validator(typeof(LocaleStringResourceEditValidator))]
    public class LocaleStringResourceEditModel
    {
        /// <summary>
        /// LocaleStringResource identity
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ResourceName
        /// </summary>
        public string ResourceName { get; set; }
        /// <summary>
        /// ResourceValue
        /// </summary>
        public string ResourceValue { get; set; }
        /// <summary>
        /// Language id
        /// </summary>
        public int LanguageId { get; set; }
    }

    #endregion

    #region Mappings

    /// <summary>
    /// Implement LocaleStringResource Map
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
            config.CreateMap<LocaleStringResource, LocaleStringResourceModel>();
            config.CreateMap<LocaleStringResource, LocaleStringResourceDetailModel>();
            config.CreateMap<LocaleStringResource, LocaleStringResourceEditModel>();
            config.CreateMap<LocaleStringResourceEditModel, LocaleStringResource>();
        }
    }

    #endregion

    #region Validators

    /// <summary>
    /// Validate for LocaleStringResourceEditModel
    /// </summary>
    public class LocaleStringResourceEditValidator : AbstractValidator<LocaleStringResourceEditModel>
    {
        /// <summary>
        /// Define validators here
        /// </summary>
        public LocaleStringResourceEditValidator()
        {
            this.RuleFor(s => s.ResourceName).NotEmpty().WithMessage("LocaleStringResource.NameRequired");
            this.RuleFor(s => s.ResourceValue).NotEmpty().WithMessage("LocaleStringResource.ValueRequired");
        }
    }

    #endregion
}