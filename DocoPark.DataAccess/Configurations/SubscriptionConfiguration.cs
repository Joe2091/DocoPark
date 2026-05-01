using DocoPark.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocoPark.DataAccess.Configurations;

internal class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(s => s.StartDate)
            .IsRequired();

        builder.Property(s => s.MonthlyFee)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(s => s.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Transactions)
            .WithOne(t => t.Subscription)
            .HasForeignKey(t => t.SubscriptionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
