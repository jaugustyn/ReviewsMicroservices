</br>
<h1 align="center">Microservices Reviews App</h2>
</br>
  
This repository contains a model of a microservices-based review application built using C# and Docker Compose. The purpose of this model is to showcase the architectural design and components of a typical microservices application.

</br>

## Table of Contents

- [Overview](#overview)
- [Services](#services)
- [Technologies Used](#technologies-used)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Running the Application](#running-the-application)
- [Endpoints](#endpoints)
- [License](#license)

</br>

## Overview

The microservices review app model demonstrates the separation of concerns and modular architecture commonly found in microservices-based applications. It consists of multiple services, each responsible for a specific functionality, which work together to provide applications for creating reviews.

</br>

## Services

The microservices blog app model consists of the following services:

1. **Authentication Service**: Responsible for user authentication and authorization.
2. **Reviews Service**: Handles the creation, retrieval, and management of reviews and their ratings.
3. **Comment Service**: Manages the comments associated with reviews.

</br>

## Technologies Used

The application is built using the following technologies:

- Docker: Containerization platform for packaging the application components.
- MongoDB: A NoSQL database used to store reviews, ratings, comments, and user information.
- .NET 6: A cross-platform framework for building microservices and APIs.
- Nginx: A web server used as a reverse proxy to handle routing and load balancing between microservices.
- RabbitMQ: A message broker used for asynchronous communication between microservices.

</br>

## Prerequisites

To run this model locally, you need to have the following software installed on your system:

- [.NET Core SDK](https://dotnet.microsoft.com/download) (version 6.0 or higher)
- [Docker](https://www.docker.com/get-started)
- [MongoDB](https://www.mongodb.com/)

</br>

## Installation

To get started with the blog microservices app, follow these steps:

1. Install Docker on your machine if you haven't already.
2. Clone this repository to your local machine.
3. Navigate to the project directory.

</br>

## Running the Application

To run the application, execute the following commands in the project directory:

1. Build the Docker images for the microservices:

```sh
docker-compose build
```

2. Start the Docker containers:

```sh
docker-compose up
```
3. Once the services are up and running, you can interact with the blog application using the provided endpoints.

Each service is implemented as a separate microservice and can communicate with each other through well-defined APIs.

</br>

## Endpoints

- **Test Authentication Service**:
  - URL Path: https://localhost:5005/swagger/index.html

- **Test Review Service**:
  - URL Path: https://localhost:5003/swagger/index.html

- **Test Comment Service**:
  - URL Path: https://localhost:5001/swagger/index.html

</br>

## License

This project is licensed under the [MIT License](LICENSE). You are free to modify and use the codebase as per the terms of this license.
