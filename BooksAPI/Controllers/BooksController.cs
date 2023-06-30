using BooksAPI.DbContexts;
using BooksAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BooksAPI.Services
{
    [Route("api")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository; 
            
        }


        [HttpGet("Books")]
        public async Task<IActionResult> GetBooks()
        {
            IEnumerable<Book> books = await _bookRepository.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("Books/{id}")]
        public async Task<IActionResult> GetBook(Guid id)
        {
            Book? book = await _bookRepository.GetBookAsync(id);
            if(book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }
    }
}
