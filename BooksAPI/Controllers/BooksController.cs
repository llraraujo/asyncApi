using AutoMapper;
using BooksAPI.DbContexts;
using BooksAPI.Entities;
using BooksAPI.Filters;
using BooksAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BooksAPI.Services
{
    [Route("api")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository; 
            _mapper = mapper;
        }


        [HttpGet("books")]
        [TypeFilter(typeof(BooksResultFilter))]
        public async Task<IActionResult> GetBooks()
        {
            IEnumerable<Book> books = await _bookRepository.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("books/{id}", Name ="GetBook")]
        [TypeFilter(typeof(BookResultFilter))]
        public async Task<IActionResult> GetBook(Guid id)
        {
            Book? book = await _bookRepository.GetBookAsync(id);
            if(book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [HttpPost("books")]
        [TypeFilter(typeof(BookResultFilter))]
        public async Task<IActionResult> CreateBook(
            [FromBody] BookForCreationDto bookForCreation)
        {
            var bookEntity = _mapper.Map<Book>(bookForCreation);
            _bookRepository.AddBook(bookEntity);

            await _bookRepository.SaveChangesAsync();

            await _bookRepository.GetBookAsync(bookEntity.Id);

            return CreatedAtRoute("GetBook", new { id = bookEntity.Id}, bookEntity);
        }
    }
}
