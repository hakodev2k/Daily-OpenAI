IF DB_ID(N'RealEngineeringLabSql008') IS NULL
BEGIN
    EXEC(N'CREATE DATABASE [RealEngineeringLabSql008]');
END
GO

USE [RealEngineeringLabSql008];
GO

DROP TABLE IF EXISTS dbo.SettlementLine;
GO

CREATE TABLE dbo.SettlementLine
(
    SettlementLineId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    MerchantId int NOT NULL,
    AmountCents int NOT NULL
);
GO

INSERT dbo.SettlementLine (MerchantId, AmountCents)
VALUES
    (42, 1100000000),
    (42, 1100000000),
    (42, 1100000000);
GO
