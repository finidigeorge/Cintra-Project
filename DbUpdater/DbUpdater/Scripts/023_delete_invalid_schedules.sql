update schedules_data
set is_deleted = 1
where strftime('%w', begin_time) <> strftime('%w', end_time);

update schedules_data
set is_deleted = 0
where is_deleted = 'false';

update schedules
set is_active = 1
where is_active = 'true';

update schedules
set is_active = 0
where is_active = 'false';

update schedules
set is_active = 1
where id = 22;

update schedules
set is_active = 1
where is_deleted = 0;


update schedules
set is_deleted = 0
where is_deleted = 'false';


