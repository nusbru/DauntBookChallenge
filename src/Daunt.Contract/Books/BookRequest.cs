namespace Daunt.Contract.Books;

public class BookRequest
{
    public required string Title { get; init; }
    public required string Author { get; init; }
    public required DateTime PublishedDate { get; init; }
}