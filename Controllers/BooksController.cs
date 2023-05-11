using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bookstore.Data;
using Bookstore.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Bookstore.ViewModels;
using static System.Reflection.Metadata.BlobBuilder;
using Azure;
using Microsoft.Extensions.Hosting;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NuGet.DependencyResolver;
using System.Collections.ObjectModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Bookstore.Controllers
{
    public class BooksController : Controller
    {
        private readonly bookstoreContext _context;
        readonly IBufferedFileUploadService _bufferedFileUploadService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private BookViewModel model;

        public BooksController(bookstoreContext context, IBufferedFileUploadService bufferedFileUploadService, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _bufferedFileUploadService = bufferedFileUploadService;
            webHostEnvironment = hostEnvironment;
        }

        // GET: Books
        public async Task<IActionResult> Index(string searchString, string searchGenre)
        {

            var bookstoreContext = _context.Book.Include(b => b.Author).Include(b => b.Reviews).Include(b => b.Genres).ThenInclude(b => b.Genre);
            var zanroviContext = _context.BookGenre.Include(b => b.Genre).Include(b => b.Book);
            var books = from m in bookstoreContext
                         select m;
            var zanrovis = from n in zanroviContext select n;

            if (!String.IsNullOrEmpty(searchString))
            {
              books = books.Where(s => s.Title!.Contains(searchString));
  
            }

            if (!String.IsNullOrEmpty(searchGenre))
            {
                zanrovis = zanrovis.Where(s => s.Genre.GenreName!.Contains(searchGenre));
                var innerJoinQuery =
                 from m in books
                 join n in zanrovis on m.Id equals n.BookId select new { m };
                ArrayList lista = new ArrayList();

                 foreach (var ownerAndPet in innerJoinQuery)
                {
 
                    lista.Add(ownerAndPet.m);
                }

                ViewBag.lista = lista;
            }
           
            return View(await books.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author).Include(b => b.Genres).ThenInclude(b => b.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        
        public async Task<IActionResult> CreateAsync(IFormFile file, [Bind("Id,Title,YearPublished,NumPages,Description,Publisher,FrontPage,DownloadUrl,AuthorId,Genres")] Book book)
        {

            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FirstName", book.AuthorId);
            //ViewBag.GenreList = _context.Set<Genre>();
            ViewData["Genres"] = new MultiSelectList(_context.Set<Genre>(), "Id", "GenreName");
            //product.Categories = new MultiSelectList(list, "ID", "Name", cat.CategorySelected.Select(c => c.ID).ToArray());

            return View();
   
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create([Bind("Id,Title,YearPublished,NumPages,Description,Publisher,FrontPage,DownloadUrl,AuthorId,Genres")] Book book, int[] genres, IFormCollection col)
        {

            if (ModelState.IsValid)
            {
                _context.Add(book);
               // FrontPage = uniqueFileName;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
           

            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FirstName", book.AuthorId);
            ViewData["Genres"] = new MultiSelectList(_context.Set<Genre>(), "Id", "GenreName", book.Genres);
            return View(book);

        }
        
        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }
            var book = _context.Book.Where(m => m.Id == id).Include(m => m.Genres).First();

            if (book == null)
            {
                return NotFound();
            }

            var genres = _context.Genre.AsEnumerable();
            genres = genres.OrderBy(s => s.GenreName);

            BookGenresEditViewModel viewmodel = new BookGenresEditViewModel
            {
                Book = book,
                GenreList = new MultiSelectList(genres, "Id", "GenreName"),
                SelectedGenres = book.Genres.Select(sa => sa.GenreId)
            };


            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FirstName", book.AuthorId);
            return View(viewmodel);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookGenresEditViewModel viewmodel)
        {
            
          if(id != viewmodel.Book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.Book);
                    await _context.SaveChangesAsync();

                    IEnumerable<int> newGenreList = viewmodel.SelectedGenres;
                    IEnumerable<int> prevGenreList = _context.BookGenre.Where(s => s.BookId == id).Select(s => s.GenreId);
                    IQueryable<BookGenre> toBeRemoved = _context.BookGenre.Where(s => s.BookId == id);
                    if (newGenreList != null)
                    {
                        toBeRemoved = toBeRemoved.Where(s => !newGenreList.Contains(s.GenreId));
                        foreach (int genreId in newGenreList)
                        {
                            if (!prevGenreList.Any(s => s == genreId))
                            {
                                _context.BookGenre.Add(new BookGenre { GenreId = genreId, BookId = id });
                            }
                        }
                    }
                    _context.BookGenre.RemoveRange(toBeRemoved);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
  
                   if(!BookExists(viewmodel.Book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FirstName", viewmodel.Book.AuthorId);

            return View(viewmodel);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'bookstoreContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
