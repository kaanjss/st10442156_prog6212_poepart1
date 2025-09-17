# CMCS (Claim Management and Compensation System)

A simple ASP.NET Core 8 MVC web application to manage claim submissions, reviews, and approvals for a university context.

## Project Structure
- CMCS.Web: ASP.NET Core MVC application
  - Controllers: MVC controllers for Home, Lecturer, and Admin.
  - Models: Domain models like User, Claim, and supporting enums.
  - Views: Razor views for lecturers (submit, dashboard), admins (review, manager, coordinator), and shared layout.
  - wwwroot: Static files including Bootstrap, jQuery and custom site assets.

## Prerequisites
- .NET SDK 8.0
- Windows, macOS, or Linux

## Getting Started
`ash
# Restore packages
dotnet restore

# Build
dotnet build

# Run (from CMCS.Web)
dotnet run --project CMCS.Web
`
The app will start on https://localhost:5001 or http://localhost:5000 depending on your environment.

## Configuration
- App settings are in CMCS.Web/appsettings.json and environment-specific overrides in ppsettings.Development.json.
- Change ports or other settings via Properties/launchSettings.json.

## License
This project will include an MIT License in a subsequent commit.

## Continuous Integration
A GitHub Actions workflow will be added to build and validate the solution on push/PR.
