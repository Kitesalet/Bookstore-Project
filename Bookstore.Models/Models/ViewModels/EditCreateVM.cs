using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.Models.ViewModels
{
    public class EditCreateVM
    {

        public int Id { get; set; }

        [StringLength(maximumLength:20, MinimumLength = 3)]
        [Required]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        [Range(1,100, ErrorMessage = "{0} must be between 1-100")]
        [Required]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }

    }
}
