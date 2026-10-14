#RaceDayAPI

[![.NET CI/CD Pipeline](https://github.com/Moyagabo05/RaceDayAPI/actions/workflows/main.yml/badge.svg)](https://github.com/Moyagabo05/RaceDayAPI/actions/workflows/main.yml)

A robust, RESTful API built with ASP.NET Core for managing race day events, registrations, and organizers. This project demonstrates clean architecture, comprehensive unit testing, and automated CI/CD pipelines.

##  Features

- **CRUD Operations**: Full Create, Read, Update, and Delete endpoints for Event management.
- **Data Validation**: DTOs (Data Transfer Objects) used to ensure clean and secure data binding.
- **Automated Testing**: Comprehensive xUnit test suite covering success and edge-case scenarios.
- **CI/CD Pipeline**: Automated build and test workflow using GitHub Actions on every push/pull request.
- **In-Memory Database**: Isolated, fast-running tests using EF Core In-Memory provider.

##  Tech Stack

- **Framework**: .NET 8.0 (ASP.NET Core Web API)
- **Database**: Entity Framework Core (SQL Server / In-Memory for testing)
- **Testing**: xUnit
- **CI/CD**: GitHub Actions
- **IDE**: Visual Studio 2022

##  Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) 
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/Moyagabo05/RaceDayAPI.git
   cd RaceDayAPI
