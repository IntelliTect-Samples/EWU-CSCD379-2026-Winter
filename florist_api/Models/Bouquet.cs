using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace florist_api.Models
{
    public class Bouquet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "A bouquet name is required")]
        [StringLength(100)]
        public required string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 10000.00)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public required string ImageUrl { get; set; }

        [Required(ErrorMessage = "Every bouquet must belong to a season.")]
        [RegularExpression("^(Spring|Summer|Autumn|Winter)$", 
         ErrorMessage = "Season must be Spring, Summer, Autumn, or Winter")]
        public required string Season { get; set; }

        public bool IsAvailable { get; set; } = true;
        
        [Range(0, 10000)]
        public int InventoryCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}