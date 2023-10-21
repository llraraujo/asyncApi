using BooksAPI.Entities;
using BooksAPI.Models.External;

namespace BooksAPI.Services
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync(IEnumerable<Guid> bookIds);
        Task<IEnumerable<Book>> GetAllBooksAsync();
        IAsyncEnumerable<Book> GetBookAsAsyncEnumerable();
        Task<Book?> GetBookAsync(Guid id);
        Task<BookCoverDto?> GetBookCoverAsync(string id);
        Task<IEnumerable<BookCoverDto>> GetBookCoversProcessOneByOneAsync(Guid bookId, CancellationToken cancellationToken);
        Task<IEnumerable<BookCoverDto>> GetBookCoversProcessAfterWaitForAllAsync(Guid bookId);
        void AddBook(Book bookToAdd);
        Task<bool> SaveChangesAsync();        
    }
}
