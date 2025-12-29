namespace Daunt.Persistence.Entity;

public class Book
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Author { get; init; }
    public DateTime PublishedDate { get; init; }
    
    public Book(){}

    public Book(string title, string author, DateTime publishedDate) 
    {
        Title = title;
        Author = author;
        PublishedDate = publishedDate;
    }
};