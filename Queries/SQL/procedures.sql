if object_id('InsertUser','P') is not null
	drop proc InsertUser
go

create procedure InsertUser
	@hashed_password as varbinary(32),
	@password_salt as varbinary(32),
	@username as varchar(100),
	@first_name as varchar(100),
	@last_name as varchar(100)
as
begin

	begin try
		begin transaction

		insert into Users(username,hashed_password, password_salt)
		values (@username, @hashed_password, @password_salt)

		declare @user_id as int = SCOPE_IDENTITY();

		insert into UsersDetails(user_id, first_name, last_name, account_creation_date)
		values (@user_id, @first_name, @last_name, GETDATE())
						  
		commit
		end try
	begin catch	
		rollback;
		throw;
	end  catch

end

 

go

  
if object_id('GetUserPasswordInfo','P') is not null
	drop procedure GetUserPasswordInfo
go
create procedure GetUserPasswordInfo
	@username as varchar(100)
as
begin
	select U.hashed_password, U.password_salt
	from Users as U
	where U.username = @username

end

go

if object_id('GetUserPointsDataset','P') is not null
	drop procedure GetUserPointsDataset
go
create procedure GetUserPointsDataset
	@username as varchar(100),
	@dataset_name as varchar(100)
as
begin
	select data_set_id 
	from DataSets as DS
		inner join Users as U
		on DS.user_id = U.user_id and U.username = @username
	where DS.dataset_name = @dataset_name

end

go

if object_id('GetColorPalette', 'P') is not null
	drop procedure GetColorPalette
go
create procedure GetColorPalette 
	@username as varchar(100),
	@palette_name as varchar(255)
as 
begin
	select CP.palette_name,
		   CP.palette_serialization from ColorPalettes as CP
		inner join Users as U
		on U.username = @username
		   and CP.user_id = U.user_id
		where CP.palette_name = @palette_name
end

if object_id('GetUserColorPalettes', 'P') is not null
	drop procedure GetUserColorPalettes
go
create procedure GetUserColorPalettes 
	@username as varchar(100) 
as 
begin
	select CP.palette_name from ColorPalettes as CP
		inner join Users as U
		on U.username = @username
		   and CP.user_id = U.user_id 
end


go

if object_id('InsertPointsDataset', 'P') is not null
	drop procedure InsertPointsDataset
go
create procedure InsertPointsDataset 
	@username as varchar(100),
	@dataset_name as varchar(100)
as 
begin

	begin try
		begin transaction
			declare @user_id as int = -1,
					@dataset_id as int = -1;
			
			--Get the user id
			select @user_id = U.user_id
			from Users as U where U.username = @username

			--Insert data into the dataset 
			insert into DataSets(user_id, dataset_name)
			values (@user_id, @dataset_name)
			
			select SCOPE_IDENTITY() as ID;
		commit

		select SCOPE_IDENTITY();
	end try
	begin catch
		rollback;
		throw;
	end catch

end


go

if object_id('InsertColorPalette', 'P') is not null
	drop procedure InsertColorPalette
go
create procedure InsertColorPalette
	@username as varchar(100),
	@palette_name as varchar(255),
	@palette_serialization as text
as 
begin
	begin try
		begin transaction
			declare @user_id as int = -1; 
			
			--Get the user id
			select @user_id = U.user_id
			from Users as U where U.username = @username

			--Insert data into the color palettes 
			insert into ColorPalettes(palette_name, palette_serialization, user_id)
			values (@palette_name, @palette_serialization, @user_id)
			
			select SCOPE_IDENTITY() as ID;
		commit

		select SCOPE_IDENTITY();
	end try
	begin catch
		rollback;
		throw;
	end catch

end

