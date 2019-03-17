drop table if exists horse_to_coaches_link;
CREATE TABLE horse_to_coaches_link (
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,
	horse_id INTEGER NOT NULL,
	coach_id INTEGER NOT NULL,	
	foreign key (coach_id) REFERENCES coaches(id),
	foreign key (horse_id) REFERENCES horses(id)	
);

drop index if exists i1_horse_to_coaches_link;
create UNIQUE index i1_horse_to_coaches_link on horse_to_coaches_link(coach_id, horse_id);