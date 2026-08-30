using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EMart.Models
{
    public class Customers
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "User is required.")]
        [DisplayName("User")]
        public int? UserId { get; set; }
    }
}
