IF DB_ID('EngineeringDeadlockLab') IS NULL CREATE DATABASE EngineeringDeadlockLab;
GO
USE EngineeringDeadlockLab;
GO
IF OBJECT_ID('dbo.Inventory','U') IS NOT NULL DROP TABLE dbo.Inventory;
CREATE TABLE dbo.Inventory(LocationId int NOT NULL PRIMARY KEY, Quantity int NOT NULL);
INSERT dbo.Inventory(LocationId, Quantity) VALUES (1,1000),(2,1000);
GO
