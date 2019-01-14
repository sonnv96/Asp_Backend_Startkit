using System.Collections.Generic;
using Nois.WebApi.Framework;
using Nois.Api.Models.Common;

namespace Nois.Api.Models.Users
{
    /// <summary>
    /// Permission object model
    /// </summary>
    public class PermissionModel : ApiJsonResult
    {
        /// <summary>
        /// Test identify
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Permission name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Permission system name
        /// </summary>
        public string SystemName { get; set; }
        /// <summary>
        /// description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Permission category
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Permission order
        /// </summary>
        public int Order { get; set; }
    }
    /// <summary>
    /// Permission object model
    /// </summary>
    public class EditPermissionModel
    {
        /// <summary>
        /// Test identify
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Permission name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Permission system name
        /// </summary>
        public string SystemName { get; set; }
        /// <summary>
        /// description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Permission category
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Permission order
        /// </summary>
        public int Order { get; set; }
        public List<ActionNameModel> ActionName { get; set; }
    }

    /// <summary>
    /// Permission list model
    /// </summary>
    public class PermissionListModel : ApiJsonResult
    {
        public PermissionListModel()
        {
            PermissionList = new List<EditPermissionModel>();
        }
        /// <summary>
        /// List of permission
        /// </summary>
        public List<EditPermissionModel> PermissionList { get; set; }
    }
}