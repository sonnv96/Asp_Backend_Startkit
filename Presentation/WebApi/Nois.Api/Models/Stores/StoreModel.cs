using System;
using System.Web;
using System.Linq;
using AutoMapper;
using FluentValidation;
using FluentValidation.Attributes;
using System.Collections.Generic;
using Nois.WebApi.Framework;
using Nois.Core.Domain;

namespace Nois.Api.Models.Stores
{

    #region Models

    /// <summary>
    /// Store item model
    /// </summary>
    public class StoreModel
    {
        /// <summary>
        /// Store identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// StoreModel Name
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Store list model
    /// </summary>
    public class StoreListModel : ApiJsonResult
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public StoreListModel()
        {
            StoreList = new List<StoreModel>();
            Total = 0;
        }
        /// <summary>
        /// List of user roles
        /// </summary>
        public List<StoreModel> StoreList { get; set; }
        public int Total { get; set; }
    }

    /// <summary>
    /// Store detail model
    /// </summary>
    public class StoreDetailModel : ApiJsonResult
    {
        /// <summary>
        /// Store identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// StoreModel Name
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Store edit model
    /// </summary>
    [Validator(typeof(StoreEditValidator))]
    public class StoreEditModel
    {
        /// <summary>
        /// Store identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// StoreModel Name
        /// </summary>
        public string Name { get; set; }
    }

    #endregion

    #region Mappings

    /// <summary>
    /// Implement Store Map
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
            config.CreateMap<Store, StoreModel>();
            config.CreateMap<Store, StoreDetailModel>();
            config.CreateMap<Store, StoreEditModel>();
            config.CreateMap<StoreEditModel, Store>();
        }
    }

    #endregion

    #region Validators

    /// <summary>
    /// Validate for StoreEditModel
    /// </summary>
    public class StoreEditValidator : AbstractValidator<StoreEditModel>
    {
        /// <summary>
        /// Define validators here
        /// </summary>
        public StoreEditValidator()
        {
            this.RuleFor(s => s.Name).NotNull().WithMessage("Name is requeried");
        }
    }

    #endregion
}