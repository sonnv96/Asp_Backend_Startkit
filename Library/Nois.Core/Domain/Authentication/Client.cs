using Nois.Framework.Data;
using System.Data.Entity.ModelConfiguration;

namespace Nois.Core.Domain.Authentication
{
    /// <summary>
    /// Client entity
    /// </summary>
    public class Client : BaseEntity
    {
        /// <summary>
        /// Get or set client guid
        /// </summary>
        public string ClientGuid { get; set; }
        /// <summary>
        /// Get or set Secret
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// Get or set Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Get or set Application Type identity
        /// </summary>
        public int ApplicationTypeId { get; set; }
        /// <summary>
        /// Get or set ApplicationType
        /// </summary>
        public ApplicationTypes ApplicationType { get; set; }
        /// <summary>
        /// Get or set Active
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Get or set Refresh token life time
        /// </summary>
        public int RefreshTokenLifeTime { get; set; }
        /// <summary>
        /// Get or set allower origin
        /// </summary>
        public string AllowedOrigin { get; set; }
    }
    public enum ApplicationTypes
    {
        JavaScript = 0,
        NativeConfidential = 1
    };

    public class ClientMap : EntityTypeConfiguration<Client>
    {
        public ClientMap()
        {
            this.ToTable("Client");
            this.HasKey(c => c.Id);
            this.Ignore(c => c.ApplicationType);
        }
    }
}
