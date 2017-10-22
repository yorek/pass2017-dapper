USE [D:\WORK\SENSORIA\CONFERENCES\PASS\2017\DAPPER\DEMO\DATA\DAPPERSAMPLE.MDF]
GO
/****** Object:  Schema [log]    Script Date: 10/22/2017 11:57:59 AM ******/
CREATE SCHEMA [log]
GO
/****** Object:  UserDefinedTableType [dbo].[UserTagsType]    Script Date: 10/22/2017 11:57:59 AM ******/
CREATE TYPE [dbo].[UserTagsType] AS TABLE(
	[UserId] [int] NOT NULL,
	[Tag] [nvarchar](20) NOT NULL
)
GO
/****** Object:  Table [dbo].[Users]    Script Date: 10/22/2017 11:57:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[EMailAddress] [nvarchar](255) NOT NULL,
	[CustomData] [nvarchar](max) NULL,
	[CompanyId] [int] NULL,
 CONSTRAINT [pk__User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [uq__EmailAddress] UNIQUE NONCLUSTERED 
(
	[EMailAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Company]    Script Date: 10/22/2017 11:57:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Company](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyName] [nvarchar](100) NOT NULL,
	[Street] [nvarchar](100) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[State] [nvarchar](100) NULL,
	[Country] [nvarchar](100) NOT NULL,
 CONSTRAINT [pk__Company] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[UsersAndCompanyAndAddress]    Script Date: 10/22/2017 11:57:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE view [dbo].[UsersAndCompanyAndAddress]
as
select 
	u.Id AS UserId,
	FirstName,
	LastName,
	EmailAddress,
	c.Id AS CompanyId,
	c.CompanyName,
	c.[Street], 
	c.City, 
	c.[State], 
	c.Country
from 
	dbo.[Users] u inner join dbo.[Company] c on u.CompanyId = c.Id

GO
/****** Object:  Table [dbo].[UserTags]    Script Date: 10/22/2017 11:57:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTags](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Tag] [nvarchar](20) NOT NULL,
 CONSTRAINT [pk__UserTags] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[UsersTagsView]    Script Date: 10/22/2017 11:57:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[UsersTagsView]
AS
SELECT 
	u.Id,
	FirstName,
	LastName,
	EmailAddress,	
	(SELECT JSON_QUERY(REPLACE(REPLACE((SELECT [Tag] FROM dbo.[UserTags] t WHERE t.UserId = u.Id FOR JSON PATH), '{"Tag":', ''), '}', ''))) AS Tags
FROM 
	dbo.[Users] u 
GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 10/22/2017 11:57:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Role] [nvarchar](20) NOT NULL,
 CONSTRAINT [pk__UserRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[UsersRolesView]    Script Date: 10/22/2017 11:57:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[UsersRolesView]
AS
SELECT 
	u.Id,
	FirstName,
	LastName,
	EmailAddress,	
	(STUFF((SELECT '|' + [Role] as [text()] FROM dbo.[UserRoles] r WHERE r.UserId = u.Id FOR XML PATH('')), 1, 1, '')) AS Roles
FROM 
	dbo.[Users] u 
GO
/****** Object:  View [dbo].[UsersAndCompany]    Script Date: 10/22/2017 11:57:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE view [dbo].[UsersAndCompany]
as
select 
	u.Id AS UserId,
	FirstName,
	LastName,
	EmailAddress,
	c.Id AS CompanyId,
	c.CompanyName
from 
	dbo.[Users] u inner join dbo.[Company] c on u.CompanyId = c.Id


GO
/****** Object:  Table [log].[Logins]    Script Date: 10/22/2017 11:57:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [log].[Logins](
	[UserId] [int] NOT NULL,
	[AttemptTime] [datetimeoffset](7) NOT NULL,
	[Result] [char](1) NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [df_Id]  DEFAULT (NEXT VALUE FOR [dbo].[UserSequence]) FOR [Id]
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [fk__UserRoles_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [fk__UserRoles_Users]
GO
ALTER TABLE [dbo].[UserTags]  WITH CHECK ADD  CONSTRAINT [fk__UserTags_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserTags] CHECK CONSTRAINT [fk__UserTags_Users]
GO
ALTER TABLE [log].[Logins]  WITH CHECK ADD  CONSTRAINT [fk__Logins__Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [log].[Logins] CHECK CONSTRAINT [fk__Logins__Users]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD CHECK  ((isjson([CustomData])=(1)))
GO
ALTER TABLE [log].[Logins]  WITH CHECK ADD  CONSTRAINT [ck_Result] CHECK  (([Result]='F' OR [Result]='S'))
GO
ALTER TABLE [log].[Logins] CHECK CONSTRAINT [ck_Result]
GO
/****** Object:  StoredProcedure [dbo].[GetUserJson]    Script Date: 10/22/2017 11:57:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetUserJson]
@userId INT 
AS
SELECT	
	u.Id,
	u.FirstName,
	u.LastName,
	u.EMailAddress AS 'EmailAddress',
	(SELECT JSON_QUERY(REPLACE(REPLACE((SELECT [Tag] FROM dbo.[UserTags] t WHERE t.UserId = u.Id FOR JSON PATH), '{"Tag":', ''), '}', ''))) AS Tags,
	JSON_QUERY((SELECT [Role] AS RoleName FROM dbo.UserRoles r WHERE r.UserId = u.Id FOR JSON AUTO)) AS Roles,
	c.Id AS 'Company.Id',
	c.CompanyName AS 'Company.CompanyName',
	c.Street AS 'Company.Address.Street',
	c.City AS 'Company.Address.City',
	c.[State] AS 'Company.Address.State',
	c.Country AS 'Company.Address.Country',
	JSON_QUERY(u.CustomData, '$.Preferences') AS Preferences
FROM
	dbo.[Users] u
LEFT JOIN
	dbo.[Company] c ON u.CompanyId = c.Id
WHERE
	u.id = @userId
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
GO
/****** Object:  StoredProcedure [dbo].[ProcedureBasic]    Script Date: 10/22/2017 11:57:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ProcedureBasic]
@email nvarchar(255)
AS
SELECT 
	Id,
	FirstName,
	LastName,
	EMailAddress
FROM 
	dbo.[Users] 
WHERE
	EMailAddress = @email
GO
/****** Object:  StoredProcedure [dbo].[ProcedureWithOutputAndReturnValue]    Script Date: 10/22/2017 11:57:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ProcedureWithOutputAndReturnValue]
@email nvarchar(255),
@firstName nvarchar(100) output,
@lastName nvarchar(100) output
AS
DECLARE @userId INT
SELECT 
	@userId = Id, 
	@firstName = FirstName, 
	@lastName = LastName 
FROM 
	dbo.[Users] 
WHERE
	EMailAddress = @email
RETURN @userId
GO
/****** Object:  StoredProcedure [dbo].[SetUserRoles]    Script Date: 10/22/2017 11:57:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SetUserRoles]
@userId INT,
@roles NVARCHAR(MAX)
AS
SET XACT_ABORT ON;


BEGIN TRAN;

DELETE FROM dbo.[UserRoles] WHERE UserId = @userId;

INSERT INTO
	dbo.[UserRoles] ([UserId], [Role])
SELECT
	@userId,
	[value] AS [Role]
FROM
	STRING_SPLIT(@roles, '|')

COMMIT;
GO
/****** Object:  StoredProcedure [dbo].[SetUserTagsViaJson]    Script Date: 10/22/2017 11:57:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SetUserTagsViaJson]
@userId INT,
@tags NVARCHAR(MAX)
AS
SET XACT_ABORT ON;

IF ( ISJSON(@tags) != 1 )
	THROW 50000, '@tags is not a valid JSON document', 1;

BEGIN TRAN;

DELETE FROM dbo.[UserTags] WHERE UserId = @userId;

INSERT INTO
	dbo.[UserTags] ([UserId], [Tag])
SELECT
	@userId,
	[value] AS [Tag]
FROM
	OPENJSON(@tags);

COMMIT;
GO
/****** Object:  StoredProcedure [dbo].[SetUserViaJson]    Script Date: 10/22/2017 11:57:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SetUserViaJson]
@userData NVARCHAR(MAX)
AS
SET XACT_ABORT ON;

IF ( ISJSON(@userData) != 1 )
	THROW 50000, '@userData is not a valid JSON document', 1;

DECLARE @userId INT;

SELECT @userId = JSON_VALUE(@userData, '$.Id');

BEGIN TRAN;

DECLARE @U AS TABLE (UserId INT);

MERGE INTO 
	dbo.Users AS t
USING
	OPENJSON(@userData) WITH ( 
		Id INT ,
        FirstName NVARCHAR(100) ,
        LastName NVARCHAR(100) ,
        EmailAddress NVARCHAR(255) ,
        CustomData NVARCHAR(MAX) '$.Preferences' AS JSON 
	) AS s
ON 
	t.Id = s.Id
WHEN NOT MATCHED THEN 
	INSERT (FirstName, LastName, EMailAddress, CustomData )
    VALUES (s.FirstName, s.LastName, s.EmailAddress, JSON_MODIFY('{}', '$.Preferences', JSON_QUERY(s.CustomData)))
WHEN MATCHED THEN
    UPDATE SET 
		t.FirstName = s.FirstName ,
        t.LastName = s.LastName ,
        t.EMailAddress = s.EmailAddress ,
        t.CustomData = JSON_MODIFY(t.CustomData, '$.Preferences', JSON_QUERY(s.CustomData))
OUTPUT
	[Inserted].Id INTO @U;

IF (@userId = 0) 
	SELECT TOP(1) @userId = UserId FROM @u;

DELETE FROM dbo.[UserTags] WHERE UserId = @userId;

INSERT INTO dbo.[UserTags] 
SELECT 
	@userId AS UserId,
	Tag
FROM 
	OPENJSON(@userData, '$.Tags') WITH
	(
		Tag VARCHAR(100) '$'
	);

DELETE FROM dbo.[UserRoles] WHERE UserId = @userId;

INSERT INTO dbo.[UserRoles] 
SELECT 
	@userId AS UserId,
	RoleName
FROM 
	OPENJSON(@userData, '$.Roles') WITH 
	(
		RoleName NVARCHAR(100)
	)

DELETE FROM dbo.[Company] WHERE CompanyName = JSON_VALUE(@userData, '$.Company.CompanyName');

DECLARE @C AS TABLE (CompanyId INT);

INSERT INTO 
	[dbo].[Company]
OUTPUT
	[Inserted].Id INTO @C
SELECT TOP (1)
	CompanyName,
	Street,
	City,
	[State],
	Country
FROM 
	OPENJSON(@userData, '$.Company') WITH 
	(
		CompanyName NVARCHAR(100),
		Street NVARCHAR(100) '$.Address.Street',
		City NVARCHAR(100) '$.Address.City',
		[State] NVARCHAR(100) '$.Address.State',
		Country NVARCHAR(100) '$.Address.Country'
	);

UPDATE 
	dbo.[Users] 
SET 
	CompanyId = (SELECT TOP (1) CompanyId FROM @c)
WHERE
	id = @userId;

COMMIT TRAN;

EXEC dbo.GetUserJson @userId
GO
