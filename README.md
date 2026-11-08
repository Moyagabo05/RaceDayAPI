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

# RaceDay - Event Management System

## System Description
RaceDay is a full-stack web-based event management system designed for the South African road running, walking, and cycling community. It allows Event Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enter events, track their personal performance history, and prepare for race day.

## User Roles
* **Organiser:** Can create, edit, and delete events, manage event categories, capture participant results, and view all event enrolments.
* **Participant:** Can create an account, browse events, enter an event by selecting a category, view their own enrolments, and track their personal results.

## Setup and Run Instructions (Local)
1. Ensure SQL Server is running and update the connection string in `RaceDayAPI/appsettings.json`.
2. Open the solution in Visual Studio.
3. Set both `RaceDayAPI` and `RaceDayWeb` as startup projects.
4. Press F5 to run.

## Docker Setup
The MVC application is containerised. To run it using Docker:
1. Ensure Docker Desktop is running and the API is running locally on port 7200.
2. Build the image: `docker build -t raceday-web .`
3. Run the container: `docker run -d -p 8080:8080 -e ApiSettings__BaseUrl=http://host.docker.internal:7200 --name raceday-web raceday-web`
4. Access the app at `http://localhost:8080`

## CI/CD Pipeline
The project uses GitHub Actions to automatically build and test the code on every push.
![CI/CD Green Build](docs/ci-cd-screenshot.png) <!-- Make sure to add your screenshot to the docs folder! -->

## Video Presentation
[Click here to watch the unlisted YouTube video presentation](INSERT_YOUR_YOUTUBE_LINK_HERE)

## AI Disclosure
AI tools (GitHub Copilot) were used to assist with boilerplate code generation and debugging. All architectural decisions, logic implementation, and security fixes (such as rotating compromised Azure keys) were performed manually.
