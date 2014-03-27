declare @datafile varchar(255)
declare @logfile varchar(255)
declare @backupfile varchar(255)
select @datafile = '$(DB_DATA_PATH)\Snapdragon.mdf'
select @logfile = '$(DB_DATA_PATH)\Snapdragon_log.ldf'
select @backupfile = '$(DB_BACKUP_PATH)\$(backupfilename)'

RESTORE DATABASE [Snapdragon] 
FROM  DISK = @backupfile 
WITH  FILE = 1,  
MOVE N'Snapdragon' TO @datafile,  
MOVE N'Snapdragon_log' TO @logfile,  
NOUNLOAD,  REPLACE,  STATS = 10
GO
