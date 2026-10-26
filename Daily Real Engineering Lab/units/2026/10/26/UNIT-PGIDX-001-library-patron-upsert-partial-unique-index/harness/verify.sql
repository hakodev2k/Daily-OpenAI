\set ON_ERROR_STOP on
TRUNCATE TABLE patron_cards RESTART IDENTITY;
INSERT INTO patron_cards(branch_id, external_card, holder_name, deleted_at)
VALUES ('branch-a','CARD-204','Former Member','2025-01-01T00:00:00Z'),
       ('branch-b','CARD-204','Other Branch',NULL);
-- First import must insert a new active row without touching the tombstone.
\i /lab/starter/upsert.sql
DO $check$
BEGIN
  IF (SELECT count(*) FROM patron_cards WHERE branch_id='branch-a' AND external_card='CARD-204' AND deleted_at IS NULL) <> 1 THEN
    RAISE EXCEPTION 'Expected exactly one active branch-a card after first import';
  END IF;
  IF (SELECT count(*) FROM patron_cards WHERE branch_id='branch-a' AND deleted_at IS NOT NULL AND holder_name='Former Member') <> 1 THEN
    RAISE EXCEPTION 'Soft-deleted history was modified';
  END IF;
  IF (SELECT count(*) FROM patron_cards WHERE branch_id='branch-b' AND holder_name='Other Branch') <> 1 THEN
    RAISE EXCEPTION 'Cross-branch card was modified';
  END IF;
END
$check$;
-- Second import must update the active row, not create a duplicate.
UPDATE patron_cards SET holder_name='Stale Name'
 WHERE branch_id='branch-a' AND external_card='CARD-204' AND deleted_at IS NULL;
\i /lab/starter/upsert.sql
DO $check$
BEGIN
  IF (SELECT count(*) FROM patron_cards WHERE branch_id='branch-a' AND external_card='CARD-204' AND deleted_at IS NULL) <> 1 THEN
    RAISE EXCEPTION 'Repeated import created duplicate active card';
  END IF;
  IF (SELECT count(*) FROM patron_cards WHERE branch_id='branch-a' AND external_card='CARD-204' AND deleted_at IS NULL AND holder_name='Mai') <> 1 THEN
    RAISE EXCEPTION 'Repeated import failed to update active holder';
  END IF;
  IF (SELECT count(*) FROM patron_cards) <> 3 THEN
    RAISE EXCEPTION 'Row count changed unexpectedly';
  END IF;
END
$check$;
\echo VERIFY PASS
