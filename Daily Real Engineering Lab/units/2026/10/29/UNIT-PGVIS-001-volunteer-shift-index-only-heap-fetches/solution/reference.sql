-- Reference only; learner must edit workspace/remediation.sql.
ALTER TABLE volunteer_shift_assignments SET (
  autovacuum_enabled = true,
  autovacuum_vacuum_scale_factor = 0.02,
  autovacuum_vacuum_threshold = 50
);
-- Deterministic immediate verification, not a substitute for ongoing maintenance.
VACUUM (ANALYZE) volunteer_shift_assignments;
