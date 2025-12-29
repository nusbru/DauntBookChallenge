using Daunt.Persistence.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Daunt.Integration.Tests;

public abstract class IntegrationTestBase : IClassFixture<AppFactory>, IDisposable
{
    protected readonly HttpClient Client;
    protected readonly AppFactory Factory;
    private readonly IServiceScope _scope;
    protected readonly DauntDbContext DbContext;

    protected IntegrationTestBase(AppFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<DauntDbContext>();
    }

    protected async Task ClearDatabase()
    {
        DbContext.Books.RemoveRange(DbContext.Books);
        await DbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        Client?.Dispose();
        GC.SuppressFinalize(this);
    }
}

