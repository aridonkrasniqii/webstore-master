using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebStore.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
       
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
