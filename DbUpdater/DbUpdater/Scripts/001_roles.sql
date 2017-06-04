drop table if exists user_roles;

create table user_roles(
	id INTEGER PRIMARY KEY, 
	name varchar(255)	
);

insert into user_roles(id, name)
values (0, "Administrator");

insert into user_roles(id, name)
values (1, "User");
