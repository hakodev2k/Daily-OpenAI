DO $$
DECLARE actual text[];
DECLARE total integer;
BEGIN
  SELECT array_agg(enrollment_id ORDER BY enrollment_id)
    INTO actual
  FROM lab.missing_enrollments WHERE tenant_id = 'A';
  IF actual IS DISTINCT FROM ARRAY['E-102','E-103']::text[] THEN
    RAISE EXCEPTION 'Wrong missing set for tenant A / batch 42: %', actual;
  END IF;
  SELECT count(*) INTO total FROM lab.missing_enrollments;
  IF total <> 2 THEN
    RAISE EXCEPTION 'Wrong total count (tenant, status or duplicates): %', total;
  END IF;
  IF EXISTS (
    SELECT 1 FROM lab.missing_enrollments AS m
    JOIN lab.partner_confirmations AS c
      ON c.tenant_id = m.tenant_id
     AND c.batch_id = 42
     AND c.enrollment_id = m.enrollment_id
  ) THEN
    RAISE EXCEPTION 'Confirmed enrollment incorrectly classified as missing';
  END IF;
  RAISE NOTICE 'VERIFIED: exactly E-102 and E-103 missing for tenant A / batch 42.';
END $$;
