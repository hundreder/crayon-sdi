--liquibase formatted sql

--changeset myuser:1
CREATE TABLE foo (
    id SERIAL PRIMARY KEY,
    data VARCHAR(100)
);
INSERT INTO foo (data) VALUES ('some data');

--rollback
DELETE FROM foo WHERE data = 'some data';
DROP TABLE foo;