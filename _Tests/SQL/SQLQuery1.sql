declare @uj nvarchar(max) = '{"Id":5,"FirstName":"Davide","LastName":"Mauri","EmailAddress":"info@davidemauri.it","Tags":["one","two"],"Roles":[{"RoleName":"User"},{"RoleName":"Developer"},{"RoleName":"Administrator"}],"Preferences":{"Theme":"Modern","Style":"Black","Resolution":"1920x1080"}}';

--delete from dbo.[Users] where Id = 5

merge into 
	dbo.Users as t
using 
	openjson(@uj) with (
		Id int,
		FirstName nvarchar(100),
		LastName nvarchar(100),
		EmailAddress nvarchar(255),
		CustomData nvarchar(max) '$.Preferences' as json
	) as s
on
	t.Id = s.Id
when not matched then
	insert values (s.Id, s.FirstName, s.LastName, s.EmailAddress, JSON_MODIFY('{}', '$.Preferences', JSON_QUERY(s.CustomData)))
when matched then
	update set
		t.FirstName = s.FirstName,
		t.LastName = s.LastName,
		t.EmailAddress = s.EmailAddress,
		t.CustomData = JSON_MODIFY(t.CustomData, '$.Preferences', JSON_QUERY(s.CustomData))
;

select * from dbo.Users
