delete from service_to_coaches_link;
delete from service_to_horses_link;

DROP TABLE services;

CREATE TABLE services (
	id INTEGER PRIMARY KEY AUTOINCREMENT not null, 
	name varchar(255) not null, 
	is_deleted boolean default false not null, 
	length_minutes INTEGER, 
	begin_time date, 
	end_time date,
	CHECK(begin_time is null || begin_time < end_time),
	CHECK((length_minutes is null and begin_time is not null and end_time is not null) || (length_minutes is not null and begin_time is null and end_time is null))
);
