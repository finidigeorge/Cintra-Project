PRAGMA foreign_keys = ON;

drop table if exists payment_types;
drop table if exists booking_payments;
drop table if exists bookings;

CREATE TABLE bookings (
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
	CHECK(begin_time < end_time),
	foreign key (client_id) REFERENCES clients(id),
	foreign key (coach_id) REFERENCES coaches(id),
	foreign key (horse_id) REFERENCES horses(id),
	foreign key (service_id) REFERENCES services(id)
);


CREATE TABLE payment_types (
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,
	name varchar(50) not null,
	is_deleted BOOLEAN default false not null
);

INSERT INTO payment_types(id, name, is_deleted) VALUES(0, 'Block', 0);
INSERT INTO payment_types(id, name, is_deleted) VALUES(1, 'Doing BT', 0);
INSERT INTO payment_types(id, name, is_deleted) VALUES(2, 'Gift voucher', 0);
INSERT INTO payment_types(id, name, is_deleted) VALUES(3, 'Bookme', 0);
INSERT INTO payment_types(id, name, is_deleted) VALUES(4, 'Lezair', 0);

CREATE TABLE booking_payments (
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,	
	booking_id INTEGER not null,
	payment_type_id INTEGER not null,
	isPaid boolean NOT NULL,
	paymentOptions varchar(200),
	is_deleted BOOLEAN default false not null,
	foreign key (payment_type_id) REFERENCES payment_types(id)
	foreign key (booking_id) REFERENCES bookings(id)
);


drop index if exists i1_bookings;
create index i1_bookings on bookings(horse_id);

drop index if exists i2_bookings;
create index i2_bookings on bookings(coach_id);

drop index if exists i3_bookings;
create index i3_bookings on bookings(client_id);

drop index if exists i4_bookings;
create index i4_bookings on bookings(service_id);

drop index if exists i1_booking_payments;
create index i1_booking_payments on booking_payments(booking_id);

drop index if exists i2_booking_payments;
create index i2_booking_payments on booking_payments(payment_type_id);
