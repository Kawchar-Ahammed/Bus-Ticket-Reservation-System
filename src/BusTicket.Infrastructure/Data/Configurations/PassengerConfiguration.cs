using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusTicket.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Passenger entity
/// </summary>
public class PassengerConfiguration : IEntityTypeConfiguration<Passenger>
{
    public void Configure(EntityTypeBuilder<Passenger> builder)
    {
        builder.ToTable("passengers");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Value Object: PhoneNumber - stored in the same table
        builder.OwnsOne(p => p.PhoneNumber, phoneNumber =>
        {
            phoneNumber.Property(ph => ph.Value)
                .HasColumnName("phone_number")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.Property(p => p.Email)
            .HasMaxLength(100);

        builder.Property(p => p.Gender)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(p => p.Age);

        // Audit fields
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(p => p.Email);

        // Relationship: One Passenger has many Tickets
        builder.HasMany(p => p.Tickets)
            .WithOne(t => t.Passenger)
            .HasForeignKey(t => t.PassengerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
