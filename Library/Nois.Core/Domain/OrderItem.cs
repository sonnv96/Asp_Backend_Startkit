using Nois.Framework.Data;
using System.Data.Entity.ModelConfiguration;

namespace Nois.Core.Domain
{
    /// <summary>
    /// OrderItem entity
    /// </summary>
    public class OrderItem : BaseEntity
    {
        /// <summary>
        /// Name of order item
        /// </summary>
        public string Name { get; set; }

        public virtual Product Product { get; set; }
    }

    /// <summary>
    /// OrderItem mapping
    /// </summary>
    public class OrderItemMap : EntityTypeConfiguration<OrderItem>
    {
        public OrderItemMap()
        {
            this.ToTable("OrderItem");
            this.HasKey(c => c.Id);

            this.HasRequired(x => x.Product)
                .WithOptional();
        }
    }
}
