using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Bookstore.Models
{
    public class UserBooks
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "User")]
        [MaxLength(450)]
        [Required]
        public string? AppUser { get; set; }

        public int BookId { get; set; }

        public Book? Book { get; set; }
    }
}