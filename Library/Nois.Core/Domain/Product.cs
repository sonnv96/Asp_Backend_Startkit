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

        /// <summary>
        /// CategoryID of product
        /// </summary>
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
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

            this.HasRequired(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .WillCascadeOnDelete(true);
        }
    }
}
