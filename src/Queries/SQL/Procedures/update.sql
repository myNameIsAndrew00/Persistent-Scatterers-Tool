go

if object_id('UpdatePointsDatasetStatus', 'P') is not null
	drop procedure UpdatePointsDatasetStatus
go
create procedure UpdatePointsDatasetStatus
	@datasetName as nvarchar(255),
	@username as nvarchar(100),
	@statusId as int
as 
begin
	update DataSets
	set status_id = @statusId
	from DataSets as DS
		inner join Users as U 
		on DS.user_id = U.user_id and U.username = @username
	where DS.dataset_name = @datasetName
	
end


go

if object_id('UpdatePointsDatasetLimits', 'P') is not null
	drop procedure UpdatePointsDatasetLimits
go
create procedure UpdatePointsDatasetLimits
	@datasetName as nvarchar(255),
	@username as nvarchar(100),
	@minimum_latitude as decimal(18,14),
	@minimum_longitude as decimal(18,14),
	@maximum_latitude as decimal(18,14),
	@maximum_longitude as decimal(18,14)
as 
begin
	update DataSets
	set minimum_latitude = @minimum_latitude,
		minimum_longitude = @minimum_longitude,
		maximum_latitude = @maximum_latitude,
		maximum_longitude = @maximum_longitude
	from DataSets as DS
		inner join Users as U 
		on DS.user_id = U.user_id and U.username = @username
	where DS.dataset_name = @datasetName
	
end

  
if object_id('UpdatePointsDatasetRepresentationLimits', 'P') is not null
	drop procedure UpdatePointsDatasetRepresentationLimits
go
create procedure UpdatePointsDatasetRepresentationLimits
	@datasetName as nvarchar(255),
	@username as nvarchar(100),
	@minimum_height decimal(18,9),
    @maximum_height decimal(18,9),
	@minimum_def_rate decimal(18,9),
	@maximum_def_rate decimal(18,9),
	@minimum_std_dev decimal(18,9),
	@maximum_std_dev decimal(18,9)
as 
begin
	update DataSets
	set minimum_height = @minimum_height,
		maximum_height = @maximum_height,
		minimum_def_rate = @minimum_def_rate,
		maximum_def_rate = @maximum_def_rate,
		minimum_std_dev  = @minimum_std_dev,
		maximum_std_dev = @maximum_std_dev
	from DataSets as DS
		inner join Users as U 
		on DS.user_id = U.user_id and U.username = @username
	where DS.dataset_name = @datasetName
	
end


go


create procedure UpdateUser 
	@username as varchar(100),
	@first_name as varchar(100),
	@last_name as varchar(100),
	@secure_stamp as varchar(255),
	@email as nvarchar(255)
as
begin

	begin try
		begin transaction
		
		declare @user_id as int = -1;

		select @user_id = U.user_id
		from Users as U where U.username = @username

		update UsersDetails
		set first_name = @first_name,
			last_name = @last_name,
			timestamp = @secure_stamp,
			email = @email
		where user_id = @user_id

		commit
		end try
	begin catch	
		rollback;
		throw;
	end  catch

end
  
 
if object_id('SetUserEmail','P') is not null
	drop procedure SetUserEmail
go
create procedure SetUserEmail
	@username as varchar(100),
	@email as nvarchar(255)
as
begin

	update UsersDetails
	set email = @email
	from UsersDetails as UD
		inner join Users as U
		on U.user_id = UD.user_id and U.username = @username

end



if object_id('SetUserEmailConfirmed','P') is not null
	drop procedure SetUserEmailConfirmed
go
create procedure SetUserEmailConfirmed
	@username as varchar(100),
	@email_confirmed as bit
as
begin

	update UsersDetails
	set email_confirmed = @email_confirmed
	from UsersDetails as UD
		inner join Users as U
		on U.user_id = UD.user_id and U.username = @username

end 
  