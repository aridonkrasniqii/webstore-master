using System.ComponentModel.DataAnnotations;

namespace WebStore.Models
{
    public enum OrderState
    {
        Placed, Processing, Shipped
    }
    public class Order
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// The sAMAccountName of the user who place the order
        /// </summary>
        [Required]
        public string PlacedBy { get; set; }
        [Required]
        public OrderState State { get; set; }

        public virtual ICollection<OrderItem>? OrderItems { get; set; }
    }
}
