drop index if exists unq1_users;
create unique index unq1_users on users(login);

insert into users(login, name, password, new_password_on_login, role_id)
values ('admin', 'Default System Administrator', 'not set', 1, 0);




