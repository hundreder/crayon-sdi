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
