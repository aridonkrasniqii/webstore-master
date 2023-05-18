using System.ComponentModel.DataAnnotations;

namespace WebStore.Models
{
    public class SignInModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
