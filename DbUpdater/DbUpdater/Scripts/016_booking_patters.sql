drop table if exists booking_permanent_data;
CREATE TABLE booking_permanent_data (
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,
	start_date date NOT NULL,
	end_date date,
	CHECK(end_date is null OR start_date < end_date)
);

drop table if exists booking_patterns;

CREATE TABLE booking_patterns (
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,	
	event_guid guid not null,
	horse_id INTEGER not null,
	coach_id INTEGER not null,
	client_id INTEGER not null,
	service_id INTEGER not null,
	is_deleted BOOLEAN default false not null,
	date_on date NOT NULL,
	begin_time time NOT NULL,
	end_time time NOT NULL,	
	permanent_data_id INTEGER not null,
	CHECK(begin_time < end_time),
	foreign key (client_id) REFERENCES clients(id),
	foreign key (coach_id) REFERENCES coaches(id),
	foreign key (horse_id) REFERENCES horses(id),
	foreign key (service_id) REFERENCES services(id),
	foreign key (permanent_data_id) REFERENCES booking_permanent_data(id)
);


drop index if exists i1_booking_patterns;
create index i1_booking_patterns on booking_patterns(horse_id);

drop index if exists i2_booking_patterns;
create index i2_booking_patterns on booking_patterns(coach_id);

drop index if exists i3_booking_patterns;
create index i3_booking_patterns on booking_patterns(client_id);

drop index if exists i4_booking_patterns;
create index i4_booking_patterns on booking_patterns(service_id);

drop index if exists i5_booking_patterns;
create index i5_booking_patterns on booking_patterns(permanent_data_id);

alter table bookings add column permanent_data_id INTEGER REFERENCES booking_patterns(permanent_data_id);
drop index if exists i5_bookings;
create index i5_bookings on bookings(permanent_data_id);

