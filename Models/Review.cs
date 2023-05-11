using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Bookstore.Models
{
    public class Review
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }

        [MaxLength(450)]
        [Display(Name = "User")]
        [Required]
        public string? AppUser { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        public int Rating { get; set; }
    }
}