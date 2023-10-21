using AutoMapper;
using BooksAPI.DbContexts;
using BooksAPI.Entities;
using BooksAPI.Filters;
using BooksAPI.Models;
using BooksAPI.Models.External;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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

        [HttpGet("booksstream")]
        public async IAsyncEnumerable<BookDto> StreamBooks()
        {
            await foreach(var book in _bookRepository.GetBookAsAsyncEnumerable())
            {
                // adicionando delay para visualizar o efeito
                await Task.Delay(500);
                yield return _mapper.Map<BookDto>(book);
            }
        }

        [HttpGet("books/{id}", Name ="GetBook")]
        [TypeFilter(typeof(BookWithCoversResultFilter))]
        public async Task<IActionResult> GetBook(Guid id, CancellationToken cancellationToken)
        {
            Book? bookEntity = await _bookRepository.GetBookAsync(id);
            if(bookEntity == null)
            {
                return NotFound();
            }
            //var bookCover = await _bookRepository.GetBookCoverAsync("dummycover");

            var bookCovers = await _bookRepository.GetBookCoversProcessOneByOneAsync(id, cancellationToken);

            //var bookCovers = await _bookRepository.GetBookCoversProcessAfterWaitForAllAsync(id);                                 

            /*
                for(int i = 0; i < 100; i++)
                {
                    // cálculos pesados (não suportam cancelamento por eles mesmos...)
                    // Usar  cancellationToken.IsCancellationRequested ou
                    // cancellationToken.ThrowIfCancellationRequested();
                }
            */


            return Ok((bookEntity, bookCovers));
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
