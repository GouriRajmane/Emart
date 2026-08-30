using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EMart.Models
{
    public class Brands
    {
        public int? BrandId { get; set; }

        [Required(ErrorMessage = "Brand Name is required.")]
        [DisplayName("Brand Name")]
        [StringLength(200, ErrorMessage = "Brand Name cannot exceed 200 characters.")]
        public string? BrandName { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; } = true;

        [DisplayName("Created On")]
        public DateTime? CreatedOn { get; set; }

        [DisplayName("Updated On")]
        public DateTime? UpdatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public string? LogoPath { get; set; }
        public IFormFile? LogoFile { get; set; }


    }
}
