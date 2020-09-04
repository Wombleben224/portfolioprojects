CREATE TABLE [dbo].[Members]
(
	[memberId] NVARCHAR(128) NOT NULL,
	[Username] NVARCHAR(50) NULL,
	[VehicleYear] INT NOT NULL,
	[VehicleMake] NVARCHAR(50) NOT NULL,
	[VehicleModel] NVARCHAR(50) NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	[Bio] NVARCHAR(1500) NOT NULL,
	[ProfileImageType] VARCHAR(100) null,
	[ProfileImageName] VARCHAR(100) null,
	PRIMARY KEY ([memberId]),
)
