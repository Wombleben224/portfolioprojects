--1
SELECT o.ordernumber, o.repairdate, o.techname FROM Orders o
ORDER BY ordernumber DESC

--2
SELECT p.PartsNumber, p.PartsName, p.PartsQuantity, p.PartCost FROM Parts p
LEFT JOIN Orders o
ON p.OrderID = o.OrderID
WHERE o.ordernumber = 37
ORDER BY p.PartsNumber

--3
SELECT ordernumber, repairdate, techname, GrandTotal FROM Orders
WHERE GrandTotal > 1000

--4
SELECT o.ordernumber, o.repairdate, o.techname, o.GrandTotal FROM Orders o
ORDER BY ordernumber

--5
SELECT CustomerID, customername, PhoneNumber  FROM Customers
WHERE customername LIKE 'K%'
Order BY customername

--6
SELECT COUNT(*) FROM Orders
WHERE techname = 'Jimmy Threehands'

--7
SELECT COUNT(*) FROM Orders
WHERE GrandTotal > 1000


--8
SELECT COUNT(*) FROM Orders
WHERE GrandTotal > 1000
	AND techname = 'Jimmy Threehands'




