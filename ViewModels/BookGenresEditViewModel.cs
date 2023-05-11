using Microsoft.AspNetCore.Mvc.Rendering;
using Bookstore.Models;

namespace Bookstore.ViewModels
{
    public class BookGenresEditViewModel
    {
        public Book Book { get; set; }
        public IEnumerable<int>? SelectedGenres { get; set; }
        public IEnumerable<SelectListItem>? GenreList { get; set; }
    }
}
