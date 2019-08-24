
create table Users(
	user_id int identity(1,1) not null,

	username varchar(100) not null unique,
	hashed_password varbinary(32) not null,
	password_salt varbinary(32) not null
	
	primary key (user_id)
)


create table UsersDetails(
	user_details_id int identity(1,1) not null,
	user_id int not null unique,

	first_name varchar(100) not null,
	last_name varchar(100) not null,

	account_creation_date date not null,

	/*more data can be added here*/

	primary key (user_details_id),
	foreign key (user_id) references Users(user_id)

)

/*this table holds the data points loaded by user in the application*/

create table DataSets(
	data_set_id int identity(1,1) not null,
	user_id int not null,
	dataset_name varchar(100) not null,
	
	unique nonclustered (user_id,dataset_name),

	primary key (data_set_id),
	foreign key (user_id) references Users(user_id) on delete cascade
)


create table ColorPalettes(
	color_palette_id int identity(1,1),
	palette_name varchar(255) not null unique,
	palette_serialization text not null,
	user_id int not null,

	primary key(color_palette_id),
	foreign key (user_id) references Users(user_id)
)
 