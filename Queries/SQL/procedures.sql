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
