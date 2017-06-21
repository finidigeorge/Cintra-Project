drop index if exists unq1_users;
create unique index unq1_users on users(login);

insert into users(login, name, password, new_password_on_login, role_id)
values ('admin', 'Default System Administrator', 'not set', 1, 0);

update users
set password = '9a4dbf6c15ab979cee8df267d2005c2362c844e6d4f0b0f35d397ad521286e34c99b7b143d435357db6873eb43cf26e5',
	new_password_on_login = 0
where login = 'admin';

alter table users add email varchar(50);
alter table users add phone varchar(50);





