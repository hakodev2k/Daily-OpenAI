DROP SCHEMA IF EXISTS lab CASCADE;
CREATE SCHEMA lab;

CREATE TABLE lab.inspection_slots (
    slot_id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    depot_id integer NOT NULL,
    vehicle_bay_id integer NULL,
    starts_at timestamptz NOT NULL,
    inspector_code text NOT NULL,
    CONSTRAINT uq_inspection_slot UNIQUE (depot_id, vehicle_bay_id, starts_at)
);

INSERT INTO lab.inspection_slots (depot_id, vehicle_bay_id, starts_at, inspector_code)
VALUES
    (42, NULL, '2026-10-29 08:00:00+00', 'MOBILE-A'),
    (42, 7,    '2026-10-29 08:00:00+00', 'BAY-7-A'),
    (42, 7,    '2026-10-29 09:00:00+00', 'BAY-7-B');
