CREATE TABLE [dbo].[Customers] (
    [CustomerID]   INT            IDENTITY (1, 1) NOT NULL,
    [customername] NVARCHAR (50)  NULL,
    [address]      NVARCHAR (50)  NULL,
    [email]        NVARCHAR (50)  NULL,
    [subtotal]     DECIMAL (9, 2) NULL,
    [taxamount]    DECIMAL (9, 2) NULL,
    [grandtotal]   DECIMAL (9, 2) NULL,
    [PhoneNumber]  Nvarchar(20) NULL,
    PRIMARY KEY CLUSTERED ([CustomerID] ASC)
);

