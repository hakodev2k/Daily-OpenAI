DO $$
DECLARE row_count integer;
BEGIN
  SELECT count(*) INTO row_count FROM lab.missing_enrollments;
  IF row_count <> 0 THEN
    RAISE EXCEPTION 'Baseline expected zero missing rows, found %', row_count;
  END IF;
  IF (SELECT count(*) FROM lab.enrollments WHERE tenant_id='A' AND status='ACTIVE') <> 4 THEN
    RAISE EXCEPTION 'Seed mismatch: expected four ACTIVE A enrollments';
  END IF;
  IF (SELECT count(*) FROM lab.partner_confirmations
      WHERE tenant_id='A' AND batch_id=42 AND enrollment_id IS NULL) <> 1 THEN
    RAISE EXCEPTION 'Seed mismatch: expected one missing identifier';
  END IF;
  RAISE NOTICE 'REPRODUCED: reconciliation returned zero rows despite active unconfirmed enrollments.';
END $$;
