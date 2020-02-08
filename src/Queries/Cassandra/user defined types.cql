create type points_displacements (
	date timestamp,
	jd decimal,
	days_from_reference decimal,
	value decimal
);
 
 
create type base_point (
	number int,  
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text
)