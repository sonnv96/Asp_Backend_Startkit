using Nois.Framework.Histories;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;

namespace Nois.Core.Domain.Users
{
    /// <summary>
    /// User role entity
    /// </summary>
    public partial class UserRole : BaseHistoryEntity
    {
        /// <summary>
        /// System name
        /// </summary>
        public string SystemName { get; set; }
        /// <summary>
        /// User name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Is Active
        /// </summary>
        public bool IsActive { get; set; }
        private ICollection<Permission> _permissions;
        /// <summary>
        /// Permissions
        /// </summary>
        public virtual ICollection<Permission> Permissions
        {
            get { return _permissions ?? (_permissions = new List<Permission>()); }
             set { _permissions = value; }
        }
        private ICollection<User> _users;
        /// <summary>
        /// User
        /// </summary>
        public virtual ICollection<User> Users
        {
            get { return _users ?? (_users = new List<User>()); }
            protected set { _users = value; }
        }
    }

    /// <summary>
    /// User role mapping
    /// </summary>
    public class UserRoleMap : EntityTypeConfiguration<UserRole>
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public UserRoleMap()
        {
            this.ToTable("UserRole");

            this.HasKey(t => t.Id);
        }
    }

    public class UserRoleHistoryConfiguration : HistoryConfiguration<UserRole>
    {
        public UserRoleHistoryConfiguration()
        {
            this.IgnoreProperty(ur => ur.Users);
            this.HistoryFor(ur => ur.Permissions).ValueFor(u => string.Join(", ", u.Permissions.Select(p => p.Name)));
        }
    }
}