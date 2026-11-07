USE RealEngineeringLabSql001;
GO
SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
BEGIN TRANSACTION;

DECLARE @CapturedCount int;
DECLARE @CapturedTotal decimal(12,2);

SELECT @CapturedCount = COUNT(*)
FROM dbo.Payments
WHERE Status = 'Captured';

WAITFOR DELAY '00:00:03';

SELECT @CapturedTotal = SUM(Amount)
FROM dbo.Payments
WHERE Status = 'Captured';

SELECT @CapturedCount AS CapturedCount, @CapturedTotal AS CapturedTotal;
COMMIT;