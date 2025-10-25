using BusTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusTicket.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Route entity
/// </summary>
public class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder)
    {
        builder.ToTable("routes");
        builder.HasKey(r => r.Id);

        // Value Object: Address (FromLocation) - stored in the same table
        builder.OwnsOne(r => r.FromLocation, fromLocation =>
        {
            fromLocation.Property(a => a.City)
                .HasColumnName("from_city")
                .HasMaxLength(100)
                .IsRequired();

            fromLocation.Property(a => a.Area)
                .HasColumnName("from_area")
                .HasMaxLength(100);

            fromLocation.Property(a => a.Street)
                .HasColumnName("from_street")
                .HasMaxLength(200);

            fromLocation.Property(a => a.PostalCode)
                .HasColumnName("from_postal_code")
                .HasMaxLength(20);
        });

        // Value Object: Address (ToLocation) - stored in the same table
        builder.OwnsOne(r => r.ToLocation, toLocation =>
        {
            toLocation.Property(a => a.City)
                .HasColumnName("to_city")
                .HasMaxLength(100)
                .IsRequired();

            toLocation.Property(a => a.Area)
                .HasColumnName("to_area")
                .HasMaxLength(100);

            toLocation.Property(a => a.Street)
                .HasColumnName("to_street")
                .HasMaxLength(200);

            toLocation.Property(a => a.PostalCode)
                .HasColumnName("to_postal_code")
                .HasMaxLength(20);
        });

        builder.Property(r => r.DistanceInKm)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(r => r.EstimatedDuration)
            .IsRequired();

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Audit fields
        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(r => r.IsActive);

        // Relationship: One Route has many Schedules
        builder.HasMany(r => r.BusSchedules)
            .WithOne(s => s.Route)
            .HasForeignKey(s => s.RouteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
