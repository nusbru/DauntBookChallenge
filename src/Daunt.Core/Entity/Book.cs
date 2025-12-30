using System.Text.RegularExpressions;
using Daunt.Shared;

namespace Daunt.Core.Entity;

public class Book : EntityBase
{
    public string Title { get; init; } 
    public string Author { get; init; }
    public DateTime PublishedDate { get; init; }

    private Book()
    {
    }

    public static Result<Book> New(string title, string author, DateTime publishedDate)
    {
        var result = IsValid(title, author, publishedDate);

        if (!result.IsValid)
        {
            return Result<Book>.Failure(result.Errors);
        }

        return Result<Book>.Success(
            new Book
            {
                Title = title,
                Author = author,
                PublishedDate = publishedDate
            });
    }

    private static ValidationResult IsValid(string title, string author, DateTime publishedDate)
    {
        var results = new List<ValidationPropertyResult>();

        if (string.IsNullOrWhiteSpace(title))
            results.Add(new ValidationPropertyResult(nameof(title), $"Title cannot be null or empty"));

        if (string.IsNullOrWhiteSpace(author))
            results.Add(new ValidationPropertyResult(nameof(author), $"Author cannot be null or empty"));
        else if (!IsValidAuthor(author))
            results.Add(new ValidationPropertyResult(nameof(author),
                $"Author contains invalid characters. Only letters and spaces are allowed."));

        if (publishedDate > DateTime.UtcNow)
            results.Add(new ValidationPropertyResult(nameof(publishedDate),
                $"Published Date cannot be in the future."));

        return results.Count != 0
            ? new ValidationResult(false, results)
            : new ValidationResult(true, []);
    }

    private static readonly Regex AuthorRegex = new(@"^[A-Za-z]+(\s+[A-Za-z]+)*$", RegexOptions.Compiled);

    private static bool IsValidAuthor(string author)
    {
        return AuthorRegex.IsMatch(author.Trim());
    }

    public static Result<Book> From(Guid id, string title, string author, DateTime publishedDate)
    {
        var result = IsValid(title, author, publishedDate);

        if (!result.IsValid)
        {
            return Result<Book>.Failure(result.Errors);
        }

        return Result<Book>.Success(
            new Book
            {
                Id = id,
                Title = title,
                Author = author,
                PublishedDate = publishedDate
            });
    }
}