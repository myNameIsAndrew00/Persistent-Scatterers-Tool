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