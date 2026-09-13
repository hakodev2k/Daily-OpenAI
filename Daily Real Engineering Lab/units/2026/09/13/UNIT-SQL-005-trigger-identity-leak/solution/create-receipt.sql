SET NOCOUNT ON;

DECLARE @Reference nvarchar(50) = N'RCV-2026-0913';
DECLARE @Quantity int = 12;
DECLARE @Inserted table (Id int NOT NULL);

INSERT INTO dbo.Receipts(Reference, Quantity)
OUTPUT inserted.Id INTO @Inserted(Id)
VALUES (@Reference, @Quantity);

DECLARE @ReturnedId int = (SELECT TOP (1) Id FROM @Inserted);

SELECT
    @ReturnedId AS ReturnedId,
    (SELECT MAX(Id) FROM dbo.Receipts WHERE Reference = @Reference) AS ActualReceiptId,
    (SELECT MAX(Id) FROM dbo.AuditLog WHERE EntityType = N'Receipt') AS LatestAuditId;

IF @ReturnedId <> (SELECT MAX(Id) FROM dbo.Receipts WHERE Reference = @Reference)
    THROW 51000, 'Returned ID does not identify the inserted receipt.', 1;
