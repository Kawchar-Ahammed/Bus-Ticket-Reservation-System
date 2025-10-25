using BusTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusTicket.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Bus entity
/// </summary>
public class BusConfiguration : IEntityTypeConfiguration<Bus>
{
    public void Configure(EntityTypeBuilder<Bus> builder)
    {
        builder.ToTable("buses");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BusNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.BusName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.TotalSeats)
            .IsRequired();

        builder.Property(b => b.Description)
            .HasMaxLength(500);

        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Audit fields
        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(b => b.BusNumber).IsUnique();
        builder.HasIndex(b => b.CompanyId);
        builder.HasIndex(b => b.IsActive);

        // Relationship: Bus belongs to Company
        builder.HasOne(b => b.Company)
            .WithMany(c => c.Buses)
            .HasForeignKey(b => b.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: One Bus has many Schedules
        builder.HasMany(b => b.BusSchedules)
            .WithOne(s => s.Bus)
            .HasForeignKey(s => s.BusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
