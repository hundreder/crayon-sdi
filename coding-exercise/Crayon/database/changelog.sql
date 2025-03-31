--liquibase formatted sql

--changeset srdjan.majstorovic:1
CREATE SCHEMA crayon;

--changeset srdjan.majstorovic:2

create table crayon.user (
    id int generated always as identity primary key not null,
    email varchar(50) not null,
    name varchar(50) not null
);

--changeset srdjan.majstorovic:3
create table  crayon.customer (
    id int primary key not null,
    name varchar(50) not null,
    
    CONSTRAINT FK_user 
    FOREIGN KEY (id) 
    REFERENCES crayon.user (id)
);

--changeset srdjan.majstorovic:4
create table  crayon.account (
    id int generated always as identity primary key not null,
    name varchar(50) not null, 
    customer_id int not null,

    CONSTRAINT FK_customer
      FOREIGN KEY (customer_id) REFERENCES crayon.customer (id)
);

--changeset srdjan.majstorovic:5
create table  crayon.order (
    id int generated always as identity primary key not null,
    account_id int not null,
    status varchar(50) not null,
    external_order_id varchar(50),
    failure_reason varchar(50),
    created_at timestamptz not null,
    updated_at timestamptz,
    
    CONSTRAINT FK_account
        FOREIGN KEY (account_id) REFERENCES crayon.account (id)

);

--changeset srdjan.majstorovic:6
create table  crayon.order_item (
    id int generated always as identity primary key not null,
    order_id int not null,
    software_id int not null,
    licence_count int not null,
    licence_valid_to timestamptz,
    
    CONSTRAINT FK_Order
        FOREIGN KEY (order_id) REFERENCES crayon.Order (id)
);

--changeset srdjan.majstorovic:7
create table  crayon.Subscription(
    id int generated always as identity primary key not null,
    account_id int not null,
    software_id int not null,
    software_name varchar(50) not null,
    status varchar(50) not null,
    created_at timestamptz not null,
    updated_at timestamptz,

    CONSTRAINT fk_account
        FOREIGN KEY (account_id) REFERENCES crayon.account (id)
);

--changeset srdjan.majstorovic:8
create table  crayon.Licence(
    id int generated always as identity primary key not null,
    subscription_id int not null,
    valid_to timestamptz not null,
    licence_key varchar(50) not null,
    licence_count int not null,

    CONSTRAINT fk_subscription
        FOREIGN KEY (subscription_id) REFERENCES crayon.subscription (id)
);

--changeset srdjan.majstorovic:9
INSERT INTO crayon.user (Email, name) 
  VALUES ('foouser@test.com', 'Mr Foo user');
INSERT INTO crayon.user (Email, name) 
  VALUES ('baruser@test.com', 'Mrs Bar user');
INSERT INTO crayon.user (Email, name) 
  VALUES ('bazuser@test.com', 'Mrs Baz user');
INSERT INTO crayon.user (Email, name) 
  VALUES ('nocustuser@test.com', 'Mr No Customer');

INSERT INTO crayon.customer (id, name) VALUES (1, 'Foo Co.');
INSERT INTO crayon.customer (id, name) VALUES (2, 'Bar Co.');
INSERT INTO crayon.customer (id, name) VALUES (3, 'Baz Ltd.');


INSERT INTO crayon.account (name, customer_id) VALUES ('Microsoft account', 1);
INSERT INTO crayon.account (name, customer_id) VALUES ('Google account', 1);
INSERT INTO crayon.account (name, customer_id) VALUES ('Amazon account', 2);
INSERT INTO crayon.account (name, customer_id) VALUES ('Google account', 2);
INSERT INTO crayon.account (name, customer_id) VALUES ('Adobe account', 3);
INSERT INTO crayon.account (name, customer_id) VALUES ('Azure account', 3);

--rollback empty
-- delete from crayon.account where 1=1
-- delete from crayon.user where 1=1
-- delete from crayon.customer where 1=1