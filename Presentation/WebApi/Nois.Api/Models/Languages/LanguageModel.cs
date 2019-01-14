using FluentValidation;
using FluentValidation.Attributes;
using System.Collections.Generic;
using Nois.WebApi.Framework;
using AutoMapper;
using Nois.Framework.Localization;

namespace Nois.Api.Models.Languages
{

    #region Models

    /// <summary>
    /// Language item model
    /// </summary>
    public class LanguageModel
    {
        /// <summary>
        /// Language identity
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the language culture
        /// </summary>
        public string LanguageCulture { get; set; }

        /// <summary>
        /// Gets or sets the flag image file name
        /// </summary>
        public string FlagImageFileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the language is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// Language list model
    /// </summary>
    public class LanguageListModel : ApiJsonResult
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public LanguageListModel()
        {
            LanguageList = new List<LanguageModel>();
        }
        /// <summary>
        /// List of user roles
        /// </summary>
        public List<LanguageModel> LanguageList { get; set; }
        /// <summary>
        /// get or set total
        /// </summary>
        public int Total { get; set; }
    }

    /// <summary>
    /// Language detail model
    /// </summary>
    public class LanguageDetailModel : ApiJsonResult
    {
        /// <summary>
        /// Language identity
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the language culture
        /// </summary>
        public string LanguageCulture { get; set; }

        /// <summary>
        /// Gets or sets the flag image file name
        /// </summary>
        public string FlagImageFileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the language is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// Language edit model
    /// </summary>
    [Validator(typeof(LanguageEditValidator))]
    public class LanguageEditModel
    {
        /// <summary>
        /// Language identity
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the language culture
        /// </summary>
        public string LanguageCulture { get; set; }

        /// <summary>
        /// Gets or sets the flag image file name
        /// </summary>
        public string FlagImageFileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the language is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }

    #endregion

    #region Mappings

    /// <summary>
    /// Implement Language Map
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
            config.CreateMap<Language, LanguageModel>();
            config.CreateMap<Language, LanguageDetailModel>();
            config.CreateMap<Language, LanguageEditModel>();
            config.CreateMap<LanguageEditModel, Language>();
        }
    }

    #endregion

    #region Validators

    /// <summary>
    /// Validate for LanguageEditModel
    /// </summary>
    public class LanguageEditValidator : AbstractValidator<LanguageEditModel>
    {
        /// <summary>
        /// Define validators here
        /// </summary>
        public LanguageEditValidator()
        {

        }
    }

    #endregion
}