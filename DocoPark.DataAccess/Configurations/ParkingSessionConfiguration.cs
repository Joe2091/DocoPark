using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocoPark.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocoPark.DataAccess.Configurations;

internal class ParkingSessionConfiguration : IEntityTypeConfiguration<ParkingSession>
{
    public void Configure(EntityTypeBuilder<ParkingSession> builder)
    {
        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.CheckInTime)
            .IsRequired();

        builder.Property(ps => ps.TotalCost)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(ps => ps.Vehicle)
            .WithMany(v => v.ParkingSessions)
            .HasForeignKey(ps => ps.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ps => ps.ParkingSpot)
            .WithMany(p => p.ParkingSessions)
            .HasForeignKey(ps => ps.ParkingSpotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ps => ps.User)
            .WithMany(u => u.ParkingSessions)
            .HasForeignKey(ps => ps.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(ps => ps.Transactions)
            .WithOne(t => t.ParkingSession)
            .HasForeignKey(t => t.ParkingSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
