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


create table coach_roles (
	id INTEGER PRIMARY KEY not null,
	name varchar(255) not null 
);

insert into coach_roles(id, name) values (1, 'Qualified coaches');
insert into coach_roles(id, name) values (2, 'Staff member');

alter table coaches add column coach_role_id INTEGER REFERENCES coach_roles(id);
create index i1_coaches on coaches(coach_role_id);


create table coach_roles_to_services_link
(
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,
	coach_role_id INTEGER not null,
	service_id INTEGER not null,
	foreign key (coach_role_id) REFERENCES coach_roles(id),
	foreign key (service_id) REFERENCES services(id)
);

create index i1_coach_roles_to_services_link on coach_roles_to_services_link(coach_role_id);
create index i2_coach_roles_to_services_link on coach_roles_to_services_link(service_id);

