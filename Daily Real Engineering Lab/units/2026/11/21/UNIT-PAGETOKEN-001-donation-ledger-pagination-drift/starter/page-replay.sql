DROP TABLE IF EXISTS #Donations;
CREATE TABLE #Donations (DonationId int NOT NULL PRIMARY KEY, CreatedAt datetime2(0) NOT NULL, Amount int NOT NULL);
INSERT INTO #Donations VALUES (101,'2026-11-20T09:00:00',100),(102,'2026-11-20T09:01:00',200),(103,'2026-11-20T09:02:00',300),(104,'2026-11-20T09:03:00',400);
-- Request 1
SELECT DonationId,CreatedAt FROM #Donations ORDER BY CreatedAt DESC,DonationId DESC OFFSET 0 ROWS FETCH NEXT 2 ROWS ONLY;
-- Concurrent insert
INSERT INTO #Donations VALUES (105,'2026-11-20T09:04:00',500);
-- Request 2
SELECT DonationId,CreatedAt FROM #Donations ORDER BY CreatedAt DESC,DonationId DESC OFFSET 2 ROWS FETCH NEXT 2 ROWS ONLY;
SELECT DonationId,CreatedAt FROM #Donations ORDER BY CreatedAt DESC,DonationId DESC;
