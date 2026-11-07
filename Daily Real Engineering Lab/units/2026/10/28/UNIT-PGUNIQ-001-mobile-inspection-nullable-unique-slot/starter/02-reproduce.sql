\set ON_ERROR_STOP on
-- The partner requests another mobile inspection for the same depot and time.
INSERT INTO lab.inspection_slots (depot_id, vehicle_bay_id, starts_at, inspector_code)
VALUES (42, NULL, '2026-10-29 08:00:00+00', 'MOBILE-B');

SELECT slot_id, depot_id, vehicle_bay_id, starts_at, inspector_code
FROM lab.inspection_slots
ORDER BY slot_id;

DO $repro$
DECLARE duplicate_count integer;
BEGIN
    SELECT count(*) INTO duplicate_count
    FROM lab.inspection_slots
    WHERE depot_id = 42
      AND vehicle_bay_id IS NULL
      AND starts_at = '2026-10-29 08:00:00+00';

    IF duplicate_count <> 2 THEN
        RAISE EXCEPTION 'REPRO FAILED: expected 2 mobile reservations, found %', duplicate_count;
    END IF;

    BEGIN
        INSERT INTO lab.inspection_slots (depot_id, vehicle_bay_id, starts_at, inspector_code)
        VALUES (42, 7, '2026-10-29 08:00:00+00', 'BAY-7-DUP');
        RAISE EXCEPTION 'REPRO FAILED: fixed-bay duplicate was accepted';
    EXCEPTION WHEN unique_violation THEN
        RAISE NOTICE 'Fixed-bay duplicate correctly rejected';
    END;

    RAISE NOTICE 'REPRODUCED: two mobile reservations at depot 42, 08:00 UTC';
END
$repro$;
