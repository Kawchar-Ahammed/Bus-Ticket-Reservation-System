using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusTicket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentAndTransactionEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payments",
                schema: "busticket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ticket_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    payment_method = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    gateway = table.Column<int>(type: "integer", nullable: false),
                    transaction_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    gateway_transaction_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    payment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    refund_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    refund_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    refund_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    failure_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    gateway_response = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_payments", x => x.id);
                    table.ForeignKey(
                        name: "f_k_payments_tickets_ticket_id",
                        column: x => x.ticket_id,
                        principalSchema: "busticket",
                        principalTable: "tickets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                schema: "busticket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gateway_transaction_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    gateway = table.Column<int>(type: "integer", nullable: false),
                    action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    is_success = table.Column<bool>(type: "boolean", nullable: false),
                    response_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    response_message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    raw_response = table.Column<string>(type: "text", nullable: true),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_transactions", x => x.id);
                    table.ForeignKey(
                        name: "f_k_transactions_payments_payment_id",
                        column: x => x.payment_id,
                        principalSchema: "busticket",
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_payments_payment_date",
                schema: "busticket",
                table: "payments",
                column: "payment_date");

            migrationBuilder.CreateIndex(
                name: "ix_payments_status",
                schema: "busticket",
                table: "payments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_payments_ticket_id",
                schema: "busticket",
                table: "payments",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_transaction_id",
                schema: "busticket",
                table: "payments",
                column: "transaction_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_gateway_transaction_id",
                schema: "busticket",
                table: "transactions",
                column: "gateway_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_payment_id",
                schema: "busticket",
                table: "transactions",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_processed_at",
                schema: "busticket",
                table: "transactions",
                column: "processed_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transactions",
                schema: "busticket");

            migrationBuilder.DropTable(
                name: "payments",
                schema: "busticket");
        }
    }
}
