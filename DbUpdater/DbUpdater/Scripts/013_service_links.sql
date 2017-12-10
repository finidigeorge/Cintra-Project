drop table if exists service_to_coaches_link;
create table service_to_coaches_link
(
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,
	service_id integer not null,
	coach_id integer not null,
	is_deleted BOOLEAN default FALSE not null,
	foreign key (coach_id) REFERENCES coaches(id),
	foreign key (service_id) REFERENCES services(id)
);

drop index if exists i1_service_to_coaches_link;
create index i1_service_to_coaches_link on service_to_coaches_link(coach_id);

drop index if exists i2_service_to_coaches_link;
create index i2_service_to_coaches_link on service_to_coaches_link(service_id);

drop table if exists service_to_horses_link;
create table service_to_horses_link
(
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,
	service_id integer not null,
	horse_id integer not null,
	is_deleted BOOLEAN default FALSE not null,
	foreign key (horse_id) REFERENCES horses(id),
	foreign key (service_id) REFERENCES services(id)	
);

drop index if exists i1_service_to_horses_link;
create index i1_service_to_horses_link on service_to_horses_link(horse_id);

drop index if exists i2_service_to_horses_link;
create index i2_service_to_horses_link on service_to_horses_link(service_id);