CREATE TABLE IF NOT EXISTS patron_cards (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    branch_id TEXT NOT NULL,
    external_card TEXT NOT NULL,
    holder_name TEXT NOT NULL,
    deleted_at TIMESTAMPTZ NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS ux_patron_cards_active
    ON patron_cards (branch_id, external_card)
    WHERE deleted_at IS NULL;
