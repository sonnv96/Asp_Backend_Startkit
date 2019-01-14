using Nois.Framework.Data;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace Nois.Core.Domain.Users
{
    /// <summary>
    /// Permission entity
    /// </summary>
    public partial class Permission : BaseEntity
    {
        private ICollection<UserRole> _userRoles;
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
        /// User roles
        /// </summary>
        public virtual ICollection<UserRole> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new List<UserRole>()); }
            protected set { _userRoles = value; }
        }
        private ICollection<ActionName> _actionName;
        /// <summary>
        /// User roles
        /// </summary>
        public virtual ICollection<ActionName> ActionNames
        {
            get { return _actionName ?? (_actionName = new List<ActionName>()); }
             set { _actionName = value; }
        }
        /// <summary>
        /// Permission order
        /// </summary>
        public int Order { get; set; }
    }

    /// <summary>
    /// Permission mapping
    /// </summary>
    public class PermissionMap : EntityTypeConfiguration<Permission>
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public PermissionMap()
        {
            this.ToTable("Permission");
            this.HasKey(pr => pr.Id);
            this.Property(pr => pr.Name).IsRequired();
            this.Property(pr => pr.SystemName).IsRequired().HasMaxLength(255);
            this.Property(pr => pr.Category).IsRequired().HasMaxLength(255);

            this.HasMany(pr => pr.UserRoles)
                .WithMany(cr => cr.Permissions)
                .Map(m => m.ToTable("Permission_Role_Mapping"));
        }
    }
}