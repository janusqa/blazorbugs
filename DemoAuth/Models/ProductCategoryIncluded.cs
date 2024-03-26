
using System.ComponentModel.DataAnnotations;

namespace DemoAuth.Models
{
    public class ProductCategoryIncluded : Product
    {
        [Required]
        public string? CategoryName { get; set; }
    }
}