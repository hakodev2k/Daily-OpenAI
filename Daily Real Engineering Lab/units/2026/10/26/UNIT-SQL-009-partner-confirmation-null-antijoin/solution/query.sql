-- Reference Solution — inspect only after attempting your own fix.
DROP TABLE IF EXISTS lab.missing_enrollments;
CREATE TABLE lab.missing_enrollments AS
SELECT e.tenant_id, e.enrollment_id
FROM lab.enrollments AS e
WHERE e.tenant_id = 'A'
  AND e.status = 'ACTIVE'
  AND NOT EXISTS (
    SELECT 1
    FROM lab.partner_confirmations AS c
    WHERE c.tenant_id = e.tenant_id
      AND c.batch_id = 42
      AND c.enrollment_id = e.enrollment_id
  );
SELECT * FROM lab.missing_enrollments ORDER BY enrollment_id;
