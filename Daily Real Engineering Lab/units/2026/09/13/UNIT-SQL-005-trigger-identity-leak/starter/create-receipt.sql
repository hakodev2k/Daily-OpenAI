SET NOCOUNT ON;

DECLARE @Reference nvarchar(50) = N'RCV-2026-0913';
DECLARE @Quantity int = 12;

INSERT INTO dbo.Receipts(Reference, Quantity)
VALUES (@Reference, @Quantity);

-- Investigation note:
-- Verify whether this value is scoped to the business INSERT statement
-- or can be affected by other identity-generating work in the same session.
DECLARE @ReturnedId int = CONVERT(int, @@IDENTITY);

SELECT
    @ReturnedId AS ReturnedId,
    (SELECT MAX(Id) FROM dbo.Receipts WHERE Reference = @Reference) AS ActualReceiptId,
    (SELECT MAX(Id) FROM dbo.AuditLog WHERE EntityType = N'Receipt') AS LatestAuditId;

IF @ReturnedId = (SELECT MAX(Id) FROM dbo.Receipts WHERE Reference = @Reference)
    RETURN 0;

THROW 51000, 'Returned ID does not identify the inserted receipt.', 1;
