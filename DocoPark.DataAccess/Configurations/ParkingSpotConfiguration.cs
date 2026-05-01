using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocoPark.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocoPark.DataAccess.Configurations;

internal class ParkingSpotConfiguration : IEntityTypeConfiguration<ParkingSpot>
{
    public void Configure(EntityTypeBuilder<ParkingSpot> builder)
    {
        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.SpotNumber)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(ps => ps.SpotNumber)
            .IsUnique();

        builder.Property(ps => ps.VehicleType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne(ps => ps.CurrentSession)
            .WithOne()
            .HasForeignKey<ParkingSpot>(ps => ps.CurrentSessionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(ps => ps.ParkingSessions)
            .WithOne(s => s.ParkingSpot)
            .HasForeignKey(s => s.ParkingSpotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ps => ps.Reservations)
            .WithOne(r => r.ParkingSpot)
            .HasForeignKey(r => r.ParkingSpotId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
