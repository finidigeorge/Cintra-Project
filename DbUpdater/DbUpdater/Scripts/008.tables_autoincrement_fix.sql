
drop table services;

CREATE TABLE services(
	id INTEGER PRIMARY KEY AUTOINCREMENT not null, 
	name varchar(255) not null	
);

drop table horses;

CREATE TABLE horses(
	id INTEGER PRIMARY KEY AUTOINCREMENT not null, 
	nickname varchar(255) not null
);

