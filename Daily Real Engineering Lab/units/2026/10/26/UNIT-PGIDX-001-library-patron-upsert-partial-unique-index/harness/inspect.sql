SELECT indexname, indexdef FROM pg_indexes
WHERE schemaname='public' AND tablename='patron_cards' ORDER BY indexname;
SELECT branch_id, external_card, holder_name, deleted_at
FROM patron_cards ORDER BY branch_id, id;
