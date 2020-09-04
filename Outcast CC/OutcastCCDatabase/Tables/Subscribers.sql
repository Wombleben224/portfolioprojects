CREATE TABLE [dbo].[Subscribers]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [FullName] NVARCHAR(200) NOT NULL,
    [Email] NVARCHAR(200) NOT NULL,
    [IsConfirmed] BIT DEFAULT((0)) NOT NULL,
    [IsSubscribed] BIT DEFAULT((0)) NOT NULL,
    [UnsubscribeUrl] VARCHAR(1024) NULL,

    CONSTRAINT [IX_Subscribers_Email] UNIQUE ([Email])
)
