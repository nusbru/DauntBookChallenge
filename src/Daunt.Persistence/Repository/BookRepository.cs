using Daunt.Core.Entity;
using Daunt.Core.Repository;
using Daunt.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Daunt.Persistence;

public class BookRepository(DauntDbContext context) : IBookRepository
{
    public async Task<Book?> GetByIdAsync(Guid id)
    {
        var book = await context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        return book == null ? null 
            : Book.From(book.Id, book.Title, book.Author, book.PublishedDate).Value;
    }

    public async Task<IEnumerable<Book>> GetAllAsync(int page, int size)
    {
        var books = await context.Books.AsNoTracking().Skip((page - 1) * size).Take(size).ToListAsync();
        return books.Count == 0 ? [] :
            books.Select(b => Book.From(b.Id, b.Title, b.Author, b.PublishedDate).Value!); 
    }

    public async Task<Book> AddAsync(Book entity)
    {
        var bookEntity = new Entity.Book(entity.Title, entity.Author, entity.PublishedDate);
        await context.Books.AddAsync(bookEntity);
        await context.SaveChangesAsync();

        return Book.From(bookEntity.Id, bookEntity.Title, bookEntity.Author, bookEntity.PublishedDate).Value!;
    }

    public async Task Update(Book entity)
    {
        context.Books.Update(new (entity.Title, entity.Author, entity.PublishedDate){ Id =  entity.Id });
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        context.Books.Remove(new Entity.Book { Id = id });
        await context.SaveChangesAsync();
    }
}