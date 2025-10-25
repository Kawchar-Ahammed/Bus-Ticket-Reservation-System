-- Generate seats for all bus schedules
-- This script creates seats for each bus schedule based on the bus's total_seats

DO $$
DECLARE
    schedule_rec RECORD;
    seat_num INT;
    row_num INT;
    col_num INT;
    seat_id UUID;
BEGIN
    -- Loop through all bus schedules
    FOR schedule_rec IN 
        SELECT bs.id as schedule_id, b.total_seats, b.id as bus_id
        FROM busticket.bus_schedules bs
        INNER JOIN busticket.buses b ON bs.bus_id = b.id
        WHERE bs.is_active = true
    LOOP
        -- Generate seats for this schedule
        FOR seat_num IN 1..schedule_rec.total_seats LOOP
            -- Calculate row and column (4 seats per row: 2 on left, 2 on right)
            row_num := ((seat_num - 1) / 4) + 1;
            col_num := ((seat_num - 1) % 4) + 1;
            
            -- Generate UUID for seat
            seat_id := gen_random_uuid();
            
            -- Insert seat
            INSERT INTO busticket.seats (
                id,
                bus_schedule_id,
                seat_number,
                row,
                "column",
                status,
                created_at,
                updated_at
            ) VALUES (
                seat_id,
                schedule_rec.schedule_id,
                'S' || LPAD(seat_num::TEXT, 2, '0'),  -- S01, S02, etc.
                row_num,
                col_num,
                1,  -- Available status (SeatStatus.Available = 1)
                NOW(),
                NOW()
            );
        END LOOP;
        
        RAISE NOTICE 'Generated % seats for schedule %', schedule_rec.total_seats, schedule_rec.schedule_id;
    END LOOP;
END $$;

-- Verify seats were created
SELECT 
    bs.id as schedule_id,
    b.bus_number,
    b.total_seats,
    COUNT(s.id) as seats_created
FROM busticket.bus_schedules bs
INNER JOIN busticket.buses b ON bs.bus_id = b.id
LEFT JOIN busticket.seats s ON bs.id = s.bus_schedule_id
GROUP BY bs.id, b.bus_number, b.total_seats
ORDER BY b.bus_number;
