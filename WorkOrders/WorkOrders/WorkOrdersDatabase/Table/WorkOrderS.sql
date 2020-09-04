CREATE TABLE [dbo].[WorkOrder]
(
	[CustomerID] INT NOT NULL,
	[OrderID] INT NOT NULL,
	[PartsID] INT NOT NULL,

	PRIMARY KEY([CustomerID], [OrderID], [PartsID]),
	FOREIGN KEY ([CustomerID]) REFERENCES [Customers] ([CustomerID]) ON DELETE CASCADE,
	FOREIGN KEY ([OrderID]) REFERENCES [Orders] ([OrderID]) ON DELETE CASCADE,
	FOREIGN KEY ([PartsID]) REFERENCES [Parts] ([PartsID]) ON DELETE CASCADE
)
