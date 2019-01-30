using System;
using System.Web;
using System.Linq;
using AutoMapper;
using FluentValidation;
using FluentValidation.Attributes;
using System.Collections.Generic;
using Nois.WebApi.Framework;
using Nois.Core.Domain;

namespace Nois.Api.Models.OrderItems
{

    #region Models

    /// <summary>
    /// OrderItem item model
    /// </summary>
    public class OrderItemModel
    {
        /// <summary>
        /// OrderItem identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// OrderItem name
        /// </summary>
        public int Name { get; set; }
        /// <summary>
        /// OrderItem Quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// OrderItem Price
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// OrderItem Discount
        /// </summary>
        public int Discount { get; set; }
    }

    /// <summary>
    /// OrderItem list model
    /// </summary>
    public class OrderItemListModel : ApiJsonResult
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public OrderItemListModel()
        {
            OrderItemList = new List<OrderItemModel>();
        }
        public int Total { get; set; }
        /// <summary>
        /// List of user roles
        /// </summary>
        public List<OrderItemModel> OrderItemList { get; set; }
    }

    /// <summary>
    /// OrderItem detail model
    /// </summary>
    public class OrderItemDetailModel : ApiJsonResult
    {
        /// <summary>
        /// OrderItem identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// OrderItem name
        /// </summary>
        public int Name { get; set; }

        /// <summary>
        /// OrderItem Quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// OrderItem Price
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// OrderItem Discount
        /// </summary>
        public int Discount { get; set; }
    }

    /// <summary>
    /// OrderItem edit model
    /// </summary>
    [Validator(typeof(OrderItemEditValidator))]
    public class OrderItemEditModel
    {
        /// <summary>
        /// OrderItem identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// OrderItem name
        /// </summary>
        public int Name { get; set; }

        /// <summary>
        /// OrderItem Quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// OrderItem Price
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// OrderItem Discount
        /// </summary>
        public int Discount { get; set; }
    }

    #endregion

    #region Mappings

    /// <summary>
    /// Implement OrderItem Map
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
            config.CreateMap<OrderItem, OrderItemModel>();
            config.CreateMap<OrderItem, OrderItemDetailModel>();
            config.CreateMap<OrderItem, OrderItemEditModel>();
            config.CreateMap<OrderItemEditModel, OrderItem>();
        }
    }

    #endregion

    #region Validators

    /// <summary>
    /// Validate for OrderItemEditModel
    /// </summary>
    public class OrderItemEditValidator : AbstractValidator<OrderItemEditModel>
    {
        /// <summary>
        /// Define validators here
        /// </summary>
        public OrderItemEditValidator()
        {

        }
    }

    #endregion
}