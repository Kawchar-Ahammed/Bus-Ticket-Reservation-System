using BusTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusTicket.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Ticket aggregate root
/// </summary>
public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("tickets");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.TicketNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.BookingDate)
            .IsRequired();

        builder.Property(t => t.IsConfirmed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.IsCancelled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.ConfirmationDate)
            .IsRequired(false);

        builder.Property(t => t.CancellationDate)
            .IsRequired(false);

        // Value Object: Address (BoardingPoint) - stored in the same table
        builder.OwnsOne(t => t.BoardingPoint, boardingPoint =>
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
        builder.OwnsOne(t => t.DroppingPoint, droppingPoint =>
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
        builder.OwnsOne(t => t.Fare, fare =>
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
        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(t => t.TicketNumber).IsUnique();
        builder.HasIndex(t => t.BusScheduleId);
        builder.HasIndex(t => t.PassengerId);
        builder.HasIndex(t => t.SeatId);
        builder.HasIndex(t => t.BookingDate);
        builder.HasIndex(t => t.IsConfirmed);
        builder.HasIndex(t => t.IsCancelled);

        // Relationship: Ticket belongs to BusSchedule
        builder.HasOne(t => t.BusSchedule)
            .WithMany(s => s.Tickets)
            .HasForeignKey(t => t.BusScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: Ticket belongs to Passenger
        builder.HasOne(t => t.Passenger)
            .WithMany(p => p.Tickets)
            .HasForeignKey(t => t.PassengerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: Ticket belongs to Seat
        builder.HasOne(t => t.Seat)
            .WithMany()
            .HasForeignKey(t => t.SeatId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
