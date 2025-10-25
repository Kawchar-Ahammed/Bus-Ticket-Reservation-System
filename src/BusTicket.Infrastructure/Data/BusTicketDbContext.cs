using BusTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BusTicket.Infrastructure.Data;

/// <summary>
/// Main DbContext for Bus Ticket Reservation System
/// </summary>
public class BusTicketDbContext : DbContext
{
    public BusTicketDbContext(DbContextOptions<BusTicketDbContext> options)
        : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Bus> Buses => Set<Bus>();
    public DbSet<Route> Routes => Set<Route>();
    public DbSet<BusSchedule> BusSchedules => Set<BusSchedule>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Passenger> Passengers => Set<Passenger>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Set default schema
        modelBuilder.HasDefaultSchema("busticket");

        // Configure PostgreSQL conventions
        ConfigurePostgreSqlConventions(modelBuilder);
    }

    /// <summary>
    /// Configure PostgreSQL naming conventions (snake_case)
    /// </summary>
    private void ConfigurePostgreSqlConventions(ModelBuilder modelBuilder)
    {
        // All table names will be in snake_case and plural
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Skip owned entity types (value objects stored inline)
            if (entity.IsOwned())
            {
                continue;
            }

            // Convert table names to snake_case
            var tableName = ToSnakeCase(entity.GetTableName() ?? entity.DisplayName());
            entity.SetTableName(tableName);

            // Convert column names to snake_case
            foreach (var property in entity.GetProperties())
            {
                var columnName = ToSnakeCase(property.Name);
                property.SetColumnName(columnName);
            }

            // Convert primary key names to snake_case
            foreach (var key in entity.GetKeys())
            {
                key.SetName(ToSnakeCase(key.GetName() ?? $"pk_{tableName}"));
            }

            // Convert foreign key names to snake_case
            foreach (var foreignKey in entity.GetForeignKeys())
            {
                var fkName = foreignKey.GetConstraintName();
                if (fkName != null)
                {
                    foreignKey.SetConstraintName(ToSnakeCase(fkName));
                }
            }

            // Convert index names to snake_case
            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (indexName != null)
                {
                    index.SetDatabaseName(ToSnakeCase(indexName));
                }
            }
        }
    }

    /// <summary>
    /// Convert PascalCase to snake_case
    /// </summary>
    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new System.Text.StringBuilder();
        result.Append(char.ToLower(input[0]));

        for (int i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                result.Append('_');
                result.Append(char.ToLower(input[i]));
            }
            else
            {
                result.Append(input[i]);
            }
        }

        return result.ToString();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update audit fields before saving
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Update audit fields (CreatedAt, UpdatedAt) for entities
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Domain.Common.Entity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (Domain.Common.Entity)entry.Entity;
            var now = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                // Set CreatedAt for new entities
                var createdAtProperty = entry.Property(nameof(Domain.Common.Entity.CreatedAt));
                if (createdAtProperty.CurrentValue is DateTime createdAt && createdAt == default)
                {
                    createdAtProperty.CurrentValue = now;
                }
            }

            // Always update UpdatedAt
            entry.Property(nameof(Domain.Common.Entity.UpdatedAt)).CurrentValue = now;
        }
    }
}
