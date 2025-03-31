# crayon-sdi
System design interview project for Crayon Senior .Net developer role

# Requirements
Requirements are located in:

`/requirements/Crayon System Design Interview Question 1.pdf`

`/requirements/Crayon Technical Exercise Interview Question 1.pdf`

# Crayon system design interview Question 1 - Solution
Solution of this task can be found in folder

`/system-design-task`

Folder has 3 files with same content but of different type (png, svg and excalidraw). 

For most efficient view of the solution go to https://excalidraw.com/  ,click Open in the left side burger menu and browse to `Crayon - system design task.excalidraw` file.

# Crayon Technical Exercise Interview Question 1 - Solution
Code is located in /coding-exercise folder.


## Prerequisites
 - .net 9 sdk installed
 - IDE would help browsing the code / debugging and DB querying
 - Docker (Containter management tool like Rancher Desktop would help too)
 
    https://docs.rancherdesktop.io/getting-started/installation

 - Liquibase (optional)

    https://docs.liquibase.com/start/install/liquibase-macos.html


## PostgresSQL database setup

### Run instance of PostgresSql server
Run the following docker command to spin up new instance of postgres db

```docker
docker run --name crayon-postgres \
    -e POSTGRES_USER=crayon \
    -e POSTGRES_PASSWORD=somepassword \
    -e POSTGRES_DB=CrayonDb \
    -p 6432:5432 \
    -d postgres:latest
```

### 1. To create database schema using liquibase
```
liquibase update
```

### 2. To create database schema using db script

Open DB Management tool (i.e. PgAdmin) and run script located in
```
coding-exercise/database/changelog.sql
```

After that db is ready. Shcema is created, and basic data is seeded.

## Starting the application

### Using dotnet run
From the root of repository run:
```
dotnet run --project  coding-exercise/Crayon/Crayon.API
```

### Using IDE
Open Crayon.slm using .net IDE and hit Run/Debug (F5)

### Using docker 
Not working atm. Conection from app container to db container has to be fixed.

Build an image. Go to coding-exercise/Crayon and run 
```docker
docker build -t crayon-api .
```


Start the image
```docker
docker run \
   -d \
   --add-host=host.docker.internal:host-gateway \
   -p 8080:8080 \
   -e ASPNETCORE_ENVIRONMENT="Development"  \
   -e AppSettings__ConnectionString="Server=host.docker.internal:6432;Database=CrayonDb;User ID=crayon;Password=somepassword"  \
   --name crayon-api-container \
   crayon-api:latest
```


# Using the API
To browse the api documentation go to http://localhost:8080/swagger/index.html
Public endpoints dont require jwt to call. Secure ones require JWT.


# Login
Several users with their coresponding customer and accounts are seeded in database.
To login on ```/api/v1/login``` use any of the below creadentials:
```
foouser@test.com
baruser@test.com
baruser@test.com
```
Password is: ```123123```

Returned jwt should be added as Authorization header when calling Secured endpoints



# Troubleshooting
If liquibase partialy suceedes or you want to reset db to initial state run this SQL script
```SQL
DELETE FROM public.databasechangelog	
WHERE 1=1;

Drop schema crayon cascade;
```
Than run: ```liquibase update``` in terminal


Usefull liquibase commands
-   ```liquibase updateTestingRollback``` - run all changesets and rolls them back
-   ```liquibase validate``` - validates changesets