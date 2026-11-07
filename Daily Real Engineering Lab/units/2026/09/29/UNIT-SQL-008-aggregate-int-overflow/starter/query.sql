USE [RealEngineeringLabSql008];
GO

SELECT SUM(AmountCents) AS TotalCents
FROM dbo.SettlementLine
WHERE MerchantId = 42;
GO
