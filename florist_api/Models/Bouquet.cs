using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace florist_api.Models
{
    public class Bouquet
    {
        // Primary Key for Azure SQL
        [Key]
        public int Id { get; set; }

        // Ensuring the Name is required and has a length limit for SQL performance
        [Required(ErrorMessage = "A bouquet name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Price precision is vital for Azure SQL to prevent rounding errors
        [Required]
        [Range(0.01, 10000.00)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        // Your specific requirement: Spring, Summer, Autumn, Winter
        [Required]
        [RegularExpression("^(Spring|Summer|Autumn|Winter)$", 
         ErrorMessage = "Season must be Spring, Summer, Autumn, or Winter")]
        public string Season { get; set; } = "Spring";

        public bool IsAvailable { get; set; } = true;

        // Records when the bouquet was added to the garden
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}