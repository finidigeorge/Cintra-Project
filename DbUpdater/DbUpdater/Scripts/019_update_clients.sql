PRAGMA foreign_keys=off;
begin transaction;

ALTER TABLE Clients RENAME TO temp_table;
 
CREATE TABLE clients(
	id INTEGER PRIMARY KEY AUTOINCREMENT not null,
	name varchar(50) not null,
	email varchar(50),
	phone varchar(50),
	age varchar(50),
	weight varchar(10),
	height varchar(10),
	contact_details varchar(200),
	is_deleted BOOLEAN default FALSE not null
);
 
INSERT INTO clients (id, name, email, phone, age, weight, height, contact_details, is_deleted)
  SELECT id, name, email, phone, age, weight, height, contact_details, is_deleted
  FROM temp_table;
 
DROP TABLE temp_table;

commit;
PRAGMA foreign_keys=on;

