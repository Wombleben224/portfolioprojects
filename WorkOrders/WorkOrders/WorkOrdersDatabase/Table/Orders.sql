CREATE TABLE [dbo].[Orders] (
    [OrderID]              INT IDENTITY (1, 1) NOT NULL,
    [ordernumber]          INT NOT NULL,
    [repairdate]           DATE           NOT NULL,
    [vehicleyear]          INT NOT NULL,
    [vehiclemake]          NVARCHAR (50)  NOT NULL,
    [vehiclemodel]         NVARCHAR (50)  NOT NULL,
    [vehicleliscenseplate] NVARCHAR (50)  NOT NULL,
    [vehiclemileage]       DECIMAL (8,1) NOT NULL,
    [orderestimate]        DECIMAL (9, 2) NOT NULL,
    [techname]             NVARCHAR (50)  NOT NULL,
    [laborhours]           DECIMAL (9, 2) NOT NULL,
    [laborcost]            DECIMAL (9, 2) NOT NULL,
    [labortotals]          DECIMAL (9, 2) NOT NULL,
    [CustomerID]           INT            NULL,
    [GrandTotal]           DECIMAL (9, 2) NULL,
    PRIMARY KEY CLUSTERED ([OrderID] ASC),
    FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customers] ([CustomerID])
);

