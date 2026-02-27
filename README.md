# Evacuation Planning and Monitoring API

Evacuation Planning and Monitoring API for managing emergency evacuation operations — zone registration, vehicle management, plan generation with best-fit capacity optimization, and real-time status monitoring.

## Demo URL for Testing

You can test the live API directly through the Swagger interface:

- **Swagger UI:** [Click here to open Swagger UI](https://evac-api-app.wonderfulwave-0a23d9cc.southeastasia.azurecontainerapps.io/swagger)
- **Base API URL:** <https://evac-api-app.wonderfulwave-0a23d9cc.southeastasia.azurecontainerapps.io>

## Tech Stack

| Layer         | Technology                      |
| ------------- | ------------------------------- |
| Runtime       | .NET 10                         |
| Framework     | ASP.NET Core Web API            |
| Database      | Redis (via StackExchange.Redis) |
| Documentation | Swagger / OpenAPI               |
| Container     | Docker & Docker Compose         |

## System Architecture

```
Controllers → Services → Repositories → Redis
     ↑            ↑
    DTOs        Models, Helpers
```

- **Controllers** — HTTP endpoints, request/response mapping
- **Services** — Business logic, plan generation algorithm
- **Repositories** — Redis data access with distributed locking
- **DTOs** — Request validation & response shaping
- **Models** — Domain entities
- **Helpers** — Haversine distance calculation

## API Endpoints

| Method   | Endpoint                  | Description                    |
| -------- | ------------------------- | ------------------------------ |
| `POST`   | `/api/evacuation-zones`   | Register a new evacuation zone |
| `POST`   | `/api/vehicles`           | Register a new vehicle         |
| `POST`   | `/api/evacuations/plan`   | Generate evacuation plan       |
| `GET`    | `/api/evacuations/status` | Get all evacuation zone status |
| `PUT`    | `/api/evacuations/update` | Update evacuation status       |
| `DELETE` | `/api/evacuations/clear`  | Clear all system data          |

## Getting Started

### Prerequisites

- .NET 10 SDK
- Redis server (or Docker)

### Run with Docker Compose

```bash
docker-compose up --build
```

API: `http://localhost:8080` | Swagger: `http://localhost:8080/swagger`

### Run Locally

```bash
# Start Redis
docker-compose up redis -d

# Run API
dotnet run
```

## Key Features

- **Urgency-based priority** — Zones with higher urgency (1-5) are processed first
- **Best-fit vehicle selection** — Prefers smallest vehicle that fits, falls back to largest available
- **Haversine distance + ETA** — Real-world distance calculation, rounded up to nearest minute
- **Real-time status** — Track all zones with evacuated count, remaining people, and last vehicle
- **Redis persistence + locking** — Data stored in Redis hashes, distributed lock prevents race conditions
- **Input validation + error handling** — Required fields, range constraints, structured 400/404/500 responses

## Example

### 1. Register Zones (POST /api/evacuation-zones) — 2 times

```json
{
  "zoneID": "Z1",
  "locationCoordinates": { "latitude": 13.7563, "longitude": 100.5018 },
  "numberOfPeople": 100,
  "urgencyLevel": 4
}
```

```json
{
  "zoneID": "Z2",
  "locationCoordinates": { "latitude": 13.7367, "longitude": 100.5231 },
  "numberOfPeople": 50,
  "urgencyLevel": 5
}
```

### 2. Register Vehicles (POST /api/vehicles) — 2 times

```json
{
  "vehicleID": "V1",
  "type": "bus",
  "capacity": 40,
  "speed": 60,
  "locationCoordinates": { "latitude": 13.765, "longitude": 100.5381 }
}
```

```json
{
  "vehicleID": "V2",
  "type": "van",
  "capacity": 20,
  "speed": 50,
  "locationCoordinates": { "latitude": 13.732, "longitude": 100.52 }
}
```

### 3. Generate Plan (POST /api/evacuations/plan)

Z2 is processed first (urgency 5 > 4):

```json
[
  {
    "zoneID": "Z2",
    "vehicleID": "V1",
    "eta": "4 minutes",
    "numberOfPeople": 40
  },
  {
    "zoneID": "Z2",
    "vehicleID": "V2",
    "eta": "1 minutes",
    "numberOfPeople": 10
  }
]
```

> Note: Plan only creates assignments. Use PUT /update to execute evacuation.

### 4. Get Status (GET /api/evacuations/status)

Returns all zones including non-evacuated:

```json
[
  {
    "zoneID": "Z1",
    "totalEvacuated": 0,
    "remainingPeople": 100,
    "lastVehicleUsed": null
  },
  {
    "zoneID": "Z2",
    "totalEvacuated": 0,
    "remainingPeople": 50,
    "lastVehicleUsed": null
  }
]
```

### 5. Update Status (PUT /api/evacuations/update)

```json
{ "zoneID": "Z2", "vehicleID": "V1", "evacuatedCount": 40 }
```

### 6. Clear Data (DELETE /api/evacuations/clear)

## Project Structure

```
EvacApiProblem/
├── Controllers/
│   ├── EvacuationsController.cs
│   ├── EvacuationZonesController.cs
│   └── VehiclesController.cs
├── DTOs/
│   ├── CreateEvacuationZoneRequest.cs
│   ├── CreateVehicleRequest.cs
│   ├── UpdateEvacuationStatusRequest.cs
│   ├── EvacuationPlanResponse.cs
│   ├── EvacuationStatusResponse.cs
│   ├── EvacuationZoneResponse.cs
│   ├── LocationDto.cs
│   └── VehicleResponse.cs
├── Helpers/
│   └── HaversineHelper.cs
├── Models/
│   ├── EvacuationPlan.cs
│   ├── EvacuationStatus.cs
│   ├── EvacuationZone.cs
│   ├── Location.cs
│   └── Vehicle.cs
├── Repositories/
│   ├── IRedisRepository.cs
│   └── RedisRepository.cs
├── Services/
│   ├── IEvacuationService.cs
│   ├── EvacuationService.cs
│   ├── IVehicleService.cs
│   └── VehicleService.cs
├── Program.cs
├── Dockerfile
├── docker-compose.yml
└── appsettings.json
```

## Configuration

Redis connection string is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

When using Docker Compose, the connection is overridden via the environment variable `ConnectionStrings__Redis=redis:6379`.
# EvacuationPlanningAndMonitoringAPI
# EvacuationPlanningAndMonitoringAPI
