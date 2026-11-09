BEGIN;
SAVEPOINT row_101;
INSERT INTO donations(donation_id,amount_cents) VALUES ('D-101',2500);
RELEASE SAVEPOINT row_101;
SAVEPOINT row_100;
-- Known duplicate from source; use conflict-aware handling at row boundary.
INSERT INTO donations(donation_id,amount_cents) VALUES ('D-100',1200)
ON CONFLICT (donation_id) DO NOTHING;
RELEASE SAVEPOINT row_100;
SAVEPOINT row_102;
INSERT INTO donations(donation_id,amount_cents) VALUES ('D-102',3100);
RELEASE SAVEPOINT row_102;
COMMIT;
