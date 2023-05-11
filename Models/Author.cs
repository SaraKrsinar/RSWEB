using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Bookstore.Models
{
    public class Author
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        [MaxLength(50)]
        public string? Nationality { get; set; }

        [MaxLength(50)]
        public string? Gender { get; set; }

        public ICollection<Book>? Books { get; set; }
        
         public string FullName//da mi se vrati celosnoto ime na avtorot bidejki mi treba, vo Delete, vo Details
         {
             get { return String.Format("{0} {1}", FirstName, LastName); }
         }

    }
}