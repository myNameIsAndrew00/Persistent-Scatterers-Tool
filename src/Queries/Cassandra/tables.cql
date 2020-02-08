create table points_by_dataset(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	displacements list<FROZEN<points_displacements>>,
	
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);



 
create table points_region_zoom_20(
	dataset_id int,
	row int,
	column int,
	
	section int,
	
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_19(
	dataset_id int,
	row int,
	column int,
	
	section int,
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_18(
	dataset_id int,
	row int,
	column int,
	
	section int,
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_17(
	dataset_id int,
	row int,
	column int,
	section int,
	
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_16(
	dataset_id int,
	row int,
	column int,
	section int,
	
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_15(
	dataset_id int,
	row int,
	column int,
	
	section int,
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_14(
	dataset_id int,
	row int,
	column int,
	section int,
	
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_13(
	dataset_id int,
	row int,
	column int,
	
	section int,
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_12(
	dataset_id int,
	row int,
	column int,
	
	section int,
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_11(
	dataset_id int,
	row int,
	column int,
	
	section int,
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_10(
	dataset_id int,
	row int,
	column int,
	
	section int,
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_9(
	dataset_id int,
	row int,
	column int,
	
	section int,
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_8(
	dataset_id int,
	row int,
	column int,
	section int,
	
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_7(
	dataset_id int,
	row int,
	column int,
	section int,
	
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_6(
	dataset_id int,
	row int,
	column int,
	section int,
	
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_5(
	dataset_id int,
	row int,
	column int,
	section int,
	
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_4(
	dataset_id int,
	row int,
	column int,
	section int,
	
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);

create table points_region_zoom_3(
	dataset_id int,
	row int,
	column int,
	section int,
	
	points list<FROZEN<base_point>>,
	
	PRIMARY KEY((dataset_id, row, column), section)
);
 






## Tables below are not required anymore

create table points_by_dataset_zoom_3(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);


create table points_by_dataset_zoom_4(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);


create table points_by_dataset_zoom_5(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);


create table points_by_dataset_zoom_6(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);


create table points_by_dataset_zoom_7(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);



create table points_by_dataset_zoom_8(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);


create table points_by_dataset_zoom_9(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);


create table points_by_dataset_zoom_10(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);




create table points_by_dataset_zoom_11(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);


create table points_by_dataset_zoom_12(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);


create table points_by_dataset_zoom_13(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);


create table points_by_dataset_zoom_14(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);


create table points_by_dataset_zoom_15(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);



create table points_by_dataset_zoom_16(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);


create table points_by_dataset_zoom_17(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);


create table points_by_dataset_zoom_18(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);


create table points_by_dataset_zoom_19(
	dataset_id int,
	number int,
	longitude decimal,
	latitude decimal,
	height decimal,
	deformation_rate decimal,
	standard_deviation decimal,
	estimated_height decimal,	
	estimated_deformation_rate decimal,
	observations text,
	PRIMARY KEY((dataset_id, number), longitude, latitude)
) WITH CLUSTERING ORDER BY(longitude desc, latitude desc);


 
