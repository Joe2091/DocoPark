using DocoPark.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocoPark.DataAccess.Configurations;

internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.TransactionDate)
            .IsRequired();

        builder.Property(t => t.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne(t => t.User)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.ParkingSession)
            .WithMany(ps => ps.Transactions)
            .HasForeignKey(t => t.ParkingSessionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Subscription)
            .WithMany(s => s.Transactions)
            .HasForeignKey(t => t.SubscriptionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
