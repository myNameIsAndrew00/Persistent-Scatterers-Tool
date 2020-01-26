
create database PersistantScatterersDatabase
on primary
(
	Name = PersistantScatterersDatabase,
	Filename = 'P:\Projects\Licence\Main\data\Database\PersistantScatterersDatabase.mdf',
	size = 100mb,
	maxsize = unlimited,
	filegrowth = 500mb
),
(
	Name = PersistantScatterersDatabaseN,
	Filename = 'P:\Projects\Licence\Main\data\Database\PersistantScatterersDatabaseN.ndf',
	size = 50mb,
	maxsize = unlimited,
	filegrowth = 500mb
),
(
	Name = PersistantScatterersDatabaseNS,
	Filename = 'P:\Projects\Licence\Main\data\Database\PersistantScatterersDatabaseNS.ndf',
	size = 50mb,
	maxsize = unlimited,
	filegrowth = 500mb
)
log on 
(
	Name = PersistantScatterersDatabaseLog,
	Filename = 'P:\Projects\Licence\Main\data\Database\PersistantScatterersDatabaseLOG.ldf',
	size = 10mb,
	maxsize = unlimited,
	filegrowth = 30mb
)


