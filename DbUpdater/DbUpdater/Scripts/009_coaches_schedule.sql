drop table if exists schedules_data;
drop table if exists schedules;
drop table if exists schedules_interval;

create TABLE schedules_interval(
	id INTEGER PRIMARY KEY not null,
	name varchar(50) not null
);

insert into schedules_interval(id, name) values (10, 'weekly');
insert into schedules_interval(id, name) values(20, 'daily');

CREATE TABLE schedules(
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,
	coach_id integer,
	horse_id integer,
	interval_id integer not null, 
	name varchar(50) not null,	
	is_active BOOLEAN DEFAULT true not null,	
	foreign key (coach_id) REFERENCES coaches(id),
	foreign key (horse_id) REFERENCES horses(id),
	foreign key (interval_id) REFERENCES schedules_interval(id),
	CHECK(COALESCE(coach_id, 0) + COALESCE(horse_id, 0) > 0)
	
	 
);

drop index if exists i1_schedules;
create index i1_schedules on schedules(coach_id);

drop index if exists i2_schedules;
create index i2_schedules on schedules(interval_id);

drop index if exists i3_schedules;
create index i3_schedules on schedules(horse_id);


CREATE TABLE schedules_data(
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,
	schedule_id integer not null, 	
	is_avialable BOOLEAN not null,
	availability_description varchar(50) not null,
	day_number integer,
	date_on date,
	begin_time time NOT NULL,
	end_time time NOT NULL,
	CHECK(begin_time < end_time),	
	CHECK((case when day_number is null then 0 else 1 end) + (case when date_on is null then 0 else 1 end) = 1)
	foreign key (schedule_id) REFERENCES schedules(id)	 
);

drop index if exists i1_schedules_data;
create index i1_schedules_data on schedules_data(schedule_id);
