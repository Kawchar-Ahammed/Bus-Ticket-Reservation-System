-- Simple seed data for testing the API
-- Delete existing data
DELETE FROM busticket.bus_schedules;
DELETE FROM busticket.seats;
DELETE FROM busticket.buses;
DELETE FROM busticket.routes;
DELETE FROM busticket.companies;

-- Insert 2 Companies
INSERT INTO busticket.companies (id, name, description, contact_number, email, is_active, created_at, updated_at)
VALUES 
    ('11111111-1111-1111-1111-111111111111'::uuid, 'Green Line Paribahan', 'Premium bus service', '+8801711111111', 'greenline@example.com', true, NOW(), NOW()),
    ('22222222-2222-2222-2222-222222222222'::uuid, 'Hanif Enterprise', 'Trusted transport service', '+8801722222222', 'hanif@example.com', true, NOW(), NOW());

-- Insert 2 Buses
INSERT INTO busticket.buses (id, bus_number, bus_name, company_id, total_seats, description, is_active, created_at, updated_at)
VALUES 
    ('33333333-3333-3333-3333-333333333333'::uuid, 'DHA-GA-11-2345', 'AC Sleeper', '11111111-1111-1111-1111-111111111111'::uuid, 40, 'AC sleeper coach', true, NOW(), NOW()),
    ('44444444-4444-4444-4444-444444444444'::uuid, 'DHA-KA-22-6789', 'AC Chair Coach', '22222222-2222-2222-2222-222222222222'::uuid, 45, 'AC chair coach', true, NOW(), NOW());

-- Insert 3 Routes
INSERT INTO busticket.routes (id, from_city, from_area, to_city, to_area, distance_in_km, estimated_duration, is_active, created_at, updated_at)
VALUES 
    ('55555555-5555-5555-5555-555555555555'::uuid, 'Dhaka', 'Gabtali', 'Chittagong', 'Oxygen', 264.00, '06:00:00', true, NOW(), NOW()),
    ('66666666-6666-6666-6666-666666666666'::uuid, 'Dhaka', 'Kalyanpur', 'Cox''s Bazar', 'Kolatoli', 397.00, '08:00:00', true, NOW(), NOW()),
    ('77777777-7777-7777-7777-777777777777'::uuid, 'Dhaka', 'Mohakhali', 'Sylhet', 'Ambarkhana', 244.00, '05:00:00', true, NOW(), NOW());

-- Insert 14 Bus Schedules (next 7 days, 2 schedules per day)
DO $$
DECLARE
    day_offset INTEGER;
    schedule_date DATE;
BEGIN
    FOR day_offset IN 0..6 LOOP
        schedule_date := CURRENT_DATE + day_offset;
        
        -- Schedule 1: Dhaka to Chittagong
        INSERT INTO busticket.bus_schedules (
            id, bus_id, route_id, journey_date, departure_time, arrival_time,
            fare_amount, fare_currency, boarding_city, boarding_area, boarding_street,
            dropping_city, dropping_area, dropping_street, is_active, created_at, updated_at
        )
        VALUES (
            gen_random_uuid(),
            '33333333-3333-3333-3333-333333333333'::uuid,
            '55555555-5555-5555-5555-555555555555'::uuid,
            schedule_date,
            '22:00:00',
            '04:00:00',
            1200.00,
            'BDT',
            'Dhaka', 'Gabtali', 'Counter Road',
            'Chittagong', 'GEC Circle', 'Station Road',
            true, NOW(), NOW()
        );
        
        -- Schedule 2: Dhaka to Cox's Bazar
        INSERT INTO busticket.bus_schedules (
            id, bus_id, route_id, journey_date, departure_time, arrival_time,
            fare_amount, fare_currency, boarding_city, boarding_area, boarding_street,
            dropping_city, dropping_area, dropping_street, is_active, created_at, updated_at
        )
        VALUES (
            gen_random_uuid(),
            '44444444-4444-4444-4444-444444444444'::uuid,
            '66666666-6666-6666-6666-666666666666'::uuid,
            schedule_date,
            '21:30:00',
            '05:30:00',
            1800.00,
            'BDT',
            'Dhaka', 'Kalyanpur', 'Terminal Road',
            'Cox''s Bazar', 'Kolatoli', 'Beach Road',
            true, NOW(), NOW()
        );
    END LOOP;
END $$;

-- Generate seats for both buses
DO $$
DECLARE
    bus RECORD;
    seat_num INTEGER;
    row_num VARCHAR(2);
    col_char CHAR(1);
BEGIN
    FOR bus IN SELECT id, total_seats FROM busticket.buses LOOP
        FOR seat_num IN 1..bus.total_seats LOOP
            row_num := LPAD(((seat_num - 1) / 4 + 1)::TEXT, 2, '0');
            col_char := CHR(65 + ((seat_num - 1) % 4)); -- A, B, C, D
            
            INSERT INTO busticket.seats (id, bus_id, seat_number, row_number, column_number, is_active, created_at, updated_at)
            VALUES (
                gen_random_uuid(),
                bus.id,
                row_num || col_char,
                row_num,
                col_char,
                true,
                NOW(),
                NOW()
            );
        END LOOP;
    END LOOP;
END $$;

-- Verify
SELECT 'Companies:' as type, COUNT(*)::TEXT as count FROM busticket.companies
UNION ALL
SELECT 'Buses:', COUNT(*)::TEXT FROM busticket.buses
UNION ALL
SELECT 'Routes:', COUNT(*)::TEXT FROM busticket.routes
UNION ALL
SELECT 'Schedules:', COUNT(*)::TEXT FROM busticket.bus_schedules
UNION ALL
SELECT 'Seats:', COUNT(*)::TEXT FROM busticket.seats;
