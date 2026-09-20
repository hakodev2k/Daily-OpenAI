USE master;
GO
IF DB_ID('RealEngineeringLabSql001') IS NOT NULL
BEGIN
    ALTER DATABASE RealEngineeringLabSql001 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RealEngineeringLabSql001;
END
GO
CREATE DATABASE RealEngineeringLabSql001;
GO
ALTER DATABASE RealEngineeringLabSql001 SET ALLOW_SNAPSHOT_ISOLATION ON;
GO
USE RealEngineeringLabSql001;
GO
CREATE TABLE dbo.Payments
(
    PaymentId int NOT NULL PRIMARY KEY,
    Amount decimal(12,2) NOT NULL,
    Status varchar(20) NOT NULL
);
INSERT dbo.Payments(PaymentId, Amount, Status)
VALUES (1, 100.00, 'Captured'), (2, 200.00, 'Captured');
GO