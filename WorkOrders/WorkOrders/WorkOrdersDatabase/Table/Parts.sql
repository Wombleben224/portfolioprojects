CREATE TABLE [dbo].[Parts] (
    [Partsid]       INT            IDENTITY (1, 1) NOT NULL,
    [partsquantity] INT            NULL,
    [partsnumber]   INT            NULL,
    [partsname]     NVARCHAR (50)  NULL,
    [partcost]      DECIMAL (9, 2) NULL,
    [OrderID]       INT            NULL,
    PRIMARY KEY CLUSTERED ([Partsid] ASC),
    FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Orders] ([OrderID])
);

