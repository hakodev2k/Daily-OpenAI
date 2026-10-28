-- A background metadata sync. The query predicates do not change.
UPDATE volunteer_shift_assignments
 SET last_seen_at=clock_timestamp()
 WHERE shift_date=DATE '2026-10-29' AND status='CONFIRMED';
