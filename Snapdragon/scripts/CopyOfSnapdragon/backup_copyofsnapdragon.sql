declare @timestamp char(8)
select @timestamp = CONVERT(char(8), GETDATE(), 112)

declare @backupfile varchar(255)
select @backupfile = 'CopyOfSnapdragon' + @timestamp + '.bak'
backup database [CopyOfSnapdragon]
to disk = @backupfile
with init, nounload, name = N'CopyOfSnapdragon backup', noskip, stats = 10, noformat

go