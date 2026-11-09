-- Import file rows; accounting requires all valid unique donations to persist.
BEGIN;
INSERT INTO donations(donation_id,amount_cents) VALUES ('D-101',2500);
INSERT INTO donations(donation_id,amount_cents) VALUES ('D-100',1200);
INSERT INTO donations(donation_id,amount_cents) VALUES ('D-102',3100);
COMMIT;
