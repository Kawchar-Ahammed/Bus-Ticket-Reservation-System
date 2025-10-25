using BusTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusTicket.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for BusSchedule aggregate root
/// </summary>
public class BusScheduleConfiguration : IEntityTypeConfiguration<BusSchedule>
{
    public void Configure(EntityTypeBuilder<BusSchedule> builder)
    {
        builder.ToTable("bus_schedules");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.JourneyDate)
            .IsRequired();

        builder.Property(s => s.DepartureTime)
            .IsRequired();

        builder.Property(s => s.ArrivalTime)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Value Object: Address (BoardingPoint) - stored in the same table
        builder.OwnsOne(s => s.BoardingPoint, boardingPoint =>
        {
            boardingPoint.Property(a => a.City)
                .HasColumnName("boarding_city")
                .HasMaxLength(100)
                .IsRequired();

            boardingPoint.Property(a => a.Area)
                .HasColumnName("boarding_area")
                .HasMaxLength(100);

            boardingPoint.Property(a => a.Street)
                .HasColumnName("boarding_street")
                .HasMaxLength(200);

            boardingPoint.Property(a => a.PostalCode)
                .HasColumnName("boarding_postal_code")
                .HasMaxLength(20);
        });

        // Value Object: Address (DroppingPoint) - stored in the same table
        builder.OwnsOne(s => s.DroppingPoint, droppingPoint =>
        {
            droppingPoint.Property(a => a.City)
                .HasColumnName("dropping_city")
                .HasMaxLength(100)
                .IsRequired();

            droppingPoint.Property(a => a.Area)
                .HasColumnName("dropping_area")
                .HasMaxLength(100);

            droppingPoint.Property(a => a.Street)
                .HasColumnName("dropping_street")
                .HasMaxLength(200);

            droppingPoint.Property(a => a.PostalCode)
                .HasColumnName("dropping_postal_code")
                .HasMaxLength(20);
        });

        // Value Object: Money (Fare) - stored in the same table
        builder.OwnsOne(s => s.Fare, fare =>
        {
            fare.Property(m => m.Amount)
                .HasColumnName("fare_amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            fare.Property(m => m.Currency)
                .HasColumnName("fare_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Audit fields
        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(s => s.JourneyDate);
        builder.HasIndex(s => s.DepartureTime);
        builder.HasIndex(s => s.BusId);
        builder.HasIndex(s => s.RouteId);
        builder.HasIndex(s => s.IsActive);
        builder.HasIndex(s => new { s.RouteId, s.JourneyDate });

        // Relationship: BusSchedule belongs to Bus
        builder.HasOne(s => s.Bus)
            .WithMany(b => b.BusSchedules)
            .HasForeignKey(s => s.BusId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: BusSchedule belongs to Route
        builder.HasOne(s => s.Route)
            .WithMany(r => r.BusSchedules)
            .HasForeignKey(s => s.RouteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: One BusSchedule has many Tickets
        builder.HasMany(s => s.Tickets)
            .WithOne(t => t.BusSchedule)
            .HasForeignKey(t => t.BusScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: One BusSchedule has many Seats
        builder.HasMany(s => s.Seats)
            .WithOne(seat => seat.BusSchedule)
            .HasForeignKey(seat => seat.BusScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
