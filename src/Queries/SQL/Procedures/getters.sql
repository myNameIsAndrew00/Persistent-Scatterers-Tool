/*Users*/
if object_id('GetUserPasswordInfo','P') is not null
	drop procedure GetUserPasswordInfo
go
create procedure GetUserPasswordInfo
	@username as varchar(100)
as
begin
	select U.hashed_password
	from Users as U
	where U.username = @username

end

go

if object_id('GetUser','P') is not null
	drop procedure GetUser
go
create procedure GetUser
	@username as varchar(100)
as
begin
	select U.username,
		   UD.first_name,
		   UD.last_name,
		   UD.timestamp
	from Users as U
		inner join UsersDetails as UD
		on UD.user_id = U.user_id
	where U.username = @username

end

go

if object_id('GetUserRoles','P') is not null
	drop procedure GetUserRoles
go
create procedure GetUserRoles
	@username as varchar(100)
as
begin
	select R.role_name
	from UsersRoles as UR
		inner join Users as U
		on UR.user_id = U.user_id and U.username = @username
			inner join Roles as R
			on R.role_id = UR.role_id
	where U.username = @username

end

go
if object_id('GetUserPointsDatasetID','P') is not null
	drop procedure GetUserPointsDatasetID
go
create procedure GetUserPointsDatasetID
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
 
if object_id('GetUserPointsDataset','P') is not null
	drop procedure GetUserPointsDataset
go
create procedure GetUserPointsDataset
	@username as varchar(100),
	@dataset_name as varchar(100)
as
begin
	select DS.data_set_id,
		   DS.dataset_name,
		   DS.maximum_latitude,
		   DS.maximum_longitude,
		   DS.minimum_latitude,
		   DS.minimum_longitude,
		   DS.status_id 
	from DataSets as DS
		inner join Users as U
		on DS.user_id = U.user_id and U.username = @username
	where DS.dataset_name = @dataset_name

end

go



/*Color palettes*/

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

go

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

if object_id('GetColorPalettesFiltered', 'P') is not null
	drop procedure GetColorPalettesFiltered
go
create procedure GetColorPalettesFiltered 
	@filter_id as int,
	@filter_value as varchar(255),
	@page_index as int,
	@items_per_page as int
as 
begin
	select 
		   U.username,
		   CP.palette_name,
		   CP.palette_serialization from ColorPalettes as CP
		inner join Users as U
		on CP.user_id = U.user_id 
	where CHARINDEX(@filter_value,
				( case 
					when @filter_id = 1 then CP.palette_name
					when @filter_id = 2 then U.username
				end )) > 0 
		   OR @filter_id = -1
	order by CP.creation_date desc
	OFFSET @page_index * @items_per_page ROWS FETCH NEXT @items_per_page ROWS ONLY;
end
 
go


if object_id('GetDataSetsFiltered', 'P') is not null
	drop procedure GetDataSetsFiltered
go
create procedure GetDataSetsFiltered 
	@filter_id as int,
	@filter_value as varchar(255),
	@page_index as int,
	@items_per_page as int
as 
begin
	select 
		   U.username,
		   DS.dataset_name,		
		   DS.data_set_id as dataset_id,
		   DS.status_id
		from DataSets as DS
		inner join Users as U
		on DS.user_id = U.user_id 
	where CHARINDEX(@filter_value,
				( case 
					when @filter_id = 1 then DS.dataset_name
					when @filter_id = 2 then U.username
				end )) > 0 
		   OR @filter_id = -1
	order by DS.data_set_id desc
	OFFSET @page_index * @items_per_page ROWS FETCH NEXT @items_per_page ROWS ONLY;
end
 
go

if object_id('GetUserColorPalette', 'P') is not null
	drop procedure GetUserColorPalette
go
create procedure GetUserColorPalette 
	@username as varchar(100),
	@palette_name as varchar(255)	 
as 
begin
	select CP.palette_serialization
		from ColorPalettes as CP
		inner join Users as U
			on U.username = @username and U.user_id = CP.user_id
	where CP.palette_name = @palette_name			
end
 




