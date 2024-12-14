create table Authors (
	Id integer primary key autoincrement,
	Name text not null unique,
);