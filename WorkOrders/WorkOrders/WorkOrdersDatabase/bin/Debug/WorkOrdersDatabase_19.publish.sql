﻿/*
Deployment script for WorkOrders

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "WorkOrders"
:setvar DefaultFilePrefix "WorkOrders"
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
/*
The type for column ordernumber in table [dbo].[Orders] is currently  INT NOT NULL but is being changed to  DECIMAL (9, 2) NOT NULL. Data loss could occur.

The type for column vehicleyear in table [dbo].[Orders] is currently  INT NOT NULL but is being changed to  DECIMAL (9, 2) NOT NULL. Data loss could occur.
*/

IF EXISTS (select top 1 1 from [dbo].[Orders])
    RAISERROR (N'Rows were detected. The schema update is terminating because data loss might occur.', 16, 127) WITH NOWAIT

GO
PRINT N'Altering [dbo].[Orders]...';


GO
ALTER TABLE [dbo].[Orders] ALTER COLUMN [ordernumber] DECIMAL (9, 2) NOT NULL;

ALTER TABLE [dbo].[Orders] ALTER COLUMN [vehicleyear] DECIMAL (9, 2) NOT NULL;


GO
PRINT N'Update complete.';


GO
