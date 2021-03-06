USE [master]
GO
/****** Object:  Database [DapperSample]    Script Date: 11/3/2017 12:52:09 PM ******/
CREATE DATABASE [DapperSample]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DapperSample', FILENAME = N'd:\Work\Conferences\PASS\2017\Dapper\Demo\Data\DapperSample.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DapperSample_log', FILENAME = N'd:\Work\Conferences\PASS\2017\Dapper\Demo\Data\DapperSample_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [DapperSample] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DapperSample].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DapperSample] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DapperSample] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DapperSample] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DapperSample] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DapperSample] SET ARITHABORT OFF 
GO
ALTER DATABASE [DapperSample] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [DapperSample] SET AUTO_SHRINK ON 
GO
ALTER DATABASE [DapperSample] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DapperSample] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DapperSample] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DapperSample] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DapperSample] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DapperSample] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DapperSample] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DapperSample] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DapperSample] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DapperSample] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DapperSample] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DapperSample] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DapperSample] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DapperSample] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DapperSample] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DapperSample] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [DapperSample] SET  MULTI_USER 
GO
ALTER DATABASE [DapperSample] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DapperSample] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DapperSample] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DapperSample] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [DapperSample] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [DapperSample] SET QUERY_STORE = OFF
GO
USE [DapperSample]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [DapperSample]
GO
/****** Object:  Schema [log]    Script Date: 11/3/2017 12:52:09 PM ******/
CREATE SCHEMA [log]
GO
USE [DapperSample]
GO
/****** Object:  Sequence [dbo].[CompanySequence]    Script Date: 11/3/2017 12:52:09 PM ******/
CREATE SEQUENCE [dbo].[CompanySequence] 
 AS [int]
 START WITH 2
 INCREMENT BY 1
 MINVALUE 1
 MAXVALUE 2147483647
 CACHE 
GO
USE [DapperSample]
GO
/****** Object:  Sequence [dbo].[UserSequence]    Script Date: 11/3/2017 12:52:09 PM ******/
CREATE SEQUENCE [dbo].[UserSequence] 
 AS [int]
 START WITH 6
 INCREMENT BY 1
 MINVALUE 1
 MAXVALUE 2147483647
 CACHE 
GO
/****** Object:  UserDefinedTableType [dbo].[UserTagsType]    Script Date: 11/3/2017 12:52:09 PM ******/
CREATE TYPE [dbo].[UserTagsType] AS TABLE(
	[UserId] [int] NOT NULL,
	[Tag] [nvarchar](20) NOT NULL
)
GO
/****** Object:  Table [dbo].[Users]    Script Date: 11/3/2017 12:52:09 PM ******/
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
/****** Object:  Table [dbo].[Company]    Script Date: 11/3/2017 12:52:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Company](
	[Id] [int] NOT NULL,
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
/****** Object:  View [dbo].[UsersAndCompanyAndAddress]    Script Date: 11/3/2017 12:52:10 PM ******/
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
/****** Object:  Table [dbo].[UserTags]    Script Date: 11/3/2017 12:52:10 PM ******/
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
/****** Object:  View [dbo].[UsersTagsView]    Script Date: 11/3/2017 12:52:10 PM ******/
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
/****** Object:  Table [dbo].[UserRoles]    Script Date: 11/3/2017 12:52:10 PM ******/
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
/****** Object:  View [dbo].[UsersRolesView]    Script Date: 11/3/2017 12:52:10 PM ******/
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
/****** Object:  View [dbo].[UsersAndCompany]    Script Date: 11/3/2017 12:52:10 PM ******/
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
/****** Object:  Table [log].[Logins]    Script Date: 11/3/2017 12:52:10 PM ******/
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
ALTER TABLE [dbo].[Company] ADD  DEFAULT (NEXT VALUE FOR [dbo].[CompanySequence]) FOR [Id]
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
/****** Object:  StoredProcedure [dbo].[GetUserJson]    Script Date: 11/3/2017 12:52:10 PM ******/
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
/****** Object:  StoredProcedure [dbo].[ProcedureBasic]    Script Date: 11/3/2017 12:52:10 PM ******/
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
/****** Object:  StoredProcedure [dbo].[ProcedureWithOutputAndReturnValue]    Script Date: 11/3/2017 12:52:10 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SetUserRoles]    Script Date: 11/3/2017 12:52:10 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SetUserTagsViaJson]    Script Date: 11/3/2017 12:52:10 PM ******/
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
/****** Object:  StoredProcedure [dbo].[SetUserViaJson]    Script Date: 11/3/2017 12:52:10 PM ******/
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

DECLARE @userId INT, @companyId INT;

--SELECT @userId = JSON_VALUE(@userData, '$.Id');
--SELECT @companyId = JSON_VALUE(@userData, '$.Company.Id');

BEGIN TRAN;

DECLARE @C AS TABLE (CompanyId INT);

MERGE INTO
	[dbo].[Company] AS t
USING
	OPENJSON(@userData, '$.Company') WITH 
	(
		Id INT, 
		CompanyName NVARCHAR(100),
		Street NVARCHAR(100) '$.Address.Street',
		City NVARCHAR(100) '$.Address.City',
		[State] NVARCHAR(100) '$.Address.State',
		Country NVARCHAR(100) '$.Address.Country'
	) AS s
ON 
	t.Id = s.Id
WHEN NOT MATCHED THEN 
	INSERT (CompanyName, Street, City, [State], Country)
    VALUES (s.CompanyName, s.Street, s.City, s.[State], s.Country)
WHEN MATCHED THEN
    UPDATE SET 
		t.CompanyName = s.CompanyName,
        t.Street = s.Street,
        t.City = s.City,
		t.[State] = s.[State],
		t.Country = s.Country
OUTPUT
	[Inserted].Id INTO @C
;	

SELECT TOP(1) @companyId = CompanyId FROM @C;

DECLARE @U AS TABLE (UserId INT);

MERGE INTO 
	dbo.Users AS t
USING
	(
		SELECT 
			Id ,
            FirstName ,
            LastName ,
            EmailAddress ,
            CustomData, 
			@companyId AS CompanyId 
		FROM 
			OPENJSON(@userData) WITH 
			( 
				Id INT ,
				FirstName NVARCHAR(100) ,
				LastName NVARCHAR(100) ,
				EmailAddress NVARCHAR(255) ,
				CustomData NVARCHAR(MAX) '$.Preferences' AS JSON 
			)
	) AS s
ON 
	t.Id = s.Id
WHEN NOT MATCHED THEN 
	INSERT (FirstName, LastName, EMailAddress, CompanyId, CustomData )
    VALUES (s.FirstName, s.LastName, s.EmailAddress, s.CompanyId, JSON_MODIFY('{}', '$.Preferences', JSON_QUERY(s.CustomData)))
WHEN MATCHED THEN
    UPDATE SET 
		t.FirstName = s.FirstName ,
        t.LastName = s.LastName ,
        t.EMailAddress = s.EmailAddress ,
		t.CompanyId = s.CompanyId,
        t.CustomData = JSON_MODIFY(ISNULL(t.CustomData, '{}'), '$.Preferences', JSON_QUERY(s.CustomData))
OUTPUT
	[Inserted].Id INTO @U;

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

COMMIT TRAN;

EXEC dbo.GetUserJson @userId
GO
USE [master]
GO
ALTER DATABASE [DapperSample] SET  READ_WRITE 
GO
