using System;
using System.Collections.Generic;
using Nois.WebApi.Framework;
using AutoMapper;
using Nois.Framework.Loggings;

namespace Nois.Api.Models.Configuration
{

    #region Models

    /// <summary>
    /// Log item model
    /// </summary>
    public class LogModel
    {
        /// <summary>
        /// Log identity
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Log level
        /// </summary>
        public string LogLevel { get; set; }
        /// <summary>
        /// Short message
        /// </summary>
        public string ShortMessage { get; set; }
        /// <summary>
        /// Created on Utc
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }

    /// <summary>
    /// Log level item model
    /// </summary>
    public class LogLevelModel
    {
        /// <summary>
        /// Log level identity
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Log level name
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Log list model
    /// </summary>
    public class LogListModel : ApiJsonResult
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public LogListModel()
        {
            LogList = new List<LogModel>();
        }
        /// <summary>
        /// List of logs
        /// </summary>
        public List<LogModel> LogList { get; set; }
        /// <summary>
        /// get or set total
        /// </summary>
        public int Total { get; set; }
    }

    /// <summary>
    /// Log list model
    /// </summary>
    public class LogLevelListModel : ApiJsonResult
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public LogLevelListModel()
        {
            LogLevelList = new List<LogLevelModel>();
        }
        /// <summary>
        /// List of log levels
        /// </summary>
        public List<LogLevelModel> LogLevelList { get; set; }
    }

    /// <summary>
    /// Log detail model
    /// </summary>
    public class LogDetailModel : ApiJsonResult
    {
        /// <summary>
        /// Log identity
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Log level
        /// </summary>
        public string LogLevel { get; set; }
        /// <summary>
        /// Short message
        /// </summary>
        public string ShortMessage { get; set; }
        /// <summary>
        /// Full message
        /// </summary>
        public string FullMessage { get; set; }
        /// <summary>
        /// Created on Utc
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }

    #endregion

    #region Mappings

    /// <summary>
    /// Implement Log Map
    /// </summary>
    public class LogMapperRegistrar : IMapperRegistrar
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
            config.CreateMap<Log, LogModel>().ForMember(m=>m.LogLevel,opt=>opt.MapFrom(l=>l.LogLevel.ToString()));
            config.CreateMap<Log, LogDetailModel>().ForMember(m => m.LogLevel, opt => opt.MapFrom(l => l.LogLevel.ToString()));
        }
    }

    #endregion

    #region Validators

    #endregion
}