drop table if exists bookings_to_horses_link;
CREATE TABLE bookings_to_horses_link (
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,
	booking_id INTEGER NOT NULL,
	horse_id INTEGER NOT NULL,
	foreign key (booking_id) REFERENCES bookings(id),
	foreign key (horse_id) REFERENCES horses(id)	
);

drop index if exists i1_bookings_to_horses_link;
create UNIQUE index i1_bookings_to_horses_link on bookings_to_horses_link(booking_id, horse_id);


drop table if exists booking_templates_to_horses_link;
CREATE TABLE booking_templates_to_horses_link (
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,
	booking_template_id INTEGER NOT NULL,
	horse_id INTEGER NOT NULL,
	foreign key (booking_template_id) REFERENCES booking_templates(id),
	foreign key (horse_id) REFERENCES horses(id)	
);

drop index if exists i1_booking_templates_to_horses_link;
create UNIQUE index i1_booking_templates_to_horses_link on booking_templates_to_horses_link(booking_template_id, horse_id);