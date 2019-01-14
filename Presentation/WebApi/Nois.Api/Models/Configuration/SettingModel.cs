using FluentValidation;
using FluentValidation.Attributes;
using System.Collections.Generic;
using Nois.WebApi.Framework;
using AutoMapper;
using Nois.Framework.Settings;

namespace Nois.Api.Models.Configuration
{

    #region Models

    /// <summary>
    /// Setting item model
    /// </summary>
    public class SettingModel
    {
        /// <summary>
        /// Setting identity
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Setting name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Setting value
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Setting list model
    /// </summary>
    public class SettingListModel : ApiJsonResult
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public SettingListModel()
        {
            SettingList = new List<SettingModel>();
        }
        /// <summary>
        /// List of user roles
        /// </summary>
        public List<SettingModel> SettingList { get; set; }
        /// <summary>
        /// get or set total
        /// </summary>
        public int Total { get; set; }
    }

    /// <summary>
    /// Setting detail model
    /// </summary>
    public class SettingDetailModel : ApiJsonResult
    {
        /// <summary>
        /// Setting identity
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Setting name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Setting value
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Setting edit model
    /// </summary>
    [Validator(typeof(SettingEditValidator))]
    public class SettingEditModel
    {
        /// <summary>
        /// Setting identity
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Setting name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Setting value
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Setting value model
    /// </summary>
    public class SettingValueModel : ApiJsonResult
    {
        /// <summary>
        /// Setting value
        /// </summary>
        public string Value { get; set; }
    }
    #endregion

    #region Mappings

    /// <summary>
    /// Implement Setting Map
    /// </summary>
    public class SettingMapperRegistrar : IMapperRegistrar
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
            config.CreateMap<Setting, SettingModel>();
            config.CreateMap<Setting, SettingDetailModel>();
            config.CreateMap<Setting, SettingEditModel>();
            config.CreateMap<SettingEditModel, Setting>();
        }
    }

    #endregion

    #region Validators

    /// <summary>
    /// Validate for SettingEditModel
    /// </summary>
    public class SettingEditValidator : AbstractValidator<SettingEditModel>
    {
        /// <summary>
        /// Define validators here
        /// </summary>
        public SettingEditValidator()
        {
            RuleFor(s => s.Name).NotNull().WithMessage("Setting name is required");
            RuleFor(s => s.Value).NotNull().WithMessage("Setting value is required");
        }
    }

    #endregion
}