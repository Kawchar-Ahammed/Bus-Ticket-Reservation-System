-- =====================================================
-- Bus Ticket Reservation System - Complete Database Schema
-- DDL (Data Definition Language) - Table Creation
-- Database: PostgreSQL 16
-- Date: October 25, 2025
-- =====================================================

-- Drop existing tables if they exist (in correct order due to foreign keys)
DROP TABLE IF EXISTS transactions CASCADE;
DROP TABLE IF EXISTS payments CASCADE;
DROP TABLE IF EXISTS tickets CASCADE;
DROP TABLE IF EXISTS seats CASCADE;
DROP TABLE IF EXISTS passengers CASCADE;
DROP TABLE IF EXISTS bus_schedules CASCADE;
DROP TABLE IF EXISTS routes CASCADE;
DROP TABLE IF EXISTS buses CASCADE;
DROP TABLE IF EXISTS companies CASCADE;
DROP TABLE IF EXISTS admin_users CASCADE;

-- =====================================================
-- Table: companies
-- Description: Bus operating companies
-- =====================================================
CREATE TABLE companies (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(200) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    phone_number VARCHAR(20),
    email VARCHAR(100),
    address_city VARCHAR(100),
    address_area VARCHAR(100),
    address_street VARCHAR(200),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Index for company lookup
CREATE INDEX idx_companies_code ON companies(code);
CREATE INDEX idx_companies_is_active ON companies(is_active);

-- =====================================================
-- Table: buses
-- Description: Bus vehicles with seating configuration
-- =====================================================
CREATE TABLE buses (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    company_id UUID NOT NULL REFERENCES companies(id) ON DELETE CASCADE,
    bus_name VARCHAR(100) NOT NULL,
    bus_number VARCHAR(50) NOT NULL UNIQUE,
    total_seats INTEGER NOT NULL CHECK (total_seats > 0),
    bus_type VARCHAR(50), -- AC, Non-AC, Sleeper, etc.
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for bus lookup
CREATE INDEX idx_buses_company_id ON buses(company_id);
CREATE INDEX idx_buses_bus_number ON buses(bus_number);
CREATE INDEX idx_buses_is_active ON buses(is_active);

-- =====================================================
-- Table: routes
-- Description: Routes between cities
-- =====================================================
CREATE TABLE routes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    route_name VARCHAR(200) NOT NULL,
    from_city VARCHAR(100) NOT NULL,
    from_area VARCHAR(100),
    from_street VARCHAR(200),
    to_city VARCHAR(100) NOT NULL,
    to_area VARCHAR(100),
    to_street VARCHAR(200),
    distance_km DECIMAL(10, 2),
    duration_minutes INTEGER,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for route lookup
CREATE INDEX idx_routes_from_city ON routes(from_city);
CREATE INDEX idx_routes_to_city ON routes(to_city);
CREATE INDEX idx_routes_from_to ON routes(from_city, to_city);
CREATE INDEX idx_routes_is_active ON routes(is_active);

-- =====================================================
-- Table: bus_schedules
-- Description: Bus schedules for specific routes and dates
-- =====================================================
CREATE TABLE bus_schedules (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    bus_id UUID NOT NULL REFERENCES buses(id) ON DELETE CASCADE,
    route_id UUID NOT NULL REFERENCES routes(id) ON DELETE CASCADE,
    journey_date DATE NOT NULL,
    departure_time TIME NOT NULL,
    arrival_time TIME NOT NULL,
    fare_amount DECIMAL(18, 2) NOT NULL CHECK (fare_amount >= 0),
    fare_currency VARCHAR(10) NOT NULL DEFAULT 'BDT',
    boarding_point_city VARCHAR(100),
    boarding_point_area VARCHAR(100),
    boarding_point_street VARCHAR(200),
    dropping_point_city VARCHAR(100),
    dropping_point_area VARCHAR(100),
    dropping_point_street VARCHAR(200),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for schedule lookup
CREATE INDEX idx_bus_schedules_bus_id ON bus_schedules(bus_id);
CREATE INDEX idx_bus_schedules_route_id ON bus_schedules(route_id);
CREATE INDEX idx_bus_schedules_journey_date ON bus_schedules(journey_date);
CREATE INDEX idx_bus_schedules_route_date ON bus_schedules(route_id, journey_date);
CREATE INDEX idx_bus_schedules_is_active ON bus_schedules(is_active);

-- =====================================================
-- Table: seats
-- Description: Individual seats for each bus schedule
-- =====================================================
CREATE TABLE seats (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    bus_schedule_id UUID NOT NULL REFERENCES bus_schedules(id) ON DELETE CASCADE,
    seat_number VARCHAR(10) NOT NULL,
    "row" INTEGER NOT NULL CHECK ("row" > 0),
    "column" INTEGER NOT NULL CHECK ("column" > 0),
    status VARCHAR(20) NOT NULL DEFAULT 'Available' CHECK (status IN ('Available', 'Booked', 'Sold', 'Blocked')),
    ticket_id UUID,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT unique_seat_per_schedule UNIQUE (bus_schedule_id, seat_number)
);

-- Indexes for seat lookup
CREATE INDEX idx_seats_bus_schedule_id ON seats(bus_schedule_id);
CREATE INDEX idx_seats_status ON seats(status);
CREATE INDEX idx_seats_ticket_id ON seats(ticket_id);
CREATE INDEX idx_seats_schedule_status ON seats(bus_schedule_id, status);

-- =====================================================
-- Table: passengers
-- Description: Passenger information
-- =====================================================
CREATE TABLE passengers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(200) NOT NULL,
    phone_number VARCHAR(20) NOT NULL,
    email VARCHAR(100),
    age INTEGER CHECK (age > 0 AND age <= 150),
    gender VARCHAR(20) CHECK (gender IN ('Male', 'Female', 'Other')),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for passenger lookup
CREATE INDEX idx_passengers_phone_number ON passengers(phone_number);
CREATE INDEX idx_passengers_email ON passengers(email);

-- =====================================================
-- Table: tickets
-- Description: Booking tickets
-- =====================================================
CREATE TABLE tickets (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ticket_number VARCHAR(50) NOT NULL UNIQUE,
    bus_schedule_id UUID NOT NULL REFERENCES bus_schedules(id) ON DELETE RESTRICT,
    passenger_id UUID NOT NULL REFERENCES passengers(id) ON DELETE RESTRICT,
    seat_id UUID NOT NULL REFERENCES seats(id) ON DELETE RESTRICT,
    booking_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    journey_date DATE NOT NULL,
    boarding_point_city VARCHAR(100),
    boarding_point_area VARCHAR(100),
    boarding_point_street VARCHAR(200),
    dropping_point_city VARCHAR(100),
    dropping_point_area VARCHAR(100),
    dropping_point_street VARCHAR(200),
    fare_amount DECIMAL(18, 2) NOT NULL CHECK (fare_amount >= 0),
    fare_currency VARCHAR(10) NOT NULL DEFAULT 'BDT',
    status VARCHAR(20) NOT NULL DEFAULT 'Confirmed' CHECK (status IN ('Confirmed', 'Cancelled', 'Refunded')),
    cancellation_reason TEXT,
    cancelled_at TIMESTAMP,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for ticket lookup
CREATE INDEX idx_tickets_ticket_number ON tickets(ticket_number);
CREATE INDEX idx_tickets_bus_schedule_id ON tickets(bus_schedule_id);
CREATE INDEX idx_tickets_passenger_id ON tickets(passenger_id);
CREATE INDEX idx_tickets_seat_id ON tickets(seat_id);
CREATE INDEX idx_tickets_booking_date ON tickets(booking_date);
CREATE INDEX idx_tickets_journey_date ON tickets(journey_date);
CREATE INDEX idx_tickets_status ON tickets(status);

-- =====================================================
-- Table: admin_users
-- Description: Admin users for system management
-- =====================================================
CREATE TABLE admin_users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(100) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(500) NOT NULL,
    full_name VARCHAR(200),
    role VARCHAR(50) NOT NULL DEFAULT 'Admin' CHECK (role IN ('SuperAdmin', 'Admin', 'Operator')),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    last_login TIMESTAMP,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for admin user lookup
CREATE INDEX idx_admin_users_username ON admin_users(username);
CREATE INDEX idx_admin_users_email ON admin_users(email);
CREATE INDEX idx_admin_users_is_active ON admin_users(is_active);

-- =====================================================
-- Table: payments
-- Description: Payment transactions
-- =====================================================
CREATE TABLE payments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ticket_id UUID NOT NULL REFERENCES tickets(id) ON DELETE RESTRICT,
    transaction_number VARCHAR(100) NOT NULL UNIQUE,
    payment_method VARCHAR(50) NOT NULL CHECK (payment_method IN ('Card', 'Cash', 'BankTransfer', 'BKash', 'Nagad', 'Rocket')),
    payment_gateway VARCHAR(50) CHECK (payment_gateway IN ('Manual', 'SSLCommerz', 'Stripe', 'BKash', 'Nagad', 'Rocket', 'Mock')),
    amount DECIMAL(18, 2) NOT NULL CHECK (amount >= 0),
    currency VARCHAR(10) NOT NULL DEFAULT 'BDT',
    status VARCHAR(50) NOT NULL DEFAULT 'Pending' CHECK (status IN ('Pending', 'Processing', 'Completed', 'Failed', 'Refunded')),
    gateway_transaction_id VARCHAR(200),
    gateway_response TEXT,
    paid_at TIMESTAMP,
    refunded_at TIMESTAMP,
    refund_reason TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for payment lookup
CREATE INDEX idx_payments_ticket_id ON payments(ticket_id);
CREATE INDEX idx_payments_transaction_number ON payments(transaction_number);
CREATE INDEX idx_payments_status ON payments(status);
CREATE INDEX idx_payments_payment_method ON payments(payment_method);
CREATE INDEX idx_payments_paid_at ON payments(paid_at);

-- =====================================================
-- Table: transactions (Audit Log)
-- Description: Transaction audit trail
-- =====================================================
CREATE TABLE transactions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ticket_id UUID REFERENCES tickets(id) ON DELETE SET NULL,
    payment_id UUID REFERENCES payments(id) ON DELETE SET NULL,
    transaction_type VARCHAR(50) NOT NULL CHECK (transaction_type IN ('Booking', 'Payment', 'Cancellation', 'Refund')),
    amount DECIMAL(18, 2) NOT NULL,
    currency VARCHAR(10) NOT NULL DEFAULT 'BDT',
    description TEXT,
    metadata JSONB,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Indexes for transaction lookup
CREATE INDEX idx_transactions_ticket_id ON transactions(ticket_id);
CREATE INDEX idx_transactions_payment_id ON transactions(payment_id);
CREATE INDEX idx_transactions_transaction_type ON transactions(transaction_type);
CREATE INDEX idx_transactions_created_at ON transactions(created_at);

-- =====================================================
-- Add Foreign Key for seats.ticket_id (after tickets table is created)
-- =====================================================
ALTER TABLE seats ADD CONSTRAINT fk_seats_ticket_id 
    FOREIGN KEY (ticket_id) REFERENCES tickets(id) ON DELETE SET NULL;

-- =====================================================
-- Create Update Timestamp Function
-- =====================================================
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- Create Update Timestamp Triggers
-- =====================================================
CREATE TRIGGER update_companies_updated_at BEFORE UPDATE ON companies
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_buses_updated_at BEFORE UPDATE ON buses
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_routes_updated_at BEFORE UPDATE ON routes
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_bus_schedules_updated_at BEFORE UPDATE ON bus_schedules
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_seats_updated_at BEFORE UPDATE ON seats
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_passengers_updated_at BEFORE UPDATE ON passengers
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_tickets_updated_at BEFORE UPDATE ON tickets
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_admin_users_updated_at BEFORE UPDATE ON admin_users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_payments_updated_at BEFORE UPDATE ON payments
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- =====================================================
-- Create Views for Common Queries
-- =====================================================

-- View: Available buses summary
CREATE OR REPLACE VIEW vw_available_buses AS
SELECT 
    bs.id AS bus_schedule_id,
    c.name AS company_name,
    b.bus_name,
    b.bus_number,
    r.from_city,
    r.to_city,
    bs.journey_date,
    bs.departure_time,
    bs.arrival_time,
    bs.fare_amount,
    bs.fare_currency,
    b.total_seats,
    COUNT(CASE WHEN s.status IN ('Booked', 'Sold') THEN 1 END) AS booked_seats,
    b.total_seats - COUNT(CASE WHEN s.status IN ('Booked', 'Sold') THEN 1 END) AS available_seats
FROM bus_schedules bs
JOIN buses b ON bs.bus_id = b.id
JOIN companies c ON b.company_id = c.id
JOIN routes r ON bs.route_id = r.id
LEFT JOIN seats s ON s.bus_schedule_id = bs.id
WHERE bs.is_active = TRUE
GROUP BY bs.id, c.name, b.bus_name, b.bus_number, r.from_city, r.to_city, 
         bs.journey_date, bs.departure_time, bs.arrival_time, bs.fare_amount, 
         bs.fare_currency, b.total_seats;

-- View: Booking summary
CREATE OR REPLACE VIEW vw_booking_summary AS
SELECT 
    t.id AS ticket_id,
    t.ticket_number,
    t.booking_date,
    t.journey_date,
    t.status AS ticket_status,
    p.name AS passenger_name,
    p.phone_number AS passenger_phone,
    c.name AS company_name,
    b.bus_name,
    b.bus_number,
    r.from_city,
    r.to_city,
    s.seat_number,
    t.fare_amount,
    t.fare_currency,
    CASE 
        WHEN pay.status = 'Completed' THEN 'Paid'
        WHEN pay.status IN ('Pending', 'Processing') THEN 'Pending'
        ELSE 'Unpaid'
    END AS payment_status
FROM tickets t
JOIN passengers p ON t.passenger_id = p.id
JOIN bus_schedules bs ON t.bus_schedule_id = bs.id
JOIN buses b ON bs.bus_id = b.id
JOIN companies c ON b.company_id = c.id
JOIN routes r ON bs.route_id = r.id
JOIN seats s ON t.seat_id = s.id
LEFT JOIN payments pay ON pay.ticket_id = t.id;

-- =====================================================
-- Grant Permissions (Adjust as needed)
-- =====================================================
-- GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO your_app_user;
-- GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO your_app_user;
-- GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO your_app_user;

-- =====================================================
-- Schema Creation Complete
-- =====================================================
SELECT 'Database schema created successfully!' AS message;
