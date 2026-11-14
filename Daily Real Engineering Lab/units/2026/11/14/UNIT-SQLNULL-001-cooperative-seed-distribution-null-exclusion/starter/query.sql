SELECT h.id, h.name
FROM households AS h
WHERE h.eligible = 1
  AND h.id NOT IN (SELECT a.household_id FROM allocations AS a)
ORDER BY h.id;
