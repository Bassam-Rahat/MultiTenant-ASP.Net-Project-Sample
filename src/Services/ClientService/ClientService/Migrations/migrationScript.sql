IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF SCHEMA_ID(N'client') IS NULL EXEC(N'CREATE SCHEMA [client];');
GO

CREATE TABLE [client].[ClientProducts] (
    [Id] int NOT NULL IDENTITY,
    [ClientId] int NOT NULL,
    [ProductId] int NOT NULL,
    CONSTRAINT [PK_ClientProducts] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [client].[Clients] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Clients] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240905095635_clientTableAdded', N'6.0.33');
GO

COMMIT;
GO

