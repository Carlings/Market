using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TestWebApplication.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [DisplayName("Display Order")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Must be greater than 0")]
        public int DisplayOrder { get; set; }

    }
}
