USE [RealEngineeringLabSql008];
GO

SELECT SUM(CAST(AmountCents AS bigint)) AS TotalCents
FROM dbo.SettlementLine
WHERE MerchantId = 42;
GO
