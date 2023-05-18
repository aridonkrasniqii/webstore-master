using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebStore.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int StockCount { get; set; }
        [Required]
        public DateTime ReleaseDate { get; set; }

        public bool Released { get
            {
                return ReleaseDate > DateTime.Now;
            } 
        }
       
        public bool InStock
        {
            get
            {
                return StockCount > 0;
            }
        }
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual List<Review>? Reviews { get; set; }
        public int Cost { get; set; }
    }
}
