drop table if exists bookings_template_metadata;
CREATE TABLE bookings_template_metadata (
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,
	start_date date NOT NULL,
	end_date date,
	CHECK(end_date is null OR start_date < end_date)
);

drop table if exists booking_templates;

CREATE TABLE booking_templates (
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,	
	event_guid guid not null,
	horse_id INTEGER not null,
	client_id INTEGER not null,
	service_id INTEGER not null,
	is_deleted BOOLEAN default false not null,
	day_of_week INTEGER NOT NULL,
	begin_time time NOT NULL,
	end_time time NOT NULL,	
	template_metadata_id INTEGER not null,
	CHECK(begin_time < end_time),
	foreign key (client_id) REFERENCES clients(id),
	foreign key (horse_id) REFERENCES horses(id),
	foreign key (service_id) REFERENCES services(id),
	foreign key (template_metadata_id) REFERENCES bookings_template_metadata(id)
);


drop index if exists i1_booking_templates;
create index i1_booking_templates on booking_templates(horse_id);

drop index if exists i3_booking_templates;
create index i3_booking_templates on booking_templates(client_id);

drop index if exists i4_booking_templates;
create index i4_booking_templates on booking_templates(service_id);

drop index if exists i5_booking_templates;
create index i5_booking_templates on booking_templates(template_metadata_id);

alter table bookings add column template_metadata_id INTEGER REFERENCES bookings_template_metadata(id);
drop index if exists i5_bookings;
create index i5_bookings on bookings(template_metadata_id);

alter table bookings add column day_of_week INTEGER;


