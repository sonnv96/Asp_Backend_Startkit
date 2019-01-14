using Nois.Framework.Data;
using System;
using System.Data.Entity.ModelConfiguration;

namespace Nois.Core.Domain.Authentication
{
    public class RefreshToken : BaseEntity
    {
        /// <summary>
        /// Get or set token guid
        /// </summary>
        public string TokenGuid { get; set; }
        /// <summary>
        /// Get or set subject
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Get or set client identity
        /// </summary>
        public int ClientId { get; set; }
        /// <summary>
        /// Get or set Client object
        /// </summary>
        public virtual Client Client { get; set; }
        /// <summary>
        /// Get or set issue utc time
        /// </summary>
        public DateTime IssuedUtc { get; set; }
        /// <summary>
        /// Get or set expires utc time
        /// </summary>
        public DateTime ExpiresUtc { get; set; }
        /// <summary>
        /// Get or set protected ticket
        /// </summary>
        public string ProtectedTicket { get; set; }
    }

    public class RefreshTokenMap : EntityTypeConfiguration<RefreshToken>
    {
        public RefreshTokenMap()
        {
            this.ToTable("RefreshToken");
            this.HasKey(rt => rt.Id);

            this.HasRequired(rt => rt.Client)
                .WithMany()
                .HasForeignKey(rt => rt.ClientId);
        }
    }
}
