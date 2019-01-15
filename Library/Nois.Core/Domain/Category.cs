using Nois.Framework.Data;
using System.Data.Entity.ModelConfiguration;

namespace Nois.Core.Domain
{
    /// <summary>
    /// Category entity
    /// </summary>
    public class Category : BaseEntity
    {
        /// <summary>
        /// Name of category
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Category mapping
    /// </summary>
    public class CategoryMap : EntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            this.ToTable("Category");
            this.HasKey(c => c.Id);
        }
    }
}
