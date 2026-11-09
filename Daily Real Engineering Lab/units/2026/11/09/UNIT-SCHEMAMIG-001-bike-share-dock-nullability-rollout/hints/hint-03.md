# Hint 3

Xem xét expand → deploy compatible writers → backfill/quarantine → verify → contract. Với PostgreSQL, `CHECK ... NOT VALID` và `VALIDATE CONSTRAINT` có thể giúp tách các giai đoạn, nhưng phải xác định chính xác lúc nào old writer ngừng tương thích.