using System.Collections.Generic;
using System;
using FluentValidation.Attributes;
using FluentValidation;
using Nois.WebApi.Framework;

namespace Nois.Api.Models.Backups
{

    #region Models

    /// <summary>
    /// Backup item model
    /// </summary>
    public class BackupModel
    {
        /// <summary>
        /// Get or set Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Get or set Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Backup identity
        /// </summary>
        public DateTime CreatedOn  { get; set; }
        /// <summary>
        /// Get or set Created by
        /// </summary>
        public string CreatedBy { get; set; }
    }

    /// <summary>
    /// Backup list model
    /// </summary>
    public class BackupListModel : ApiJsonResult
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        public BackupListModel()
        {
            BackupList = new List<BackupModel>();
        }

        /// <summary>
        /// Get or set Total
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// List of user roles
        /// </summary>
        public List<BackupModel> BackupList { get; set; }
    }

    /// <summary>
    /// Product edit model
    /// </summary>
    [Validator(typeof(BackupAddValidator))]
    public class BackupAddModel
    {
        /// <summary>
        /// Get or set Name
        /// </summary>
        public string Name { get; set; }
    }
    #endregion

    #region Mappings

    /// <summary>
    /// Implement Backup Map
    /// </summary>
    //public class MapperRegistrar : IMapperRegistrar
    //{
    //    public int Order
    //    {
    //        get
    //        {
    //            return 1;
    //        }
    //    }

    //    public void Register(IMapperConfigurationExpression config)
    //    {
    //        config.CreateMap<Backup, BackupModel>();
    //        config.CreateMap<Backup, BackupDetailModel>();
    //        config.CreateMap<Backup, BackupEditModel>();
    //        config.CreateMap<BackupEditModel, Backup>();
    //    }
    //}

    #endregion

    #region Validators

    /// <summary>
    /// Validate for BackupAddModel
    /// </summary>
    public class BackupAddValidator : AbstractValidator<BackupAddModel>
    {
        /// <summary>
        /// Define validators here
        /// </summary>
        public BackupAddValidator()
        {
            this.RuleFor(b => b.Name).NotEmpty().WithMessage("Backup.NameRequired");
        }
    }

    #endregion
}