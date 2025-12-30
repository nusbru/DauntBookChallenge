using Daunt.Contract.Books;
using Daunt.Core.Entity;

namespace Daunt.App.Helper;

public static class Mapper
{
    public static BookResponse ToBookResponse(this Book book) =>
        new()
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            PublishedDate = book.PublishedDate
        };
    
    public static BookResponse[] ToBookResponse(this IEnumerable<Book> books) =>
        books.Select(ToBookResponse).ToArray();
}