update schedules_data
set is_deleted = 1
where strftime('%w', begin_time) <> strftime('%w', end_time);



