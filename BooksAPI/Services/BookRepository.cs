using BooksAPI.DbContexts;
using BooksAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace BooksAPI.Services
{
    public class BookRepository : IBookRepository
    {
        private readonly BooksContext _context;

        public BookRepository(BooksContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddBook(Book bookToAdd)
        {
            if(bookToAdd == null)
            {
                throw new ArgumentNullException(nameof(bookToAdd));
            }

            _context.Add(bookToAdd);
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Author)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync(IEnumerable<Guid> bookIds)
        {
            return await _context.Books
                            .Where(b => bookIds.Contains(b.Id))
                            .Include(b => b.Author)
                            .ToListAsync();
        }

        public async Task<Book?> GetBookAsync(Guid id)
        {
            Book? book = await _context.Books
                .Include(b => b.Author)
                .FirstAsync(b => b.Id == id);

            return book;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }
    }
}
