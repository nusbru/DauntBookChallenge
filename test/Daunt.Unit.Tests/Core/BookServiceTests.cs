using Daunt.Core.Entity;
using Daunt.Core.Repository;
using Daunt.Core.Service;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Daunt.Unit.Tests.Core;

public class BookServiceTests
{
    private readonly IBookRepository _bookRepository;
    private readonly BookService _sut;

    public BookServiceTests()
    {
        _bookRepository = Substitute.For<IBookRepository>();
        _sut = new BookService(_bookRepository);
    }

    [Fact]
    public async Task GetAllBooks_ShouldReturnSuccess_WhenCalled()
    {
        // Arrange
        var books = new List<Book>
        {
            Book.New("Title 1", "Author One", DateTime.UtcNow.AddYears(-1)).Value!,
            Book.New("Title 2", "Author Two", DateTime.UtcNow.AddYears(-2)).Value!
        };
        _bookRepository.GetAllAsync(1, 10).Returns(books);

        // Act
        var result = await _sut.GetAllBooks(1, 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(books);
    }

    [Fact]
    public async Task GetBookById_ShouldReturnSuccess_WhenBookExists()
    {
        // Arrange
        var book = Book.New("Title", "Author", DateTime.UtcNow.AddYears(-1)).Value!;
        _bookRepository.GetByIdAsync(book.Id).Returns(book);

        // Act
        var result = await _sut.GetBookById(book.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(book);
    }

    [Fact]
    public async Task GetBookById_ShouldReturnFailure_WhenBookDoesNotExist()
    {
        // Arrange
        _bookRepository.GetByIdAsync(Arg.Any<Guid>()).Returns((Book?)null);

        // Act
        var result = await _sut.GetBookById(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessages.Should().Contain(x => x.ErrorMessage == "Book not found");
    }

    [Fact]
    public async Task SaveBook_ShouldReturnSuccess_WhenBookIsValid()
    {
        // Arrange
        var title = "Valid Title";
        var author = "Valid Author";
        var date = DateTime.UtcNow.AddYears(-1);
        
        _bookRepository.AddAsync(Arg.Any<Book>()).Returns(callInfo => callInfo.Arg<Book>());

        // Act
        var result = await _sut.SaveBook(title, author, date);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Title.Should().Be(title);
        await _bookRepository.Received(1).AddAsync(Arg.Is<Book>(b => b.Title == title));
    }

    [Fact]
    public async Task SaveBook_ShouldReturnFailure_WhenBookIsInvalid()
    {
        // Arrange
        var title = ""; // Invalid
        var author = "Valid Author";
        var date = DateTime.UtcNow.AddYears(-1);

        // Act
        var result = await _sut.SaveBook(title, author, date);

        // Assert
        result.IsSuccess.Should().BeFalse();
        await _bookRepository.DidNotReceive().AddAsync(Arg.Any<Book>());
    }

    [Fact]
    public async Task SaveBook_ShouldReturnFailure_WhenRepositoryThrows()
    {
        // Arrange
        var title = "Valid Title";
        var author = "Valid Author";
        var date = DateTime.UtcNow.AddYears(-1);
        
        _bookRepository.AddAsync(Arg.Any<Book>()).Throws(new Exception("Db Error"));

        // Act
        var result = await _sut.SaveBook(title, author, date);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessages.Should().Contain(x => x.ErrorMessage == "Book not created");
    }

    [Fact]
    public async Task UpdateBook_ShouldReturnSuccess_WhenBookIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Updated Title";
        var author = "Updated Author";
        var date = DateTime.UtcNow.AddYears(-1);

        // Act
        var result = await _sut.UpdateBook(id, title, author, date);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _bookRepository.Received(1).Update(Arg.Is<Book>(b => b.Id == id && b.Title == title));
    }

    [Fact]
    public async Task UpdateBook_ShouldReturnFailure_WhenBookIsInvalid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = ""; // Invalid
        var author = "Updated Author";
        var date = DateTime.UtcNow.AddYears(-1);

        // Act
        var result = await _sut.UpdateBook(id, title, author, date);

        // Assert
        result.IsSuccess.Should().BeFalse();
        await _bookRepository.DidNotReceive().Update(Arg.Any<Book>());
    }

    [Fact]
    public async Task UpdateBook_ShouldReturnFailure_WhenRepositoryThrows()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Updated Title";
        var author = "Updated Author";
        var date = DateTime.UtcNow.AddYears(-1);
        
        _bookRepository.When(x => x.Update(Arg.Any<Book>())).Do(x => { throw new Exception("Db Error"); });

        // Act
        var result = await _sut.UpdateBook(id, title, author, date);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessages.Should().Contain(x => x.ErrorMessage == "Book not updated");
    }

    [Fact]
    public async Task DeleteBook_ShouldReturnSuccess_WhenCalled()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = await _sut.DeleteBook(id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _bookRepository.Received(1).Delete(id);
    }

    [Fact]
    public async Task DeleteBook_ShouldReturnFailure_WhenRepositoryThrows()
    {
        // Arrange
        var id = Guid.NewGuid();
        _bookRepository.When(x => x.Delete(id)).Do(x => { throw new Exception("Db Error"); });

        // Act
        var result = await _sut.DeleteBook(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessages.Should().Contain(x => x.ErrorMessage == "Book not deleted");
    }
}
