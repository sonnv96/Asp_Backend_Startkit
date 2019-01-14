using Nois.Framework.Data;
using System.Data.Entity.ModelConfiguration;

namespace Nois.Core.Domain
{
    /// <summary>
    /// Product entity
    /// </summary>
    public class Product : BaseEntity
    {
        /// <summary>
        /// Name of product
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Product mapping
    /// </summary>
    public class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            this.ToTable("Product");
            this.HasKey(c => c.Id);
        }
    }
}
