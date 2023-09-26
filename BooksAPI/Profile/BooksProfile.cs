using AutoMapper;
using BooksAPI.Entities;
using BooksAPI.Models;

namespace BooksAPI.Filters
{
    public class BooksProfile:Profile 
    {
        public BooksProfile()
        {
            CreateMap<Book, BookDto>()
             .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src =>
                 $"{src.Author.FirstName} {src.Author.LastName}"))
             .ConstructUsing(src => 
             new BookDto(
                 src.Id,
                 string.Empty,
                 src.Title,
                 src.Description
                )
            );

            CreateMap<BookForCreationDto, Book>()
                .ConstructUsing(src => new Book(
                    Guid.NewGuid(),
                    src.AuthorId,
                    src.Title,
                    src.Description));
        }
    }
}
