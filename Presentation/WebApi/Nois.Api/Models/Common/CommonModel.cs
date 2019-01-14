using AutoMapper;
using Nois.WebApi.Framework;
using System.Collections.Generic;

namespace Nois.Api.Models.Common

{
    /// <summary>
    /// common model
    /// </summary>
    public class CommonModel
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// name
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// common model listing
    /// </summary>
    public class CommonListModel : ApiJsonResult
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public CommonListModel()
        {
            List = new List<CommonModel>();
        }
        /// <summary>
        /// List of CommonModel
        /// </summary>
        public List<CommonModel> List { get; set; }
    }
}