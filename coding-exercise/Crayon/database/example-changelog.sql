--liquibase formatted sql

--changeset srdjan.majstorovic:0
CREATE SCHEMA Crayon;
--rollback Drop schema Crayon cascade;

--changeset srdjan.majstorovic:1

create table Crayon.User (
    Id varchar(50) primary key not null,
    Email varchar(50) not null,
    CustomerId int
)

--rollback DROP TABLE  Crayon.User;


--changeset srdjan.majstorovic:2
create table  Crayon.Customer (
    Id int generated always as identity primary key not null,
    Name varchar(50) not null
)

--rollback DROP TABLE  Crayon.Customer; 

--changeset srdjan.majstorovic:3
alter table Crayon.User 
   ADD CONSTRAINT FK_Customer 
   FOREIGN KEY (CustomerId) 
   REFERENCES Crayon.Customer (Id)

--rollback ALTER TABLE IF EXISTS crayon.user DROP CONSTRAINT IF EXISTS fk_customer;


--changeset srdjan.majstorovic:5
create table  Crayon.Account (
    Id int generated always as identity primary key not null,
    CustomerId int not null,

    CONSTRAINT FK_Customer
      FOREIGN KEY (CustomerId) REFERENCES Crayon.Customer (Id)
)

--rollback DROP TABLE  Crayon.Account;


--changeset srdjan.majstorovic:6
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

--rollback DROP TABLE  Crayon.Order;


--changeset srdjan.majstorovic:7
create table  Crayon.OrderItem (
    Id int generated always as identity primary key not null,
    OrderId int not null,
    SoftwareId int not null,
    LicenceCount int not null,
    LicencedUntil timestamptz,
    
    CONSTRAINT FK_Order
        FOREIGN KEY (OrderId) REFERENCES Crayon.Order (Id)
)

--rollback DROP TABLE  Crayon.OrderItem;


--changeset srdjan.majstorovic:8
create table  Crayon.Subscription(
    Id int generated always as identity primary key not null,
    SoftwareName varchar(50) not null,
    Status varchar(50) not null,
    CreatedAt timestamptz not null
)

--rollback DROP TABLE  Crayon.Subscription;


--changeset srdjan.majstorovic:9
create table  Crayon.Licence(
    Id int generated always as identity primary key not null,
    SubscriptionId int not null,
    ValidTo timestamptz not null,
    LicenceKey varchar(50) not null
)


--rollback DROP TABLE  Crayon.Licence;
