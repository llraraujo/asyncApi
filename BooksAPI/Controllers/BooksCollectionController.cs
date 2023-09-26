using AutoMapper;
using BooksAPI.Entities;
using BooksAPI.Filters;
using BooksAPI.Helpers;
using BooksAPI.Models;
using BooksAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BooksAPI.Controllers
{
    [Route("api/bookcollections")]
    [ApiController]
    [TypeFilter(typeof(BooksResultFilter))]
    public class BooksCollectionController : ControllerBase
    {
        private readonly IBookRepository _booksRepository;
        private readonly IMapper _mapper;

        public BooksCollectionController(IBookRepository booksRepository, IMapper mapper)
        {
            _booksRepository = booksRepository ?? 
                        throw new ArgumentNullException(nameof(booksRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookCollection(IEnumerable<BookForCreationDto> bookCollection)
        {

            var bookEntities = _mapper.Map<IEnumerable<Book>>(bookCollection);
            foreach(var book in bookEntities)
            {
                _booksRepository.AddBook(book);
            }
            await _booksRepository.SaveChangesAsync();
            var booksToReturn = await _booksRepository.GetAllBooksAsync(bookEntities.Select(b => b.Id).ToList());

            var bookIds = string.Join(",", booksToReturn.Select(b => b.Id));

            return CreatedAtRoute("GetBookCollection", new { bookIds }, booksToReturn);
        }

        [HttpGet("{bookIds}", Name = "GetBookCollection")]        
        public async Task<IActionResult> GetBookCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]   
            IEnumerable<Guid> bookIds )
        {
            var bookEntities = await _booksRepository.GetAllBooksAsync(bookIds);
            if(bookIds.Count() != bookEntities.Count())
            {
                return NotFound();
            }
            return Ok(bookEntities);
        }
    }
}
