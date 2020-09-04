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
PRINT N'Dropping unnamed constraint on [dbo].[Events]...';


GO
ALTER TABLE [dbo].[Events] DROP CONSTRAINT [DF__Events__Going__01142BA1];


GO
PRINT N'Altering [dbo].[Events]...';


GO
ALTER TABLE [dbo].[Events] ALTER COLUMN [Going] INT NULL;


GO
PRINT N'Creating [dbo].[Subscribers]...';


GO
CREATE TABLE [dbo].[Subscribers] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [FullName]       NVARCHAR (200) NOT NULL,
    [Email]          NVARCHAR (200) NOT NULL,
    [IsConfirmed]    BIT            NOT NULL,
    [IsSubscribed]   BIT            NOT NULL,
    [UnsubscribeUrl] VARCHAR (1024) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_Subscribers_Email] UNIQUE NONCLUSTERED ([Email] ASC)
);


GO
PRINT N'Creating unnamed constraint on [dbo].[Events]...';


GO
ALTER TABLE [dbo].[Events]
    ADD DEFAULT ((0)) FOR [Going];


GO
PRINT N'Creating unnamed constraint on [dbo].[Subscribers]...';


GO
ALTER TABLE [dbo].[Subscribers]
    ADD DEFAULT ((0)) FOR [IsConfirmed];


GO
PRINT N'Creating unnamed constraint on [dbo].[Subscribers]...';


GO
ALTER TABLE [dbo].[Subscribers]
    ADD DEFAULT ((0)) FOR [IsSubscribed];


GO
PRINT N'Update complete.';


GO
