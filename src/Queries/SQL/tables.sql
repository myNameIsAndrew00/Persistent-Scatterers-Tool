
create table Users(
	user_id int identity(1,1) not null,

	username varchar(100) not null unique,
	hashed_password varbinary(64) not null,

	primary key (user_id)
)


create table UsersDetails(
	user_details_id int identity(1,1) not null,
	user_id int not null unique,

	first_name varchar(100) not null,
	last_name varchar(100) not null,

	account_creation_date date not null,
	timestamp nvarchar(255),
	/*more data can be added here*/

	primary key (user_details_id),
	foreign key (user_id) references Users(user_id)

)

create table Roles(
	role_id int identity(1,1) not null,	
	role_name nvarchar(100) not null unique,
	primary key(role_id)
)

/************/

insert into Roles(role_name)
values ('Normal')

/************/

create table UsersRoles(
	user_id int not null,
	role_id int not null,

	primary key(user_id,role_id),
	foreign key (user_id) references Users(user_id),
	foreign key(role_id) references Roles(role_id)
)


create table DatasetsStatuses
  (
	status_id int not null,
	name nvarchar(100) not null,
	primary key(status_id)
)


insert into DatasetsStatuses(status_id, name)
values (1, 'Uploaded'),
		   (2, 'Generated'),
		   (3, 'Pending'),
		   (4, 'UploadFail'),
		   (5, 'GenerateFail')

/*this table holds the data points loaded by user in the application*/

create table DataSets(
	data_set_id int identity(1,1) not null,
	user_id int not null,
	dataset_name varchar(100) not null,
	status_id int,
	
	/*this fields are used for optimizations*/
	minimum_latitude decimal, 
	minimum_longitude decimal, 
	maximum_latitude decimal, 
	maximum_longitude decimal
	
	unique nonclustered (user_id,dataset_name),

	primary key (data_set_id),
	foreign key (user_id) references Users(user_id) on delete cascade,
	foreign key (status_id) references DatasetsStatuses(status_id)
)


create table ColorPalettes(
	color_palette_id int identity(1,1),
	palette_name varchar(255) not null,
	palette_serialization text not null,
	user_id int not null,
	creation_date datetime not null default(GETDATE()),
	
	unique nonclustered(palette_name,user_id),
	primary key(color_palette_id),
	foreign key (user_id) references Users(user_id)
)
 