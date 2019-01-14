using Nois.Framework.Histories;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;

namespace Nois.Core.Domain.Users
{
    /// <summary>
    /// User entity
    /// </summary>
    public partial class User : BaseHistoryEntity
    {
        /// <summary>
        /// User Guid
        /// </summary>
        public Guid UserGuid { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Get or set position
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Gender identity : 1-Male; 2-Female
        /// </summary>
        public int GenderId { get; set; }

        /// <summary>
        /// Gender
        /// </summary>
        public Gender Gender
        {
            get { return (Gender)GenderId; }
            set { GenderId = (int)value; }
        }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Password salt
        /// </summary>
        public string PasswordSalt { get; set; }

        /// <summary>
        /// User is deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// User is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Get or set date of birth
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Get or set latest logged-in
        /// </summary>
        public DateTime LatestLoggedin { get; set; }

        private ICollection<UserRole> _userRoles;

        /// <summary>
        /// User roles
        /// </summary>
        public virtual ICollection<UserRole> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new List<UserRole>()); }
            set { _userRoles = value; }
        }
    }

    /// <summary>
    /// Gender enum
    /// </summary>
    public enum Gender
    {
        Female = 0,
        Male = 1,
        Other = 2,
    }

    /// <summary>
    /// User mapping
    /// </summary>
    public class UserMap : EntityTypeConfiguration<User>
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public UserMap()
        {
            this.ToTable("User");
            this.HasKey(t => t.Id);

            this.Ignore(c => c.Gender);

            this.HasMany(u => u.UserRoles)
                .WithMany(u => u.Users)
                .Map(m => m.ToTable("User_UserRole_Mapping"));

            this.Property(u => u.LatestLoggedin).HasColumnType("datetime2");
            this.Property(u => u.BirthDate).HasColumnType("datetime2");
        }
    }

    public class UserHistoryConfiguration : HistoryConfiguration<User>
    {
        public UserHistoryConfiguration()
        {
            this.IgnoreProperty(u => u.Password);
            this.IgnoreProperty(u => u.PasswordSalt);
            this.IgnoreProperty(u => u.GenderId);
            this.IgnoreProperty(u => u.LatestLoggedin);
            this.IgnoreProperty(u => u.Id);

            this.HistoryFor(u => u.Gender).ValueFor(u => u.Gender.ToString());
            this.HistoryFor(u => u.IsActive).DisplayFor("Is Active");

            this.HistoryFor(u => u.UserRoles).DisplayFor("User roles")
                .ValueFor(u => String.Join(", ", u.UserRoles.Select(ur => ur.Name).OrderBy(n => n)))
                .CompareFor((u, v) => String.Join(", ", u.UserRoles.Select(ur => ur.Name).OrderBy(n => n)) == v);
        }
    }
}