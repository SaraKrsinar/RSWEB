using Microsoft.AspNetCore.Mvc.Rendering;
using Bookstore.Models;

namespace Bookstore.ViewModels
{
    public class BookGenresCreateViewModel
    {

        public IEnumerable<int>? SelectedGenreIds { get; set; }
        public IEnumerable<SelectListItem>? GenreList { get; set; }
    }
}
