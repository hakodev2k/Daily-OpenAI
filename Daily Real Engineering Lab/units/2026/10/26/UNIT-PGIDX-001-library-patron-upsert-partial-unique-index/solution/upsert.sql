-- Index inference phải mô tả cùng phạm vi với uniqueness policy.
INSERT INTO patron_cards (branch_id, external_card, holder_name)
VALUES ('branch-a', 'CARD-204', 'Mai')
ON CONFLICT (branch_id, external_card) WHERE deleted_at IS NULL
DO UPDATE SET holder_name = EXCLUDED.holder_name;
