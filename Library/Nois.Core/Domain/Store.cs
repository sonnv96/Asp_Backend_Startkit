using Nois.Framework.Data;
using System.Data.Entity.ModelConfiguration;

namespace Nois.Core.Domain
{
    /// <summary>
    /// Store entity
    /// </summary>
    public class Store : BaseEntity
    {
        /// <summary>
        /// Name of store
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Store mapping
    /// </summary>
    public class StoreMap : EntityTypeConfiguration<Store>
    {
        public StoreMap()
        {
            this.ToTable("Store");
            this.HasKey(c => c.Id);
        }
    }
}
