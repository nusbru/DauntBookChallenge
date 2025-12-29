using Daunt.Core.Entity;
using Daunt.Core.Repository;
using Daunt.Shared;

namespace Daunt.Core.Service;

public class BookService(IBookRepository bookRepository)
{
    public async Task<Result<IEnumerable<Book>>> GetAllBooks(int page, int size)
    {
        var books = await bookRepository.GetAllAsync(page, size);
        return Result<IEnumerable<Book>>.Success(books);
    }
    
    public async Task<Result<Book>> GetBookById(Guid bookId)
    {
        var book = await bookRepository.GetByIdAsync(bookId);

        return book is null
            ? Result<Book>.Failure([new ValidationPropertyResult(nameof(bookId), "Book not found")])
            : Result<Book>.Success(book);
    }

    public async Task<Result<Book>> SaveBook(string title, string author, DateTime publishedDate)
    {
        var book = Book.New(title, author, publishedDate);
        
        if (!book.IsSuccess)
            return Result<Book>.Failure(book.ErrorMessages);
        
        try
        {
            var result = await bookRepository.AddAsync(book.Value!);
            return Result<Book>.Success(result);
        }
        catch
        {
            return Result<Book>.Failure([new  ValidationPropertyResult(nameof(book), "Book not created")]);
        }
    }

    public async Task<Result<Book>> UpdateBook(Guid bookId, string title, string author, DateTime publishedDate)
    {
        var result = Book.From(bookId, title, author, publishedDate);

        if (!result.IsSuccess)
            return Result<Book>.Failure(result.ErrorMessages);

        try
        {
            await bookRepository.Update(result.Value!);
            return Result<Book>.Success(result.Value!);
        }
        catch(Exception ex)
        {
            return Result<Book>.Failure([new  ValidationPropertyResult(nameof(bookId), "Book not updated")]);
        }
    }
    
    public async Task<Result<Book>> DeleteBook(Guid bookId)
    {
        try
        {
            await bookRepository.Delete(bookId);
            return Result<Book>.Success();
        }
        catch(Exception ex)
        {
            return Result<Book>.Failure([new  ValidationPropertyResult(nameof(bookId), "Book not deleted")]);
        }
    }
}