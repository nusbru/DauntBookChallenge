using Daunt.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Daunt.Persistence.Configuration;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.Property(e => e.Id)
            .HasValueGenerator<SequentialGuidValueGenerator>();
        
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(e => e.Author)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(e => e.PublishedDate)
            .IsRequired();
    }
}