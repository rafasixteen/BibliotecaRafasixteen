create table BookAuthor (
	BookISBN text not null,
	AuthorId int not null,
	
	primary key (BookISBN, AuthorId),
	foreign key (BookISBN) references Books(ISBN),
	foreign key (AuthorId) references Authors(Id)
);