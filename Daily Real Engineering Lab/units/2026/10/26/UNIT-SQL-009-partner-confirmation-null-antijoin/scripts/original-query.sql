-- Baseline bất biến cho reproduce; không sửa file này.
DROP TABLE IF EXISTS lab.missing_enrollments;
CREATE TABLE lab.missing_enrollments AS
SELECT e.tenant_id, e.enrollment_id
FROM lab.enrollments AS e
WHERE e.tenant_id = 'A'
  AND e.status = 'ACTIVE'
  AND e.enrollment_id NOT IN (
    SELECT c.enrollment_id
    FROM lab.partner_confirmations AS c
    WHERE c.tenant_id = 'A'
      AND c.batch_id = 42
  );
SELECT * FROM lab.missing_enrollments ORDER BY enrollment_id;
