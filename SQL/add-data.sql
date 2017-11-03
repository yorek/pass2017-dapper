DELETE FROM [log].[Logins]
DELETE FROM [dbo].[UserRoles]
DELETE FROM [dbo].[UserTags]
DELETE FROM [dbo].[Company]
DELETE FROM [dbo].[Users]
GO

INSERT INTO dbo.[Company] 
	([Id], [CompanyName], [Street], [City], [State], [Country])
VALUES 
	(1, 'Acme LLC', '10123 Ave NE', N'Redmond', N'WA', N'United States');
GO

ALTER SEQUENCE dbo.CompanySequence RESTART WITH 2
GO

INSERT INTO dbo.[Users] 
	([Id], [FirstName], [LastName], [EMailAddress], [CompanyId])
VALUES 
	(1, 'Joe', 'Black', 'jb@email.com', null),
	(2, 'Sarah', 'White', 'sw@email.com', null),
	(3, 'Mike', 'Green', 'mg@email.com', null),
	(4, 'Ann', 'Brown', 'ab@email.com', null),
	(5, 'Davide', 'Mauri', 'info@davidemauri.it', 1)
GO

ALTER SEQUENCE dbo.UserSequence RESTART WITH 6
GO
