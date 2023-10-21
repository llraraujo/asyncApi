using AutoMapper;
using BooksAPI.Entities;
using BooksAPI.Models;
using BooksAPI.Models.External;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BooksAPI.Filters
{
    public class BookWithCoversResultFilter : IAsyncResultFilter
    {
        private readonly IMapper _mapper;

        public BookWithCoversResultFilter(IMapper mapper) 
        {
            _mapper = mapper;
        }


        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var resultFromAction = context.Result as ObjectResult;
            if (resultFromAction?.Value == null || resultFromAction.StatusCode < 200 || resultFromAction.StatusCode >= 300)
            {
                await next();
                return;
            }

            var (book, bookCovers) = ((Book book, IEnumerable<Models.External.BookCoverDto> bookCovers))resultFromAction.Value;

            var mappedBook = _mapper.Map<BookWithCoversDto>(book);
            resultFromAction.Value = _mapper.Map(bookCovers, mappedBook);

            await next();
        }
    }
}
