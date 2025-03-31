# crayon-sdi
System design interview project for Crayon Senior .Net developer role

# Requirements
Requirements are located in /requirements folder

`Crayon System Design Interview Question 1.pdf`

`Crayon Technical Exercise Interview Question 1.pdf`

# Crayon system design interview Question 1 - Solution
Solution of this task can be found in system-design-task folder. Filder has 3 files with same content but of different type. For most efficient viewof solution go to https://excalidraw.com/ ,click Open in the left side burger menu and choose `Crayon - system design task.excalidraw` file.

# Crayon Technical Exercise Interview Question 1 - Solution
Code is located in /coding-exercise folder.


# Prerequisites
 - .net 9 sdk installed
 - IDE would help browsing the code / debugging
 - Docker (Containter management tool like Rancher Desktop would help too)
 
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
```
liquibase update
```

# Starting the application

## Using dotnet run
From the root of repository run:
```
dotnet run --project  coding-exercise/Crayon/Crayon.API
```


## Using docker

Build an image. Go to coding-exercise/Crayon and run 
```
docker build -t crayon-api .
```


Start the image
```
docker run -d -p 8080:8080 -e "ASPNETCORE_ENVIRONMENT=Development"  --name crayon-api-container crayon-api:latest
```


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


Usefull liquibase commands
-   ```liquibase updateTestingRollback``` - run all changesets and rolls them back
-   ```liquibase validate``` - validates changesets