drop table if exists horses_unavailability_types;
create table horses_unavailability_types
(
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,	
	type_name VARCHAR(50),
	is_deleted BOOLEAN default FALSE not null
);

INSERT INTO horses_unavailability_types(id, type_name, is_deleted) VALUES(0, 'On holiday', 0);
INSERT INTO horses_unavailability_types(id, type_name, is_deleted) VALUES(1, 'Sick', 0);
INSERT INTO horses_unavailability_types(id, type_name, is_deleted) VALUES(2, 'Day off', 0);


drop table if exists horses_schedule_data;
create table horses_schedule_data
(
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,	
	horse_id integer not null,
	unavailability_type_id integer not null,
	is_deleted BOOLEAN default FALSE not null,
	start_date DATE not null,
	end_date DATE  not null,
	foreign key (horse_id) REFERENCES horses(id),
	foreign key (unavailability_type_id) REFERENCES horses_unavailability_types(id)
);

drop index if exists i1_horses_schedule_data;
create index i1_horses_schedule_data on horses_schedule_data(horse_id);

drop index if exists i2_horses_schedule_data;
create index i2_horses_schedule_data on horses_schedule_data(unavailability_type_id);