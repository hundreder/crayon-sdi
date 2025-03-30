# crayon-sdi
System design interview project for Crayon Senior .Net developer role

# Requirements
Requirements are located in /requirements folder

`Crayon System Design Interview Question 1.pdf`

`Crayon Technical Exercise Interview Question 1.pdf`

# Crayon system design interview Question 1
Solution of this task can be found in system-design-task folder. Filder has 3 files with same content but of different type. For most efficient viewof solution go to https://excalidraw.com/ ,click Open in the left side burger menu and choose `Crayon - system design task.excalidraw` file.

# Crayon Technical Exercise Interview Question 1
Swaggeer http://localhost:5072/swagger/index.html


# Prerequisites
 - Containter management tool like Rancher Desktop 
 
    https://docs.rancherdesktop.io/getting-started/installation
    
 - Liquibase

    https://docs.liquibase.com/start/install/liquibase-macos.html


# PostgresSQL database setup
Run the following docker command to spin up new instance of postgres db

```docker
docker run --name crayon-postgres \
    -e POSTGRES_USER=crayon \
    -e POSTGRES_PASSWORD=somepassword \
    -e POSTGRES_DB=CrayonDb \
    -p 5432:5432 \
    -d postgres:latest
```

To create database schema run
-  ```liquibase update```

Usefull liquibase commands
-   ```liquibase updateTestingRollback``` - run all changesets and rolls them back
-   ```liquibase validate``` - validates changesets


DB is seeded with several users, customers and accounts.


# Login
Login e
Emails:
- foouser@test.com
- baruser@test.com
- baruser@test.com

Password is: ```123123```



# Troubleshooting
If liquibase partialy suceedes or you want to reset db to initial state run this SQL script
```SQL
DELETE FROM public.databasechangelog	
WHERE 1=1;

Drop schema crayon cascade;
```
Than run: ```liquibase update``` in terminal