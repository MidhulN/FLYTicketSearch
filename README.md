# Flight Search API (.NET 5 + Clean Architecture)

A **.NET 5**-based flight search API built using **Clean Architecture**, with **Elasticsearch** for search indexing, **Redis** for caching, and **Keycloak** for authentication and authorization — all containerized using **Docker Compose**.

---

## Getting Started

> **Note:** The first run may take a few minutes as Docker downloads all required images depending on your internet speed.

###  Run with Docker Compose or run with visual studio

```bash
docker-compose up --build 
```
API Endpoint:
https://localhost:5001/swagger/index.html

## Authorization Token:
To obtain the authorization token, execute the Postman collection located in the src/postman collection directory to get a bearer token .

## User Registration:

To register a new user, use the following Keycloak endpoint: http://localhost:8080/

As default added one user 

 admin username : admin

 admin password : admin

 realm: FlyTicket

 username : midhul

 password : Fly
