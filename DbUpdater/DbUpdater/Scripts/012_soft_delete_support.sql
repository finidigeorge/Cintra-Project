alter table coaches add is_deleted boolean default false not null;
alter table horses add is_deleted boolean default false not null;
alter table schedules add is_deleted boolean default false not null;
alter table schedules_data add is_deleted boolean default false not null;
alter table services add is_deleted boolean default false not null;
alter table users add is_deleted boolean default false not null;
alter table user_roles add is_deleted boolean default false not null;
