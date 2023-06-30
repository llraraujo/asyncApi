using BooksAPI.Entities;

namespace BooksAPI.Services
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookAsync(Guid id);
    }
}
