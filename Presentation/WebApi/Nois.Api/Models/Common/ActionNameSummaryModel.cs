using AutoMapper;
using Nois.Core.Domain.Users;
using Nois.WebApi.Framework;
using System.Collections.Generic;

namespace Nois.Api.Models.Common

{
    /// <summary>
    /// action name list model
    /// </summary>
    public class ActionNameListModel : ApiJsonResult
    {
        /// <summary>
        /// ctor
        /// </summary>
        public ActionNameListModel()
        {
            Data = new List<ActionNameSummaryModel>();
        }
        /// <summary>
        /// list action name summary model
        /// </summary>
        public List<ActionNameSummaryModel> Data { get; set; }
        /// <summary>
        /// get or set total
        /// </summary>
        public int Total { get; set; }
    }
    /// <summary>
    /// action name summary model
    /// </summary>
    public class ActionNameSummaryModel
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// fullname is controller name and action name
        /// </summary>
        public string FullName { get; set; }
    }
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
            config.CreateMap<ActionName, ActionNameSummaryModel>()
               .ForMember(p => p.FullName, m => m.MapFrom(a =>a.Controller+"."+a.Name));
        }
    }
}