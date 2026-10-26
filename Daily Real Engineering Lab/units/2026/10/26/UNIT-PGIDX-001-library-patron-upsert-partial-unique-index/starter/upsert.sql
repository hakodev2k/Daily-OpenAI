-- Bản nhập thẻ từ hệ thống đối tác, chạy lại phải an toàn.
INSERT INTO patron_cards (branch_id, external_card, holder_name)
VALUES ('branch-a', 'CARD-204', 'Mai')
ON CONFLICT (branch_id, external_card)
DO UPDATE SET holder_name = EXCLUDED.holder_name;
