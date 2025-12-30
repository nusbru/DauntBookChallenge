using Daunt.App.Helper;
using Daunt.Core.Entity;
using FluentAssertions;

namespace Daunt.Unit.Tests.App;

public class MapperTests
{
    [Fact]
    public void ToBookResponse_ShouldMapCorrectly_WhenSingleBook()
    {
        // Arrange
        var book = Book.New("Title", "Author", DateTime.UtcNow).Value!;

        // Act
        var response = book.ToBookResponse();

        // Assert
        response.Id.Should().Be(book.Id);
        response.Title.Should().Be(book.Title);
        response.Author.Should().Be(book.Author);
        response.PublishedDate.Should().Be(book.PublishedDate);
    }

    [Fact]
    public void ToBookResponse_ShouldMapCorrectly_WhenCollection()
    {
        // Arrange
        var books = new[]
        {
            Book.New("Title 1", "Author One", DateTime.UtcNow).Value!,
            Book.New("Title 2", "Author Two", DateTime.UtcNow).Value!
        };

        // Act
        var responses = books.ToBookResponse();

        // Assert
        responses.Should().HaveCount(2);
        responses[0].Title.Should().Be("Title 1");
        responses[1].Title.Should().Be("Title 2");
    }
}
