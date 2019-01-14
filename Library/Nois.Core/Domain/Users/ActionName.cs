using Nois.Framework.Data;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace Nois.Core.Domain.Users
{
    /// <summary>
    /// ActionName entity
    /// </summary>
    public class ActionName : BaseEntity
    {
        /// <summary>
        /// controller name
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// action name
        /// </summary>
        public string Name { get; set; }
        private ICollection<Permission> _permission;
        /// <summary>
        /// User roles
        /// </summary>
        public virtual ICollection<Permission> Permissions
        {
            get { return _permission ?? (_permission = new List<Permission>()); }
            protected set { _permission = value; }
        }
    }

    /// <summary>
    /// ActionName mapping
    /// </summary>
    public class ActionNameMap : EntityTypeConfiguration<ActionName>
    {
        public ActionNameMap()
        {
            this.ToTable("ActionName");
            this.HasKey(c => c.Id);
        }
    }
}