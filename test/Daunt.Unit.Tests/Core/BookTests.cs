using Daunt.Core.Entity;
using FluentAssertions;

namespace Daunt.Unit.Tests.Core;

public class BookTests
{
    [Fact]
    public void New_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var title = "Clean Code";
        var author = "Robert Martin";
        var date = DateTime.UtcNow.AddYears(-10);

        // Act
        var result = Book.New(title, author, date);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Title.Should().Be(title);
        result.Value.Author.Should().Be(author);
        result.Value.PublishedDate.Should().Be(date);
    }

    [Theory]
    [InlineData("", "Author", "Title cannot be null or empty")]
    [InlineData(null, "Author", "Title cannot be null or empty")]
    [InlineData("Title", "", "Author cannot be null or empty")]
    [InlineData("Title", null, "Author cannot be null or empty")]
    [InlineData("Title", "Author123", "Author contains invalid characters. Only letters and spaces are allowed.")]
    public void New_ShouldReturnFailure_WhenDataIsInvalid(string? title, string? author, string expectedError)
    {
        // Arrange
        var date = DateTime.UtcNow.AddYears(-1);

        // Act
        var result = Book.New(title!, author!, date);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessages.Should().Contain(x => x.ErrorMessage == expectedError);
    }

    [Fact]
    public void New_ShouldReturnFailure_WhenDateIsInFuture()
    {
        // Arrange
        var title = "Title";
        var author = "Author";
        var date = DateTime.UtcNow.AddDays(1);

        // Act
        var result = Book.New(title, author, date);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessages.Should().Contain(x => x.ErrorMessage == "Published Date cannot be in the future.");
    }

    [Fact]
    public void From_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Clean Code";
        var author = "Robert Martin";
        var date = DateTime.UtcNow.AddYears(-10);

        // Act
        var result = Book.From(id, title, author, date);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(id);
        result.Value.Title.Should().Be(title);
    }

    [Fact]
    public void From_ShouldReturnFailure_WhenDataIsInvalid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "";
        var author = "Author";
        var date = DateTime.UtcNow.AddYears(-1);

        // Act
        var result = Book.From(id, title, author, date);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
