--liquibase formatted sql

--changeset srdjan.majstorovic:1
CREATE SCHEMA Crayon;

--changeset srdjan.majstorovic:2
create table  Crayon.Customer (
    Id int generated always as identity primary key not null,
    Name varchar(50) not null
)

--changeset srdjan.majstorovic:3

create table Crayon.User (
    Id int generated always as identity primary key not null,
    Email varchar(50) not null,
    Name varchar(50) not null,
    CustomerId int,

    CONSTRAINT FK_Customer 
    FOREIGN KEY (CustomerId) 
    REFERENCES Crayon.Customer (Id)
)

--changeset srdjan.majstorovic:4
create table  Crayon.Account (
    Id int generated always as identity primary key not null,
    Name varchar(50) not null, 
    CustomerId int not null,

    CONSTRAINT FK_Customer
      FOREIGN KEY (CustomerId) REFERENCES Crayon.Customer (Id)
)

--changeset srdjan.majstorovic:5
create table  Crayon.Order (
    Id int generated always as identity primary key not null,
    AccountId int not null,
    Status varchar(50) not null,
    FailureReason varchar(50) not null,
    CreatedAt timestamptz,
    UpdatedAt timestamptz,
    
    CONSTRAINT FK_Account
        FOREIGN KEY (AccountId) REFERENCES Crayon.Account (Id)

)

--changeset srdjan.majstorovic:6
create table  Crayon.OrderItem (
    Id int generated always as identity primary key not null,
    OrderId int not null,
    SoftwareId int not null,
    LicenceCount int not null,
    LicencedUntil timestamptz,
    
    CONSTRAINT FK_Order
        FOREIGN KEY (OrderId) REFERENCES Crayon.Order (Id)
)

--changeset srdjan.majstorovic:7
create table  Crayon.Subscription(
    Id int generated always as identity primary key not null,
    SoftwareName varchar(50) not null,
    Status varchar(50) not null,
    CreatedAt timestamptz not null
)

--changeset srdjan.majstorovic:8
create table  Crayon.Licence(
    Id int generated always as identity primary key not null,
    SubscriptionId int not null,
    ValidTo timestamptz not null,
    LicenceKey varchar(50) not null
)

--changeset srdjan.majstorovic:9
INSERT INTO Crayon.Customer (Name) VALUES ('Foo Co.');
INSERT INTO Crayon.Customer (Name) VALUES ('Bar Co.');
INSERT INTO Crayon.Customer (Name) VALUES ('Baz Ltd.');


INSERT INTO Crayon.User (Email, Name, CustomerId) 
  VALUES ('foouser@test.com', 'Mr Foo User', 1);
INSERT INTO Crayon.User (Email, Name, CustomerId) 
  VALUES ('basuser@test.com', 'Mrs Bar User', 2);
INSERT INTO Crayon.User (Email, Name, CustomerId) 
  VALUES ('bazuser@test.com', 'Mrs Baz User', 3);
 
INSERT INTO Crayon.Account (Name, CustomerId) VALUES ('Microsoft account', 1);
INSERT INTO Crayon.Account (Name, CustomerId) VALUES ('Google account', 1);
INSERT INTO Crayon.Account (Name, CustomerId) VALUES ('Amazon account', 2);
INSERT INTO Crayon.Account (Name, CustomerId) VALUES ('Google account', 2);
INSERT INTO Crayon.Account (Name, CustomerId) VALUES ('Adobe account', 3);
INSERT INTO Crayon.Account (Name, CustomerId) VALUES ('Azure account', 3);

--rollback empty
-- delete from Crayon.Account where 1=1
-- delete from Crayon.User where 1=1
-- delete from Crayon.Customer where 1=1