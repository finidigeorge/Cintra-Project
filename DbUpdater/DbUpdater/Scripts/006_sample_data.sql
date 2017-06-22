insert into users(login, name, password, new_password_on_login, role_id, email, phone, is_locked)
values ('user', 'Test Regular User', '134f4f6737c1102b1d0e5e54a5379f4107848e87f6cb756b8208ce3f2275b86c1f1f376974316115ba03bc7cb3d70ce6', 0, 1, 'test@gmail.com', '0123456789', 0);

insert into horses(nickname) values ('BB');
insert into horses(nickname) values ('Angus');

insert into services(name) values ('Park Ride');
insert into services(name) values ('Beginners raiding lession');

insert into trainers(name, email, phone)
values ('Trainer1', 'trainer1@gmail.com', '012345678');

insert into trainer_schedules(trainer_id, is_avialable, availability_status, begin_time, end_time) 
values ((SELECT last_insert_rowid() FROM trainers), 1, 'Working hours', time('08:00'), time('12:00'))

insert into trainer_schedules(trainer_id, is_avialable, availability_status, begin_time, end_time) 
values ((SELECT last_insert_rowid() FROM trainers), 0, 'Lunch time', time('12:00'), time('14:00'))

insert into trainer_schedules(trainer_id, is_avialable, availability_status, begin_time, end_time) 
values ((SELECT last_insert_rowid() FROM trainers), 1, 'Working hours', time('14:00'), time('19:00'))




