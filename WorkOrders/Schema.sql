CREATE TABLE Customers (
		[CustomerID] int not null identity,
		[customername] nvarchar(50),
		[address] nvarchar(50),
		[email] nvarchar(50),
		[phonenumber] INT,
		PRIMARY KEY( [CustomerID] )
	)

CREATE TABLE Order (
		[OrderID] int not null identity,
		[ordernumber] INT,
		[repairdate] date,
		[vehicelyear] INT,
		[vehiclemake] NVARCHAR(50),
		[vehiclemodel] NVARCHAR(50),
		[vehicleliscenceplate] NVARCHAR (50),
		[vehiclemileage] INT,
		[orderestimate] DECIMAL(9,2),
		[techname] NVARCHAR (50),
		[laborhours] Decimal(9,2),
		[laborcost] DECIMAL(9,2),
		[labortotal] DECIMAL(9,2),
		[subtotal] DECIMAL(9,2),
		[taxamount] DECIMAL(9,2),
		[grandtotal] DECIMAL(9,2),
		[CustomerID] INT,
		PRIMARY KEY( [OrderID] ),
		FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
	)

CREATE TABLE Parts (
		[PartsID] int not null identity,
		[partsquantity] INT,
		[partnumber] INT,
		[partsname] nvarchar(50),
		[partcost] DECIMAL(9,2),
		[partcosttotal] DECIMAL(9,2),
		[OrderID] INT
		PRIMARY KEY( [PartsID] ),
		FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
	)