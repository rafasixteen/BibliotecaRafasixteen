create table Books (
	ISBN text primary key not null unique,
	Title text not null,
	PublisherId int not null,

	foreign key (PublisherId) references Publishers(Id)
);