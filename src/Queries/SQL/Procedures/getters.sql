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
	@username as varchar(100),
	@email as nvarchar(255)
as
begin
	select U.username,
		   UD.first_name,
		   UD.last_name,
		   UD.timestamp,
		   UD.email,
		   UD.email_confirmed
	from Users as U
		inner join UsersDetails as UD
		on UD.user_id = U.user_id
	where U.username = @username or UD.email = @email

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
	@dataset_name as varchar(100),
	@dataset_id as int
as
begin
	select DS.data_set_id,
		   DS.dataset_name,
		   DS.maximum_latitude,
		   DS.maximum_longitude,
		   DS.minimum_latitude,
		   DS.minimum_longitude,
		   DS.status_id,
		   DS.minimum_height,
		   DS.maximum_height,
		   DS.minimum_def_rate,
		   DS.maximum_def_rate,
		   DS.minimum_std_dev,
		   DS.maximum_std_dev,
		   DS.source_name,
		   U.username
	from DataSets as DS
		inner join Users as U
		on DS.user_id = U.user_id 
	where (DS.dataset_name = @dataset_name and U.username = @username) or DS.data_set_id = @dataset_id

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
		   DS.status_id, 
		   DS.source_name
		from DataSets as DS
		inner join Users as U
		on DS.user_id = U.user_id 
	where CHARINDEX(@filter_value,
				( case 
					when @filter_id = 1 then DS.dataset_name
					when @filter_id = 2 then U.username					
					when @filter_id = 3 then DS.source_name
				end )) > 0 
		   OR @filter_id = -1
	order by DS.data_set_id desc
	OFFSET @page_index * @items_per_page ROWS FETCH NEXT @items_per_page ROWS ONLY;
end
 
go

if object_id('GetUserGeoserverPointsDataasetID', 'P') is not null
	drop procedure GetUserGeoserverPointsDataasetID
go
create procedure GetUserGeoserverPointsDataasetID 
	@dataset_id int
as 
begin
	select GDS.geoserver_dataset_id
		from GeoserverDataSets as GDS
			where GDS.data_set_id = @dataset_id
				
end






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
		   CP.palette_serialization,
		   CP.status_mask
		from ColorPalettes as CP
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
		   CP.palette_serialization,
		   CP.status_mask
		from ColorPalettes as CP
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
 
go

if object_id('GetGeoserverColorPalettes', 'P') is not null
	drop procedure GetGeoserverColorPalettes
go
create procedure GetGeoserverColorPalettes 
	@geoserver_dataset_id int
as 
begin
	select 
		   U.username,
		   CP.palette_name,
		   CP.palette_serialization,
		   CP.status_mask
		from ColorPalettes as CP
		inner join Users as U
		on CP.user_id = U.user_id  
			inner join GeoserverDataSetsPalettes as GDSP
				on CP.color_palette_id = GDSP.color_palette_id
					inner join GeoserverDataSets as GDS
						on GDSP.geoserver_dataset_id = GDS.geoserver_dataset_id
						and GDS.geoserver_dataset_id = @geoserver_dataset_id
				
end




