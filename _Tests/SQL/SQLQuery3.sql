drop procedure dbo.[SetUserViaJson]
go

create procedure dbo.[SetUserViaJson]
@userData nvarchar(max)
as
if (isjson(@userData) != 1)
	throw 50000, '@userData is not a valid JSON document', 1

merge into 
	dbo.Users as t
using 
	openjson(@userData) with (
		Id int,
		FirstName nvarchar(100),
		LastName nvarchar(100),
		EmailAddress nvarchar(255),
		CustomData nvarchar(max) '$.Preferences' as json
	) as s
on
	t.Id = s.Id
when not matched then
	insert values (s.Id, s.FirstName, s.LastName, s.EmailAddress, json_modify('{}', '$.Preferences', json_query(s.CustomData)))
when matched then
	update set
		t.FirstName = s.FirstName,
		t.LastName = s.LastName,
		t.EmailAddress = s.EmailAddress,
		t.CustomData = json_modify(t.CustomData, '$.Preferences', json_query(s.CustomData))
;


go

