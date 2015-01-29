select 'TODO: update with your db name and db server IP (and Scoring Daemon name, currently set to the value in the sample config file)'
return; -- comment this out when values have been updated

--TODO: use your db and server and SD name...
declare @IP varchar(30)
declare @privateIP varchar(30)
declare @dbname varchar(255)

set @IP = 'ts-db-dev.opentestsystem.org'
set @privateIP = 'ts-db-dev.opentestsystem.org'
set @dbname = 'TDS_QC_OSS'

/****** Object:  Table [dbo].[ScoringDaemonMonitoredSites]    Script Date: 01/15/2015 15:05:20 ******/
INSERT [dbo].[ScoringDaemonMonitoredSites] ([_efk_ClientName], [environment], [IP], [privateIP], [dbname], [ScoringDaemonName], [serviceType], [serviceRole]) VALUES (N'SBAC', N'Dev', @IP, @privateIP, @dbname, N'TISScoringDaemon_OSS_IT', NULL, NULL)