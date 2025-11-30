# FA25_PRN232_SE1713_ASM4_SE183033_TRINM

## Overview
This solution implements an Electric Vehicle Charging Station (EVCS) management system using SOAP architecture. It consists of a SOAP API service, a Web MVC client, and a Console application client, supported by repository and service layers.

## Technology Stack

### Core Framework
- **.NET 8**: The solution is built on the latest .NET 8 platform.

### Communication Protocols
- **SOAP**: Implemented using `SoapCore` for the server and `System.ServiceModel` for clients.
- **REST/HTTP**: Used for some underlying communications.
- **SignalR**: Used for real-time web functionality (`Microsoft.AspNetCore.SignalR.Client`).

### Data Access & Storage
- **Entity Framework Core 8.0.5**: ORM for database interactions.
- **SQL Server**: Database provider (`Microsoft.EntityFrameworkCore.SqlServer`).

### Authentication & Security
- **ASP.NET Core Identity**: For user management (`Microsoft.Extensions.Identity.Core`).
- **JWT**: JSON Web Tokens for secure transmission (`System.IdentityModel.Tokens.Jwt`).
- **Google Auth**: Google API Authentication (`Google.Apis.Auth`).

### Documentation
- **Swagger/OpenAPI**: API documentation using `Swashbuckle.AspNetCore`.

## Project Structure

- **EVCS.SOAPAPIServices.TriNM**: The main SOAP service provider.
- **EVCS.SOAP.WebMVC.TriNM**: A Web MVC application acting as a client to the SOAP services.
- **EVCS.SoapClient.ConsoleApp.TriNM**: A console application client for testing and interaction.
- **EVCS.TriNM.Services**: Contains business logic and service implementations.
- **EVCS.TriNM.Repositories**: Handles data access and database operations.
