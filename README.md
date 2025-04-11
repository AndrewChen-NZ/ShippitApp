# ShippitApp

## Installation

Requirements: .NET SDK 9.0
dotnet --version  # Check the installed version of .NET SDK

Restore dependencies:
dotnet restore

Compile the project:
dotnet build

Docker:
docker build -t shippitapp .

## Usage

POST to /route - adds a line haul route.
Parameters:
{
	"from": "//Origin",
	"to"  :	"//Destination",
	"travel_Time_Seconds": 12345
}

GET from /route?from=locationA&to=locationB - returns the route with the shortest time distance.
Return format:
{
	"path": ["LocationA", "LocationB"],
	"travel_Time_Total_Seconds": 12345
}
