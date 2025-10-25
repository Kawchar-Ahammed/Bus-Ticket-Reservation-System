using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusTicket.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments", "busticket");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(p => p.TicketId)
            .HasColumnName("ticket_id")
            .IsRequired();

        builder.OwnsOne(p => p.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(p => p.PaymentMethod)
            .HasColumnName("payment_method")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.Gateway)
            .HasColumnName("gateway")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.TransactionId)
            .HasColumnName("transaction_id")
            .HasMaxLength(100);

        builder.Property(p => p.GatewayTransactionId)
            .HasColumnName("gateway_transaction_id")
            .HasMaxLength(200);

        builder.Property(p => p.PaymentDate)
            .HasColumnName("payment_date")
            .HasColumnType("timestamp with time zone");

        builder.Property(p => p.RefundAmount)
            .HasColumnName("refund_amount")
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(p => p.RefundDate)
            .HasColumnName("refund_date")
            .HasColumnType("timestamp with time zone");

        builder.Property(p => p.RefundReason)
            .HasColumnName("refund_reason")
            .HasMaxLength(500);

        builder.Property(p => p.FailureReason)
            .HasColumnName("failure_reason")
            .HasMaxLength(500);

        builder.Property(p => p.GatewayResponse)
            .HasColumnName("gateway_response")
            .HasColumnType("text");

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(p => p.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        // Relationships
        builder.HasOne(p => p.Ticket)
            .WithMany()
            .HasForeignKey(p => p.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Transactions)
            .WithOne(t => t.Payment)
            .HasForeignKey(t => t.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.TicketId)
            .HasDatabaseName("ix_payments_ticket_id");

        builder.HasIndex(p => p.TransactionId)
            .IsUnique()
            .HasDatabaseName("ix_payments_transaction_id");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("ix_payments_status");

        builder.HasIndex(p => p.PaymentDate)
            .HasDatabaseName("ix_payments_payment_date");
    }
}
