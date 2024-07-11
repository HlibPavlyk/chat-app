using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.EntityTypeConfiguration;

internal class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Title)
            .IsUnique();
        
        builder.Property(x => x.Title)
            .HasMaxLength(50)
            .IsUnicode()
            .IsRequired();

        builder.Property(x => x.Author)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasMany(x => x.Messages)
            .WithOne(x => x.Chat)
            .HasForeignKey(x => x.ChatId);
    }
}