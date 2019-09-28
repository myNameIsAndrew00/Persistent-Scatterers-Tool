if object_id('InsertUser','P') is not null
	drop proc InsertUser
go

create procedure InsertUser
	@hashed_password as varbinary(64)
	@username as varchar(100),
	@first_name as varchar(100),
	@last_name as varchar(100)
as
begin

	begin try
		begin transaction

		insert into Users(username,hashed_password)
		values (@username, @hashed_password)

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
			insert into ColorPalettes(palette_name, palette_serialization, user_id, creation_date)
			values (@palette_name, @palette_serialization, @user_id, GETDATE())
			
			select SCOPE_IDENTITY() as ID;
		commit

		select SCOPE_IDENTITY();
	end try
	begin catch
		rollback;
		throw;
	end catch

end

