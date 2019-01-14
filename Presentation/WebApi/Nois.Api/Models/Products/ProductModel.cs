using System;
using System.Web;
using System.Linq;
using AutoMapper;
using FluentValidation;
using FluentValidation.Attributes;
using System.Collections.Generic;
using Nois.WebApi.Framework;
using Nois.Core.Domain;

namespace Nois.Api.Models.Products
{

    #region Models

    /// <summary>
    /// Product item model
    /// </summary>
    public class ProductModel
    {
        /// <summary>
        /// Product identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ProductModel Name
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Product list model
    /// </summary>
    public class ProductListModel : ApiJsonResult
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public ProductListModel()
        {
            ProductList = new List<ProductModel>();
            Total = 0;
        }
        public int Total { get; set; }
        /// <summary>
        /// List of user roles
        /// </summary>
        public List<ProductModel> ProductList { get; set; }
    }

    /// <summary>
    /// Product detail model
    /// </summary>
    public class ProductDetailModel : ApiJsonResult
    {
        /// <summary>
        /// Product identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ProductModel Name
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Product edit model
    /// </summary>
    [Validator(typeof(ProductEditValidator))]
    public class ProductEditModel
    {
        /// <summary>
        /// Product identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ProductModel Name
        /// </summary>
        public string Name { get; set; }
    }

    #endregion

    #region Mappings

    /// <summary>
    /// Implement Product Map
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
            config.CreateMap<Product, ProductModel>();
            config.CreateMap<Product, ProductDetailModel>();
            config.CreateMap<Product, ProductEditModel>();
            config.CreateMap<ProductEditModel, Product>();
        }
    }

    #endregion

    #region Validators

    /// <summary>
    /// Validate for ProductEditModel
    /// </summary>
    public class ProductEditValidator : AbstractValidator<ProductEditModel>
    {
        /// <summary>
        /// Define validators here
        /// </summary>
        public ProductEditValidator()
        {
            this.RuleFor(s => s.Name).NotNull().WithMessage("Name is requeried");
        }
    }

    #endregion
}