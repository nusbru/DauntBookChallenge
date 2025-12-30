using Daunt.Core.Entity;
using Daunt.Persistence.Context;
using Daunt.Persistence.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Daunt.Unit.Tests.Persistence;

public class BookRepositoryTests
{
    private readonly DauntDbContext _context;
    private readonly BookRepository _sut;

    public BookRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DauntDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DauntDbContext(options);
        _sut = new BookRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddBookToDatabase()
    {
        // Arrange
        var book = Book.New("Title", "Author", DateTime.UtcNow).Value!;

        // Act
        var result = await _sut.AddAsync(book);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        
        var dbBook = await _context.Books.FindAsync(result.Id);
        dbBook.Should().NotBeNull();
        dbBook!.Title.Should().Be("Title");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBook_WhenExists()
    {
        // Arrange
        var bookEntity = new Daunt.Persistence.Entity.Book("Title", "Author", DateTime.UtcNow);
        _context.Books.Add(bookEntity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByIdAsync(bookEntity.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(bookEntity.Id);
        result.Title.Should().Be("Title");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnBooks()
    {
        // Arrange
        _context.Books.Add(new Daunt.Persistence.Entity.Book("Title 1", "Author 1", DateTime.UtcNow));
        _context.Books.Add(new Daunt.Persistence.Entity.Book("Title 2", "Author 2", DateTime.UtcNow));
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllAsync(1, 10);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Update_ShouldUpdateBookInDatabase()
    {
        // Arrange
        var bookEntity = new Daunt.Persistence.Entity.Book("Title", "Author", DateTime.UtcNow);
        _context.Books.Add(bookEntity);
        await _context.SaveChangesAsync();
        _context.Entry(bookEntity).State = EntityState.Detached;

        var updatedBook = Book.From(bookEntity.Id, "Updated Title", "Updated Author", DateTime.UtcNow).Value!;

        // Act
        await _sut.Update(updatedBook);

        // Assert
        var dbBook = await _context.Books.FindAsync(bookEntity.Id);
        dbBook!.Title.Should().Be("Updated Title");
        dbBook.Author.Should().Be("Updated Author");
    }

    [Fact]
    public async Task Delete_ShouldRemoveBookFromDatabase()
    {
        // Arrange
        var bookEntity = new Daunt.Persistence.Entity.Book("Title", "Author", DateTime.UtcNow);
        _context.Books.Add(bookEntity);
        await _context.SaveChangesAsync();
        _context.Entry(bookEntity).State = EntityState.Detached;

        // Act
        await _sut.Delete(bookEntity.Id);

        // Assert
        var dbBook = await _context.Books.FindAsync(bookEntity.Id);
        dbBook.Should().BeNull();
    }
}
