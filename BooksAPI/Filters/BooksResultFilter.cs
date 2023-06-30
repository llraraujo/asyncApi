using AutoMapper;
using BooksAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BooksAPI.Filters
{
    public class BooksResultFilter : IAsyncResultFilter
    {
        private readonly IMapper _mapper;

        public BooksResultFilter(IMapper mapper) 
        { 
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var resultFromAction = context.Result as ObjectResult;
            if (resultFromAction?.Value == null || resultFromAction.StatusCode < 200 || resultFromAction.StatusCode >= 300)
            {
                await next();
                return;
            }

            resultFromAction.Value = _mapper.Map<IEnumerable<BookDto>>(resultFromAction.Value);
            await next();
        }
    }
}
