
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoAuth.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [NotMapped]
        public Category? Category { get; set; }
    }
}