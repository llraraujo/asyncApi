using BooksAPI.Models.External;

namespace BooksAPI.Models
{
    public class BookCoverDto
    {
        public string Id { get; set; }

        public BookCoverDto(string id)
        {
            Id = id;
        }

        //public byte[]? Content { get; set; }

        //public BookCoverDto(string id, byte[]? content)
        //{
        //    Id = id;
        //    Content = content;
        //}
    }
}
