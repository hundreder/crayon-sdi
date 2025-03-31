#!/bin/bash

echo "Checking for existing crayon-postgres container..."
if [ "$(docker ps -q -f name=crayon-postgres)" ]; then
    echo "Stopping existing crayon-postgres container..."
    docker stop crayon-postgres
fi

if [ "$(docker ps -aq -f name=crayon-postgres)" ]; then
    echo "Removing existing crayon-postgres container..."
    docker rm crayon-postgres
fi

echo "Running docker image for Postgres db" 
docker run --name crayon-postgres \
    -e POSTGRES_USER=crayon \
    -e POSTGRES_PASSWORD=somepassword \
    -e POSTGRES_DB=CrayonDb \
    -p 6432:5432 \
    -d postgres:latest \


echo "Liquibase to create db schema and seed the data"
liquibase \
--changelog-file=Crayon/database/changelog.sql \
--defaultsFile=Crayon/database/liquibase.properties \
update

echo "Stopping and removing existing crayon-api container (if exists)"
# Check if a container with name crayon-api-container exists and stop it if running
if [ "$(docker ps -q -f name=crayon-api-container)" ]; then
    docker stop crayon-api-container
fi

# Remove the container if it exists
if [ "$(docker ps -aq -f name=crayon-api-container)" ]; then
    docker rm crayon-api-container
fi

echo "Removing existing crayon-api image (if exists)"
# Remove the image if it exists
if [ "$(docker images -q crayon-api:latest)" ]; then
    docker rmi crayon-api:latest
fi


echo "Createing docker image for application"
docker build -t crayon-api ./Crayon

echo "Docker run to the app"
docker run \
   -d \
   --add-host=host.docker.internal:host-gateway \
   -p 8080:8080 \
   -e ASPNETCORE_ENVIRONMENT="Development" \
   -e AppSettings__ConnectionString="Server=host.docker.internal:6432;Database=CrayonDb;User ID=crayon;Password=somepassword"  \
   --name crayon-api-container \
   crayon-api:latest

echo "Opening swagger"
sleep 5 #help container start
open "http://localhost:8080/swagger/index.html"