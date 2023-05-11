using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bookstore.Models;

namespace Bookstore.Data
{
    public class bookstoreContext : DbContext
    {
        public bookstoreContext(DbContextOptions<bookstoreContext> options)
            : base(options)
        {
        }

        public DbSet<Bookstore.Models.Book> Book { get; set; } = default!;

        public DbSet<Bookstore.Models.Genre>? Genre { get; set; }

        public DbSet<Bookstore.Models.Author>? Author { get; set; }

        public DbSet<Bookstore.Models.Review>? Review { get; set; }

        public DbSet<Bookstore.Models.UserBooks>? UserBooks { get; set; }

        public DbSet<BookGenre> BookGenre { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<BookGenre>()
            .HasOne<Book>(p => p.Book)
            .WithMany(p => p.Genres)
            .HasForeignKey(p => p.BookId);
            builder.Entity<BookGenre>()
            .HasOne<Genre>(p => p.Genre)
            .WithMany(p => p.Books)
            .HasForeignKey(p => p.GenreId);
            builder.Entity<Book>()
            .HasOne<Author>(p => p.Author)
            .WithMany(p => p.Books)
            .HasForeignKey(p => p.AuthorId);
            builder.Entity<Review>().
                HasOne<Book>(p => p.Book).WithMany(p => p.Reviews).HasForeignKey(p => p.BookId);
            builder.Entity<UserBooks>()
                .HasOne<Book>(p => p.Book).WithMany(p => p.Buyers).HasForeignKey(p => p.BookId);

        }
    }
}