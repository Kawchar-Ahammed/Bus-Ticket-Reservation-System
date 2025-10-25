using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusTicket.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions", "busticket");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(t => t.PaymentId)
            .HasColumnName("payment_id")
            .IsRequired();

        builder.Property(t => t.GatewayTransactionId)
            .HasColumnName("gateway_transaction_id")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Gateway)
            .HasColumnName("gateway")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.Action)
            .HasColumnName("action")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(t => t.IsSuccess)
            .HasColumnName("is_success")
            .IsRequired();

        builder.Property(t => t.ResponseCode)
            .HasColumnName("response_code")
            .HasMaxLength(50);

        builder.Property(t => t.ResponseMessage)
            .HasColumnName("response_message")
            .HasMaxLength(500);

        builder.Property(t => t.RawResponse)
            .HasColumnName("raw_response")
            .HasColumnType("text");

        builder.Property(t => t.ProcessedAt)
            .HasColumnName("processed_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(t => t.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(t => t.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        // Relationships
        builder.HasOne(t => t.Payment)
            .WithMany(p => p.Transactions)
            .HasForeignKey(t => t.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.PaymentId)
            .HasDatabaseName("ix_transactions_payment_id");

        builder.HasIndex(t => t.GatewayTransactionId)
            .HasDatabaseName("ix_transactions_gateway_transaction_id");

        builder.HasIndex(t => t.ProcessedAt)
            .HasDatabaseName("ix_transactions_processed_at");
    }
}
