using System;
using System.Web;
using System.Linq;
using AutoMapper;
using FluentValidation;
using FluentValidation.Attributes;
using System.Collections.Generic;
using Nois.WebApi.Framework;
using Nois.Core.Domain;

namespace Nois.Api.Models.Categories
{

    #region Models

    /// <summary>
    /// Category item model
    /// </summary>
    public class CategoryModel
    {
        /// <summary>
        /// Category identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// CategoryModel Name
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Category list model
    /// </summary>
    public class CategoryListModel : ApiJsonResult
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public CategoryListModel()
        {
            CategoryList = new List<CategoryModel>();
            Total = 0;
        }
        /// <summary>
        /// List of user roles
        /// </summary>
        public List<CategoryModel> CategoryList { get; set; }
        public int Total { get; set; }
    }

    /// <summary>
    /// Category detail model
    /// </summary>
    public class CategoryDetailModel : ApiJsonResult
    {
        /// <summary>
        /// Category identity
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// CategoryModel Name
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Category edit model
    /// </summary>
    [Validator(typeof(CategoryEditValidator))]
    public class CategoryEditModel
    {
        /// <summary>
        /// Category identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// CategoryModel Name
        /// </summary>
        public string Name { get; set; }
    }

    #endregion

    #region Mappings

    /// <summary>
    /// Implement Category Map
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
            config.CreateMap<Category, CategoryModel>();
            config.CreateMap<Category, CategoryDetailModel>();
            config.CreateMap<Category, CategoryEditModel>();
            config.CreateMap<CategoryEditModel, Category>();
        }
    }

    #endregion

    #region Validators

    /// <summary>
    /// Validate for CategoryEditModel
    /// </summary>
    public class CategoryEditValidator : AbstractValidator<CategoryEditModel>
    {
        /// <summary>
        /// Define validators here
        /// </summary>
        public CategoryEditValidator()
        {
            this.RuleFor(s => s.Name).NotNull().WithMessage("Name is requeried");
        }
    }

    #endregion
}