alter table services add max_clients INTEGER default 6 not null;
alter table services add max_coaches INTEGER default 6 not null;
alter table services add max_horses INTEGER default 6 not null;