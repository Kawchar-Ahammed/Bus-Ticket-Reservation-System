using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusTicket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "busticket");

            migrationBuilder.CreateTable(
                name: "companies",
                schema: "busticket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    contact_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_companies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "passengers",
                schema: "busticket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    age = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_passengers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "routes",
                schema: "busticket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    from_area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    from_street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    from_postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    to_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    to_area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    to_street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    to_postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    distance_in_km = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    estimated_duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_routes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "buses",
                schema: "busticket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bus_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    bus_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_seats = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_buses", x => x.id);
                    table.ForeignKey(
                        name: "f_k_buses_companies_company_id",
                        column: x => x.company_id,
                        principalSchema: "busticket",
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bus_schedules",
                schema: "busticket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bus_id = table.Column<Guid>(type: "uuid", nullable: false),
                    route_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    departure_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    arrival_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    fare_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    fare_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    boarding_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    boarding_area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    boarding_street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    boarding_postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    dropping_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    dropping_area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    dropping_street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    dropping_postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_bus_schedules", x => x.id);
                    table.ForeignKey(
                        name: "f_k_bus_schedules_buses_bus_id",
                        column: x => x.bus_id,
                        principalSchema: "busticket",
                        principalTable: "buses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_bus_schedules_routes_route_id",
                        column: x => x.route_id,
                        principalSchema: "busticket",
                        principalTable: "routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "seats",
                schema: "busticket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bus_schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seat_number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    row = table.Column<int>(type: "integer", nullable: false),
                    column = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ticket_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ticket_id1 = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_seats", x => x.id);
                    table.ForeignKey(
                        name: "f_k_seats_bus_schedules_bus_schedule_id",
                        column: x => x.bus_schedule_id,
                        principalSchema: "busticket",
                        principalTable: "bus_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tickets",
                schema: "busticket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ticket_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    bus_schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    passenger_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    boarding_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    boarding_area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    boarding_street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    boarding_postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    dropping_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    dropping_area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    dropping_street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    dropping_postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    fare_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    fare_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    booking_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_cancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    confirmation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancellation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_tickets", x => x.id);
                    table.ForeignKey(
                        name: "f_k_tickets_bus_schedules_bus_schedule_id",
                        column: x => x.bus_schedule_id,
                        principalSchema: "busticket",
                        principalTable: "bus_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_tickets_passengers_passenger_id",
                        column: x => x.passenger_id,
                        principalSchema: "busticket",
                        principalTable: "passengers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_tickets_seats_seat_id",
                        column: x => x.seat_id,
                        principalSchema: "busticket",
                        principalTable: "seats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_bus_schedules_bus_id",
                schema: "busticket",
                table: "bus_schedules",
                column: "bus_id");

            migrationBuilder.CreateIndex(
                name: "i_x_bus_schedules_departure_time",
                schema: "busticket",
                table: "bus_schedules",
                column: "departure_time");

            migrationBuilder.CreateIndex(
                name: "i_x_bus_schedules_is_active",
                schema: "busticket",
                table: "bus_schedules",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_bus_schedules_journey_date",
                schema: "busticket",
                table: "bus_schedules",
                column: "journey_date");

            migrationBuilder.CreateIndex(
                name: "i_x_bus_schedules_route_id",
                schema: "busticket",
                table: "bus_schedules",
                column: "route_id");

            migrationBuilder.CreateIndex(
                name: "i_x_bus_schedules_route_id_journey_date",
                schema: "busticket",
                table: "bus_schedules",
                columns: new[] { "route_id", "journey_date" });

            migrationBuilder.CreateIndex(
                name: "i_x_buses_bus_number",
                schema: "busticket",
                table: "buses",
                column: "bus_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_buses_company_id",
                schema: "busticket",
                table: "buses",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "i_x_buses_is_active",
                schema: "busticket",
                table: "buses",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_companies_email",
                schema: "busticket",
                table: "companies",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "i_x_companies_name",
                schema: "busticket",
                table: "companies",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "i_x_passengers_email",
                schema: "busticket",
                table: "passengers",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "i_x_routes_is_active",
                schema: "busticket",
                table: "routes",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_seats_bus_schedule_id",
                schema: "busticket",
                table: "seats",
                column: "bus_schedule_id");

            migrationBuilder.CreateIndex(
                name: "i_x_seats_seat_number_status",
                schema: "busticket",
                table: "seats",
                columns: new[] { "seat_number", "status" });

            migrationBuilder.CreateIndex(
                name: "i_x_seats_status",
                schema: "busticket",
                table: "seats",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "i_x_seats_ticket_id",
                schema: "busticket",
                table: "seats",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "i_x_seats_ticket_id1",
                schema: "busticket",
                table: "seats",
                column: "ticket_id1");

            migrationBuilder.CreateIndex(
                name: "i_x_tickets_booking_date",
                schema: "busticket",
                table: "tickets",
                column: "booking_date");

            migrationBuilder.CreateIndex(
                name: "i_x_tickets_bus_schedule_id",
                schema: "busticket",
                table: "tickets",
                column: "bus_schedule_id");

            migrationBuilder.CreateIndex(
                name: "i_x_tickets_is_cancelled",
                schema: "busticket",
                table: "tickets",
                column: "is_cancelled");

            migrationBuilder.CreateIndex(
                name: "i_x_tickets_is_confirmed",
                schema: "busticket",
                table: "tickets",
                column: "is_confirmed");

            migrationBuilder.CreateIndex(
                name: "i_x_tickets_passenger_id",
                schema: "busticket",
                table: "tickets",
                column: "passenger_id");

            migrationBuilder.CreateIndex(
                name: "i_x_tickets_seat_id",
                schema: "busticket",
                table: "tickets",
                column: "seat_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_tickets_ticket_number",
                schema: "busticket",
                table: "tickets",
                column: "ticket_number",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "f_k_seats_tickets_ticket_id1",
                schema: "busticket",
                table: "seats",
                column: "ticket_id1",
                principalSchema: "busticket",
                principalTable: "tickets",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_bus_schedules_buses_bus_id",
                schema: "busticket",
                table: "bus_schedules");

            migrationBuilder.DropForeignKey(
                name: "f_k_bus_schedules_routes_route_id",
                schema: "busticket",
                table: "bus_schedules");

            migrationBuilder.DropForeignKey(
                name: "f_k_seats_bus_schedules_bus_schedule_id",
                schema: "busticket",
                table: "seats");

            migrationBuilder.DropForeignKey(
                name: "f_k_tickets_bus_schedules_bus_schedule_id",
                schema: "busticket",
                table: "tickets");

            migrationBuilder.DropForeignKey(
                name: "f_k_seats_tickets_ticket_id1",
                schema: "busticket",
                table: "seats");

            migrationBuilder.DropTable(
                name: "buses",
                schema: "busticket");

            migrationBuilder.DropTable(
                name: "companies",
                schema: "busticket");

            migrationBuilder.DropTable(
                name: "routes",
                schema: "busticket");

            migrationBuilder.DropTable(
                name: "bus_schedules",
                schema: "busticket");

            migrationBuilder.DropTable(
                name: "tickets",
                schema: "busticket");

            migrationBuilder.DropTable(
                name: "passengers",
                schema: "busticket");

            migrationBuilder.DropTable(
                name: "seats",
                schema: "busticket");
        }
    }
}
