DROP TABLE IF EXISTS donations;
CREATE TABLE donations (
  donation_id text PRIMARY KEY,
  amount_cents bigint NOT NULL CHECK (amount_cents > 0),
  imported_at timestamptz NOT NULL DEFAULT now()
);
INSERT INTO donations(donation_id,amount_cents) VALUES ('D-100',1200);
