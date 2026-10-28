\set ON_ERROR_STOP on
BEGIN;

DO $verify$
DECLARE row_count integer;
BEGIN
    BEGIN
        INSERT INTO lab.inspection_slots (depot_id, vehicle_bay_id, starts_at, inspector_code)
        VALUES (42, NULL, '2026-10-29 08:00:00+00', 'MOBILE-DUP');
        RAISE EXCEPTION 'VERIFY_FAIL: conflicting mobile slot was accepted';
    EXCEPTION WHEN unique_violation THEN
        RAISE NOTICE 'PASS: conflicting mobile slot rejected';
    END;

    BEGIN
        INSERT INTO lab.inspection_slots (depot_id, vehicle_bay_id, starts_at, inspector_code)
        VALUES (42, 7, '2026-10-29 08:00:00+00', 'BAY-DUP');
        RAISE EXCEPTION 'VERIFY_FAIL: conflicting fixed-bay slot was accepted';
    EXCEPTION WHEN unique_violation THEN
        RAISE NOTICE 'PASS: conflicting fixed-bay slot rejected';
    END;

    INSERT INTO lab.inspection_slots (depot_id, vehicle_bay_id, starts_at, inspector_code)
    VALUES
        (42, NULL, '2026-10-29 09:00:00+00', 'MOBILE-LATER'),
        (43, NULL, '2026-10-29 08:00:00+00', 'OTHER-DEPOT'),
        (42, 8,    '2026-10-29 08:00:00+00', 'OTHER-BAY');

    SELECT count(*) INTO row_count FROM lab.inspection_slots;
    IF row_count <> 6 THEN
        RAISE EXCEPTION 'VERIFY_FAIL: expected 6 rows including three legal additions, found %', row_count;
    END IF;

    RAISE NOTICE 'PASS: three valid non-conflicting slots accepted';
END
$verify$;

ROLLBACK;
