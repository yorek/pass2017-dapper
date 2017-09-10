select 
	* 
from 
	sys.dm_exec_requests r
inner join 
	sys.dm_exec_sessions s on r.session_id = s.session_id
where
	s.is_user_process = 1
and
	[program_name] = 'DapperDemo'
