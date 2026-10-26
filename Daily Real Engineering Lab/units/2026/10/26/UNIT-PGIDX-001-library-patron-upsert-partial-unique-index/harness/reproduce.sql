\set ON_ERROR_STOP on
TRUNCATE TABLE patron_cards RESTART IDENTITY;
INSERT INTO patron_cards(branch_id, external_card, holder_name, deleted_at)
VALUES ('branch-a','CARD-204','Former Member','2025-01-01T00:00:00Z');
\i /lab/starter/upsert.sql
