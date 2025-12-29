namespace Daunt.Contract.Books;

public class BookResponse
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Author { get; init; }
    public required DateTime PublishedDate { get; init; }
}