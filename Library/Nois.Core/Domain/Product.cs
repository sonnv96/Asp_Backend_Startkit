using Nois.Framework.Data;
using System;
using System.Collections.Generic;
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
        /// DatePurchase of product
        /// </summary>
        public DateTime DatePurchase { get; set; }
        /// <summary>
        /// ProductModel Quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// ProductModel Price
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// ProductModel Discount
        /// </summary>
        public int Discount { get; set; }

        /// <summary>
        /// CategoryID of product
        /// </summary>
        public int CategoryId { get; set; }
        public int StoreId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
        public virtual OrderItem OrderItem { get; set; }
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

            this.HasMany(x => x.Stores)
                .WithMany()
                 .Map(cs =>
                 {
                     cs.MapLeftKey("ProductId");
                     cs.MapRightKey("StoreId");
                     cs.ToTable("ProductStore");
                 });
            this.HasOptional(x => x.OrderItem)
                .WithRequired();
            
        }
    }
}
