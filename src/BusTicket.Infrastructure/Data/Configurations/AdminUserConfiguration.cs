using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusTicket.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for AdminUser
/// </summary>
public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        // Table name
        builder.ToTable("admin_users");

        // Primary key
        builder.HasKey(a => a.Id);

        // Properties
        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(a => a.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Role)
            .IsRequired()
            .HasConversion<string>(); // Store enum as string in database

        builder.Property(a => a.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.LastLoginDate)
            .IsRequired(false);

        builder.Property(a => a.RefreshToken)
            .HasMaxLength(512);

        builder.Property(a => a.RefreshTokenExpiryTime)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(a => a.Email)
            .IsUnique()
            .HasDatabaseName("ix_admin_users_email");

        builder.HasIndex(a => a.RefreshToken)
            .HasDatabaseName("ix_admin_users_refresh_token");

        builder.HasIndex(a => a.IsActive)
            .HasDatabaseName("ix_admin_users_is_active");

        // Audit fields
        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired();
    }
}
