drop table if exists bookings_to_coaches_link;
CREATE TABLE bookings_to_coaches_link (
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,
	booking_id INTEGER NOT NULL,
	coach_id INTEGER NOT NULL,
	foreign key (booking_id) REFERENCES bookings(id),
	foreign key (coach_id) REFERENCES coaches(id)	
);

drop index if exists i1_bookings_to_coaches_link;
create UNIQUE index i1_bookings_to_coaches_link on bookings_to_coaches_link(booking_id, coach_id);


drop table if exists booking_templates_to_coaches_link;
CREATE TABLE booking_templates_to_coaches_link (
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,
	booking_template_id INTEGER NOT NULL,
	coach_id INTEGER NOT NULL,
	foreign key (booking_template_id) REFERENCES booking_templates(id),
	foreign key (coach_id) REFERENCES coaches(id)	
);

drop index if exists i1_booking_templates_to_coaches_link;
create UNIQUE index i1_booking_templates_to_coaches_link on booking_templates_to_coaches_link(booking_template_id, coach_id);

