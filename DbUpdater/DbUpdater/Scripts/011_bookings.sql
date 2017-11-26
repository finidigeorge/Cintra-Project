drop table if exists booking_payments;
drop table if exists bookings;

CREATE TABLE bookings (
	id INTEGER PRIMARY KEY not null,	
	event_guid guid not null,
	horse_id INTEGER not null,
	coach_id INTEGER not null,
	client_id INTEGER not null,
	service_id INTEGER not null,
	is_deleted BOOLEAN default false not null,
	date_on date NOT NULL,
	begin_time time NOT NULL,
	end_time time NOT NULL,	
	foreign key (client_id) REFERENCES clients(id),
	foreign key (coach_id) REFERENCES coaches(id),
	foreign key (horse_id) REFERENCES horses(id),
	foreign key (service_id) REFERENCES services(id)
);

CREATE TABLE booking_payments (
	id INTEGER PRIMARY KEY not null,	
	booking_id INTEGER not null,
	isPaid boolean NOT NULL,
	paymentOptions varchar(200),
	is_deleted BOOLEAN default false not null,
	foreign key (booking_id) REFERENCES bookings(id)
);
