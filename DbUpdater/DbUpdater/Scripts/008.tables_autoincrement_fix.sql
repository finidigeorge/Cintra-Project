
drop table services;

CREATE TABLE services(
	id INTEGER PRIMARY KEY AUTOINCREMENT not null, 
	name varchar(255) not null	
);

insert into services(name) values ('Park Ride');
insert into services(name) values ('Beginners raiding lession');

drop table horses;

CREATE TABLE horses(
	id INTEGER PRIMARY KEY AUTOINCREMENT not null, 
	nickname varchar(255) not null
);

insert into horses(nickname) values ('BB');