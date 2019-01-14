using Nois.Framework.Histories;
using Nois.WebApi.Framework;
using System.Collections.Generic;

namespace Nois.Api.Models.Histories
{
    #region Models
    /// <summary>
    /// History list model
    /// </summary>
    public class HistoryListModel : ApiJsonResult
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public HistoryListModel()
        {
            HistoryList = new List<HistoryModel>();
        }
        /// <summary>
        /// List of user roles
        /// </summary>
        public List<HistoryModel> HistoryList { get; set; }
    }
    #endregion
}