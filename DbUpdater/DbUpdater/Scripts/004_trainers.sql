CREATE TABLE trainers (
	id INTEGER PRIMARY KEY AUTOINCREMENT, 
	name varchar(128) not null, 
	email varchar(50), 
	phone varchar(50)
);

CREATE TABLE trainer_schedules(
	id INTEGER PRIMARY KEY AUTOINCREMENT,
	trainer_id integer not null, 
	is_avialable BOOLEAN not null,
	availability_status varchar(50) not null,
	begin_time time NOT NULL,
	end_time time NOT NULL,
	foreign key (trainer_id) REFERENCES trainers(id)
	CHECK(begin_time < end_time) 
);

drop index if exists i1_trainer_schedules;
create unique index i1_trainer_schedules on trainer_schedules(trainer_id);