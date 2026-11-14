CREATE TABLE households (id INTEGER PRIMARY KEY, name TEXT NOT NULL, eligible INTEGER NOT NULL);
CREATE TABLE allocations (household_id INTEGER, batch TEXT NOT NULL);
INSERT INTO households VALUES (101,'Lan',1),(102,'Minh',1),(103,'Hoa',1),(104,'Nam',0),(105,'An',1);
INSERT INTO allocations VALUES (102,'B1'),(NULL,'legacy-import'),(105,'B1');
