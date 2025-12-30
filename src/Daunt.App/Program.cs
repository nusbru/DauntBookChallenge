using Daunt.App.Http;
using Daunt.Core.Repository;
using Daunt.Core.Service;
using Daunt.Persistence.Context;
using Daunt.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddProblemDetails();

        builder.Services.AddDbContext<DauntDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDatabase");
            options.EnableSensitiveDataLogging();
        });

        // Register Core Services
        builder.Services.AddScoped<IBookRepository, BookRepository>();
        builder.Services.AddScoped<BookService>();

        var app = builder.Build();

        app.UseExceptionHandler();
        app.UseStatusCodePages();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference("/docs", options =>
            {
                options.WithTitle("Daunt Lib")
                    .WithTheme(ScalarTheme.Purple)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }

        // Map API endpoints
        app.MapBookEndpoints();

        app.Run();
    }
}