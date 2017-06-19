CREATE TABLE users(
	id INTEGER PRIMARY KEY AUTOINCREMENT, 
	login varchar(32) not null, 
	name varchar(128) not null, 
	password varchar(255) not null, 
	new_password_on_login boolean,
	role_id integer not null, email varchar(50), phone varchar(50), 
	foreign key (role_id) REFERENCES user_roles(id)
);
