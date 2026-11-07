-- Lab-only fixture: never run against another database.
DROP TABLE IF EXISTS volunteer_shift_assignments;
CREATE TABLE volunteer_shift_assignments (
 volunteer_id integer PRIMARY KEY,
 shift_date date NOT NULL,
 status text NOT NULL,
 last_seen_at timestamptz NULL
) WITH (autovacuum_enabled = false);
INSERT INTO volunteer_shift_assignments (volunteer_id,shift_date,status)
SELECT g,
 CASE WHEN g%3=0 THEN DATE '2026-10-29' ELSE DATE '2026-10-30' END,
 CASE WHEN g%2=0 THEN 'CONFIRMED' ELSE 'WAITLIST' END
FROM generate_series(1,30000) AS g;
CREATE INDEX idx_volunteer_shift_cover
 ON volunteer_shift_assignments(shift_date,status) INCLUDE (volunteer_id);
VACUUM (ANALYZE) volunteer_shift_assignments;
