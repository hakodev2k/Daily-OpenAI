IF DB_ID(N'EngineeringLabSql005') IS NULL
    CREATE DATABASE EngineeringLabSql005;
GO

USE EngineeringLabSql005;
GO

DROP TABLE IF EXISTS dbo.AuditLog;
DROP TABLE IF EXISTS dbo.Receipts;
GO

CREATE TABLE dbo.Receipts
(
    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Receipts PRIMARY KEY,
    Reference nvarchar(50) NOT NULL,
    Quantity int NOT NULL
);
GO

CREATE TABLE dbo.AuditLog
(
    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditLog PRIMARY KEY,
    EntityType nvarchar(50) NOT NULL,
    EntityId int NOT NULL,
    EventName nvarchar(50) NOT NULL
);
GO

DBCC CHECKIDENT ('dbo.AuditLog', RESEED, 1000) WITH NO_INFOMSGS;
GO

CREATE TRIGGER dbo.TR_Receipts_Audit
ON dbo.Receipts
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.AuditLog(EntityType, EntityId, EventName)
    SELECT N'Receipt', i.Id, N'Created'
    FROM inserted AS i;
END;
GO
