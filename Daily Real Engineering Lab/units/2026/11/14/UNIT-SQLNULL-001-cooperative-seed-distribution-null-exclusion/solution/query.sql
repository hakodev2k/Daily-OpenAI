SELECT h.id, h.name
FROM households AS h
WHERE h.eligible = 1
  AND NOT EXISTS (
    SELECT 1
    FROM allocations AS a
    WHERE a.household_id = h.id
  )
ORDER BY h.id;
