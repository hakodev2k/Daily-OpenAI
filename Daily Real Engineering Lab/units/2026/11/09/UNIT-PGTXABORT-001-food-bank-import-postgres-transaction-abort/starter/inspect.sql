SELECT donation_id,amount_cents FROM donations ORDER BY donation_id;
SELECT COUNT(*) AS total, COALESCE(SUM(amount_cents),0) AS total_cents FROM donations;
