﻿/*
Deployment script for OutcastCC

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "OutcastCC"
:setvar DefaultFilePrefix "OutcastCC"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Starting rebuilding table [dbo].[Members]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Members] (
    [memberId]         INT            IDENTITY (1, 1) NOT NULL,
    [Username]         NVARCHAR (50)  NOT NULL,
    [Password]         NVARCHAR (25)  NOT NULL,
    [VehicleYear]      INT            NOT NULL,
    [VehicleMake]      NVARCHAR (50)  NOT NULL,
    [VehicleModel]     NVARCHAR (50)  NOT NULL,
    [Name]             NVARCHAR (50)  NOT NULL,
    [Bio]              NVARCHAR (500) NOT NULL,
    [ProfileImageType] VARCHAR (100)  NULL,
    [ProfileImageName] VARCHAR (100)  NULL
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Members])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Members] ON;
        INSERT INTO [dbo].[tmp_ms_xx_Members] ([memberId], [Username], [Password], [VehicleYear], [VehicleMake], [VehicleModel], [Name], [Bio], [ProfileImageType], [ProfileImageName])
        SELECT [memberId],
               [Username],
               [Password],
               [VehicleYear],
               [VehicleMake],
               [VehicleModel],
               [Name],
               [Bio],
               [ProfileImageType],
               [ProfileImageName]
        FROM   [dbo].[Members];
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Members] OFF;
    END

DROP TABLE [dbo].[Members];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Members]', N'Members';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Update complete.';


GO
