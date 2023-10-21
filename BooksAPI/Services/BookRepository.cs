using BooksAPI.DbContexts;
using BooksAPI.Entities;
using BooksAPI.Models.External;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BooksAPI.Services
{
    public class BookRepository : IBookRepository
    {
        private readonly BooksContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public BookRepository(BooksContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException( nameof(httpClientFactory));
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

        public IAsyncEnumerable<Book> GetBookAsAsyncEnumerable()
        {
            return _context.Books.AsAsyncEnumerable<Book>();
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

        public async Task<BookCoverDto?> GetBookCoverAsync(string id)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync($"http://localhost:5103/api/bookcovers/{id}");

            if(response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<BookCoverDto>(await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
            }

            return null;
        }

        public async Task<IEnumerable<BookCoverDto>> GetBookCoversProcessOneByOneAsync(Guid bookId,
            CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var bookCovers = new List<BookCoverDto>();

            // lista falsa de bookcovers
            var bookCoversUrls = new[]
            {
                $"http://localhost:5103/api/bookcovers/{bookId}-dummycover1",
                $"http://localhost:5103/api/bookcovers/{bookId}-dummycover2",
                $"http://localhost:5103/api/bookcovers/{bookId}-dummycover3",
                $"http://localhost:5103/api/bookcovers/{bookId}-dummycover4",
                $"http://localhost:5103/api/bookcovers/{bookId}-dummycover5",
            };

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                using(var linkedCancellationTokenSource = 
                            CancellationTokenSource
                            .CreateLinkedTokenSource(cancellationToken, cancellationTokenSource.Token))
                {
                    // ativa a task e processa um por um
                    foreach (var bookCoverUrl in bookCoversUrls)
                    {
                        var response = await httpClient.GetAsync(bookCoverUrl, linkedCancellationTokenSource.Token);
                        if (response.IsSuccessStatusCode)
                        {
                            var bookCover = JsonSerializer.Deserialize<BookCoverDto>(await response.Content.ReadAsStringAsync(linkedCancellationTokenSource.Token),
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                            if (bookCover != null)
                            {
                                bookCovers.Add(bookCover);
                            }
                        }
                        else
                        {
                            cancellationTokenSource.Cancel();
                        }
                    }
                }
                

            }

            return bookCovers;
        }

        public async Task<IEnumerable<BookCoverDto>> GetBookCoversProcessAfterWaitForAllAsync(Guid bookId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var bookCovers = new List<BookCoverDto>();

            // lista falsa de bookcovers
            var bookCoversUrls = new[]
            {
                $"http://localhost:5103/api/bookcovers/{bookId}-dummycover1",
                $"http://localhost:5103/api/bookcovers/{bookId}-dummycover2",
                $"http://localhost:5103/api/bookcovers/{bookId}-dummycover3",
                $"http://localhost:5103/api/bookcovers/{bookId}-dummycover4",
                $"http://localhost:5103/api/bookcovers/{bookId}-dummycover5",
            };

            var bookCoversTasks = new List<Task<HttpResponseMessage>>();
            foreach(var bookCoverUrl in bookCoversUrls)
            {
                bookCoversTasks.Add(httpClient.GetAsync(bookCoverUrl));
            }

            // Espera todas as tasks serem completadas
            var bookCoverTasksResults = await Task.WhenAll(bookCoversTasks);

            foreach(var response in bookCoverTasksResults.Reverse())
            {
                if (response.IsSuccessStatusCode)
                {
                    var bookCover = JsonSerializer.Deserialize<BookCoverDto>(await response.Content.ReadAsStringAsync(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (bookCover != null)
                    {
                        bookCovers.Add(bookCover);
                    }
                }
            }

            return bookCovers;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }
        
    }
}
