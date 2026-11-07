SET NOCOUNT ON;
IF DB_ID('EngineeringLabSql002') IS NULL CREATE DATABASE EngineeringLabSql002;
GO
USE EngineeringLabSql002;
GO
DROP TABLE IF EXISTS dbo.Orders;
GO
CREATE TABLE dbo.Orders(
    Id bigint IDENTITY PRIMARY KEY,
    TenantId int NOT NULL,
    CreatedAt datetime2 NOT NULL,
    Status tinyint NOT NULL,
    Amount decimal(18,2) NOT NULL,
    Padding char(120) NOT NULL DEFAULT REPLICATE('X',120)
);
CREATE INDEX IX_Orders_Tenant_CreatedAt ON dbo.Orders(TenantId, CreatedAt) INCLUDE(Status, Amount);
GO
;WITH n AS (
    SELECT TOP (100000) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
    FROM sys.all_objects a CROSS JOIN sys.all_objects b
)
INSERT dbo.Orders(TenantId,CreatedAt,Status,Amount)
SELECT CASE WHEN rn <= 500 THEN 1 ELSE 999 END,
       DATEADD(minute,-rn,'2026-09-23T00:00:00'),
       rn % 4,
       10 + (rn % 500)
FROM n;
GO
CREATE OR ALTER PROCEDURE dbo.GetTenantOrders
    @TenantId int,
    @From datetime2
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id,TenantId,CreatedAt,Status,Amount
    FROM dbo.Orders
    WHERE TenantId=@TenantId AND CreatedAt>=@From
    ORDER BY CreatedAt DESC;
END;
GO