declare @timestamp char(8)
select @timestamp = CONVERT(char(8), GETDATE(), 112)

declare @backupfile varchar(255)
select @backupfile = 'Snapdragon' + @timestamp + '.bak'
backup database [Snapdragon]
to disk = @backupfile
with init, nounload, name = N'Snapdragon backup', noskip, stats = 10, noformat

go