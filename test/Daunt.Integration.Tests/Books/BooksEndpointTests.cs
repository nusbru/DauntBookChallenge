using System.Net;
using System.Net.Http.Json;
using Daunt.Contract.Books;
using Daunt.Persistence.Entity;

namespace Daunt.Integration.Tests.Books;
public class BooksEndpointTests(AppFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetAllBooks_WhenNoBooksExist_ReturnsEmptyList()
    {
        // Arrange
        await ClearDatabase();
        // Act
        var response = await Client.GetAsync("/api/books?page=1&size=10");
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books.Should().NotBeNull();
        books.Should().BeEmpty();
    }
    [Fact]
    public async Task GetAllBooks_WhenBooksExist_ReturnsAllBooks()
    {
        // Arrange
        await ClearDatabase();
        Book book1 = new ("The Great Gatsby", "Francis Scott Fitzgerald", new DateTime(1925, 4, 10));
        Book book2 = new("1984", "George Orwell", new DateTime(1949, 6, 8));
        DbContext.Books.AddRange(book1, book2);
        await DbContext.SaveChangesAsync();
        // Act
        var response = await Client.GetAsync("/api/books?page=1&size=10");
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books.Should().NotBeNull();
        books.Should().HaveCount(2);
        books.Should().Contain(b => b.Title == "The Great Gatsby");
        books.Should().Contain(b => b.Title == "1984");
    }
    [Fact]
    public async Task CreateBook_WithValidData_ReturnsCreatedBook()
    {
        // Arrange
        await ClearDatabase();
        var request = new BookRequest
        {
            Title = "To Kill a Mockingbird",
            Author = "Harper Lee",
            PublishedDate = new DateTime(1960, 7, 11)
        };
        // Act
        var response = await Client.PostAsJsonAsync("/api/books", request);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdBook = await response.Content.ReadFromJsonAsync<BookResponse>();
        createdBook.Should().NotBeNull();
        createdBook!.Title.Should().Be(request.Title);
        createdBook.Author.Should().Be(request.Author);
        createdBook.PublishedDate.Should().Be(request.PublishedDate);
        // Verify in database
        var bookInDb = await DbContext.Books.FindAsync(createdBook.Id);
        bookInDb.Should().NotBeNull();
    }
    [Fact]
    public async Task CreateBook_WithInvalidAuthor_ReturnsBadRequest()
    {
        // Arrange
        await ClearDatabase();
        var request = new BookRequest
        {
            Title = "Invalid Book",
            Author = "",
            PublishedDate = DateTime.UtcNow.AddDays(-1)
        };
        // Act
        var response = await Client.PostAsJsonAsync("/api/books", request);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task CreateBook_WithFuturePublishedDate_ReturnsBadRequest()
    {
        // Arrange
        await ClearDatabase();
        var request = new BookRequest
        {
            Title = "Future Book",
            Author = "Author Name",
            PublishedDate = DateTime.UtcNow.AddDays(1)
        };
        // Act
        var response = await Client.PostAsJsonAsync("/api/books", request);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
