drop table if exists clients;

create table clients(
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,
	name varchar(50) not null,
	email varchar(50),
	phone varchar(50),
	age number not null,
	weight varchar(10),
	height varchar(10),
	contact_details varchar(200),
	is_deleted BOOLEAN default FALSE not null,
	CHECK((email is not null) OR (phone is not null))
);

