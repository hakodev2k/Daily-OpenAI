USE RealEngineeringLabSql001;
GO
SET NOCOUNT ON;
WAITFOR DELAY '00:00:01';
INSERT dbo.Payments(PaymentId, Amount, Status)
VALUES (3, 300.00, 'Captured');
SELECT 'WRITER_COMMITTED' AS WriterStatus;