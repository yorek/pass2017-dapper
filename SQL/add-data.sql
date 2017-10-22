DELETE FROM [log].[Logins]
DELETE FROM [dbo].[UserRoles]
DELETE FROM [dbo].[UserTags]
DELETE FROM [dbo].[Company]
DELETE FROM [dbo].[Users]
GO

SET IDENTITY_INSERT dbo.Company ON;

INSERT INTO dbo.[Company] 
	([Id], [CompanyName], [Street], [City], [State], [Country])
VALUES 
	(1, 'Acme LLC', '10123 Ave NE', N'Redmond', N'WA', N'United States');

SET IDENTITY_INSERT dbo.Company OFF;
GO

INSERT INTO dbo.[Users] 
	([Id], [FirstName], [LastName], [EMailAddress], [CompanyId])
VALUES 
	(1, 'Joe', 'Black', 'jb@email.com', null),
	(2, 'Sarah', 'White', 'sw@email.com', null),
	(3, 'Mike', 'Green', 'mg@email.com', null),
	(4, 'Ann', 'Brown', 'ab@email.com', null),
	(5, 'Davide', 'Mauri', 'dm@email.com', 1)
GO

ALTER SEQUENCE dbo.UserSequence RESTART WITH 6
GO

