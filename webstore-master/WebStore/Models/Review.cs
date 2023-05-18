using System.ComponentModel.DataAnnotations;

namespace WebStore.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string Title { get; set;}
        [Range(1, 5)]
        public int Stars { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
