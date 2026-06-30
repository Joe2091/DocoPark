using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocoPark.Domain.Entities;
using DocoPark.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocoPark.DataAccess.Configurations;

internal class ParkingSpotConfiguration : IEntityTypeConfiguration<ParkingSpot>
{
    public void Configure(EntityTypeBuilder<ParkingSpot> builder)
    {
        builder.Property(p => p.SpotNumber).HasMaxLength(10).IsRequired();
        builder.HasIndex(p => p.SpotNumber).IsUnique();

        builder.Property(p => p.SpotStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Seed 50 parking spots
        var spots = Enumerable.Range(1, 50).Select(i => new ParkingSpot
        {
            Id = i,
            SpotNumber = $"S{i:D3}",
            SpotStatus = SpotStatus.Available
        }).ToArray();

        builder.HasData(spots);
    }
}
