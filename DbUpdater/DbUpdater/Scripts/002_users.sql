drop table if exists users;

create table users(
	id INTEGER PRIMARY KEY AUTOINCREMENT, 
	login varchar(32) not null, 
	name varchar2(128) not null, 
	password varchar2(255) not null, 
	new_password_on_login boolean,
	role_id integer not null, 
	foreign key (role_id) REFERENCES user_roles(id)
);

create index i1_users on users(role_id);
create index i2_users on users(login);
create index i3_users on users(name);
