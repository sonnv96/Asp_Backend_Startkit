using Nois.Framework.Data;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace Nois.Core.Domain.Resources
{
    /// <summary>
    /// Resource entity
    /// </summary>
    public class Resource : BaseEntity
    {
        /// <summary>
        /// filename
        /// </summary>
        [MaxLength(250)]
        [Required]
        public string FileName { get; set; }
        /// <summary>
        /// resource url
        /// </summary>
        [Required]
        public string ResourceUrl { get; set; }
        /// <summary>
        /// thumbnailUrl
        /// </summary>
        public string ThumbnailUrl { get; set; }
        /// <summary>
        /// mime type
        /// </summary>
        public string MimeType { get; set; }
    }

    /// <summary>
    /// Resource mapping
    /// </summary>
    public class ResourceMap : EntityTypeConfiguration<Resource>
    {
        public ResourceMap()
        {
            this.ToTable("Resource");
            this.HasKey(c => c.Id);
        }
    }
}
