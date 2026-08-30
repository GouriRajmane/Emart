using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMart.Models
{
    public class Categories
    {
        public int? CategoryId { get; set; }

        [Required(ErrorMessage ="Enter category Name")]
        [StringLength(100,ErrorMessage ="Can't exceed more than 100 charcters")]
        [DisplayName("Category Name")]
        public string? CategoryName { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
