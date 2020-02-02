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