using Daunt.App.Helper;
using Daunt.Contract.Books;
using Daunt.Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace Daunt.App.Http;

public static class BooksEndpoint
{
    public static void MapBookEndpoints(this WebApplication app)
    {
        app.MapPost("/api/books", CreateBook)
            .WithName("CreateCreateBook")
            .WithSummary("Create a new Book")
            .WithTags("Books")
            .Produces<BookResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        app.MapGet("/api/books/{bookId:guid}", GetBook)
            .WithName("GetBook")
            .WithSummary("Get a Book by ID")
            .WithTags("Books")
            .Produces<BookResponse>()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
        
        app.MapGet("/api/books", GetBooks)
            .WithName("GetBooks")
            .WithSummary("Get all Books")
            .WithTags("Books")
            .Produces<BookResponse>()
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        app.MapPut("/api/books/{bookId:guid}", UpdateBook)
            .WithName("UpdateBook")
            .WithSummary("Update an existing Book")
            .WithTags("Books")
            .Produces<BookResponse>()
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        app.MapDelete("/api/books/{bookId:guid}", DeleteBook)
            .WithName("DeleteBook")
            .WithSummary("Delete a Book")
            .WithTags("Books")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateBook([FromBody] BookRequest request, BookService bookService)
    {
        var result = await bookService.SaveBook(request.Title, request.Author, request.PublishedDate);
        return !result.IsSuccess ? 
            Results.BadRequest(result.ErrorMessages) 
            : Results.Created(new Uri($"/api/books/{result.Value!.Id}"), result.Value!.ToBookResponse());
    }

    private static async Task<IResult> GetBook([FromRoute] Guid bookId, BookService bookService)
    {
        var result = await bookService.GetBookById(bookId);
        return !result.IsSuccess ? 
            Results.NotFound(result.ErrorMessages) 
            : Results.Ok(result.Value!.ToBookResponse());
    }
    
    private static async Task<IResult> GetBooks([FromQuery] int page, [FromQuery] int size, BookService bookService)
    {
        var result = await bookService.GetAllBooks(page, size);
        return !result.IsSuccess ? 
            Results.NotFound(result.ErrorMessages) 
            : Results.Ok(result.Value!.ToBookResponse());
    }

    private static async Task<IResult> UpdateBook([FromRoute] Guid bookId, [FromBody] BookRequest request, BookService bookService)
    {
        var result = await bookService.UpdateBook(bookId, request.Title, request.Author, request.PublishedDate);
        return !result.IsSuccess ? 
            Results.BadRequest(result.ErrorMessages) 
            : Results.NoContent();
    }

    private static async Task<IResult> DeleteBook(Guid bookId, BookService bookService)
    {
        var result = await bookService.DeleteBook(bookId);
        
        return !result.IsSuccess ? 
            Results.BadRequest(result.ErrorMessages) 
            : Results.NoContent();
    }
}