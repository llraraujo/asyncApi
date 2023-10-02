using BooksAPI.Entities;

namespace BooksAPI.Services
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync(IEnumerable<Guid> bookIds);
        Task<IEnumerable<Book>> GetAllBooksAsync();
        IAsyncEnumerable<Book> GetBookAsAsyncEnumerable();
        Task<Book?> GetBookAsync(Guid id);
        void AddBook(Book bookToAdd);
        Task<bool> SaveChangesAsync();        
    }
}
