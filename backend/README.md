# Online Doctor Consultation & Chat System - Backend

## Project Structure

```
backend/
├── ConsultationApi/                 # ASP.NET Core 8 Web API + SignalR Hub
├── ConsultationApi.Application/     # Business logic, services, DTOs
├── ConsultationApi.Domain/          # Core entities, interfaces
├── ConsultationApi.Infrastructure/  # EF Core, Redis, RabbitMQ implementations
├── ConsultationWorker/              # RabbitMQ consumer (.NET Worker Service)
├── ConsultationApi.Tests/           # Unit tests
├── docker-compose.yml               # Local Docker services
└── README.md                         # This file
```

## Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| API Framework | ASP.NET Core | 8.0 |
| ORM | Entity Framework Core | 8.0.4 |
| Database | PostgreSQL | 15+ |
| Cache | Redis | 7+ |
| Messaging | RabbitMQ | 3.11+ |
| Authentication | JWT Bearer | Built-in |
| Real-time | ASP.NET Core SignalR | 8.0 |
| HTTP Client | HttpClientFactory | Built-in |

## Prerequisites

- **.NET 8 SDK** installed
- **Docker Desktop** (for PostgreSQL, Redis, RabbitMQ)
- **PostgreSQL client** (optional, for manual DB management)
- **Port availability**: 5000 (API), 5432 (PostgreSQL), 6379 (Redis), 5672 (RabbitMQ)

## Quick Start (5 minutes)

### 1. Start Infrastructure Services (Docker)

```bash
# From the backend directory
cd backend

# Start PostgreSQL, Redis, and RabbitMQ
docker-compose up -d
```

Verify services are running:
```bash
docker-compose ps
# Should show: postgres, redis, rabbitmq (all running)
```

**Docker Services Ports:**
- PostgreSQL: `localhost:5432` (user: `postgres`, password: `postgres`)
- Redis: `localhost:6379`
- RabbitMQ Management UI: `http://localhost:15672` (user: `guest`, password: `guest`)

### 2. Build and Run API

```bash
# Terminal 1 - API
cd backend
dotnet build
dotnet run --project ConsultationApi
```

The API will start at: `http://localhost:5266`

API Health Check:
```bash
curl http://localhost:5266/health
```

### 3. Build and Run Worker Service

```bash
# Terminal 2 - Worker Service
cd backend
dotnet run --project ConsultationWorker
```

Expected output:
```
info: ConsultationWorker.RabbitMQ.AppointmentEventConsumer[0]
      AppointmentEventConsumer starting
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### 4. Apply Database Migrations (one-time)

```bash
# From backend directory
cd backend
dotnet ef database update --project ConsultationApi.Infrastructure --startup-project ConsultationApi
```

This creates all tables and applies seed data.

## Configuration

### appsettings.json (Connections & Services)

**File:** `ConsultationApi/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=consultationdb;Username=postgres;Password=postgres"
  },
  "RabbitMq": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "ExchangeName": "appointment.exchange"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  },
  "Jwt": {
    "SecretKey": "your-super-secret-jwt-key-at-least-32-characters-long",
    "Issuer": "https://doctor-consultation.local",
    "Audience": "https://doctor-consultation.local",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

**Worker Settings:** `ConsultationWorker/appsettings.json`

Same RabbitMQ and database settings as API, so worker can connect and consume messages.

## Database Schema

### Tables Implemented

1. **users** - Patient, Doctor, Admin accounts
2. **doctor_profiles** - Doctor specialization, bio, consultation fee
3. **availability_slots** - Doctor bookable time slots
4. **appointments** - Patient-Doctor appointment bookings
5. **consultation_sessions** - Active consultation records
6. **chat_messages** - Messages during consultation
7. **notifications** - Notification queue (populated by RabbitMQ consumer)
8. **reviews** - Patient ratings post-consultation
9. **refresh_tokens** - JWT refresh token storage

### Key Indexes
- `users.email` (UNIQUE)
- `appointments.patient_id`, `doctor_id`, `status`
- `chat_messages.session_id`
- `notifications.user_id`, `is_read`

## API Endpoints

### Auth Endpoints
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Get JWT tokens
- `POST /api/auth/refresh` - Refresh expired token
- `GET /api/auth/me` - Get current user profile

### Doctor Endpoints
- `GET /api/doctors` - List all doctors (Redis cached)
- `GET /api/doctors/{id}` - Doctor profile & slots (Redis cached)
- `PUT /api/doctors/profile` - Update profile (invalidates cache)
- `POST /api/doctors/slots` - Add availability slots
- `PATCH /api/doctors/availability` - Toggle availability

### Appointment Endpoints
- `POST /api/appointments` - Book appointment (publishes RabbitMQ event)
- `GET /api/appointments` - List user appointments
- `PATCH /api/appointments/{id}/confirm` - Doctor confirms
- `PATCH /api/appointments/{id}/cancel` - Cancel appointment
- `POST /api/appointments/{id}/session/start` - Start consultation
- `POST /api/appointments/{id}/session/end` - End consultation (publishes event)

### Chat Endpoints
- `GET /api/sessions/{sessionId}/messages` - Chat history
- `POST /api/sessions/{sessionId}/messages` - Send message (via REST)
- SignalR Hub: `/hubs/consultation` (WebSocket for real-time chat)

### Notification Endpoints
- `GET /api/notifications` - Get user notifications
- `PATCH /api/notifications/{id}/read` - Mark as read
- `PATCH /api/notifications/read-all` - Mark all as read

## SignalR Real-Time Chat

### Hub Connection
```
URL: ws://localhost:5266/hubs/consultation
Auth: JWT Bearer token required
```

### Hub Methods (Client → Server)
```csharp
// Join a consultation session
await hubConnection.InvokeAsync("JoinSession", sessionId);

// Send message
await hubConnection.InvokeAsync("SendMessage", sessionId, messageText);

// Mark messages as read
await hubConnection.InvokeAsync("MarkRead", sessionId);

// Leave session
await hubConnection.InvokeAsync("LeaveSession", sessionId);
```

### Hub Events (Server → Client)
```csharp
// Receive new message
hubConnection.On<string, string, string, DateTime>("ReceiveMessage", 
    (senderId, senderName, text, sentAt) => { /* handle */ });

// User joined session
hubConnection.On<string, string>("UserJoined", 
    (userId, name) => { /* handle */ });

// Session ended
hubConnection.On<string, DateTime>("SessionEnded", 
    (sessionId, endedAt) => { /* handle */ });
```

## RabbitMQ Messaging

### Events Published by API

| Event | Exchange | Routing Key | Trigger |
|-------|----------|-------------|---------|
| AppointmentBooked | appointment.exchange | appointment.booked | POST /appointments |
| AppointmentConfirmed | appointment.exchange | appointment.confirmed | PATCH confirm |
| AppointmentCancelled | appointment.exchange | appointment.cancelled | PATCH cancel |
| ConsultationCompleted | appointment.exchange | consultation.completed | POST session/end |

### Event Payload Example
```json
{
  "eventId": "uuid",
  "appointmentId": "uuid",
  "patientId": "uuid",
  "patientName": "John Doe",
  "doctorId": "uuid",
  "doctorName": "Dr. Smith",
  "slotDate": "2026-06-10",
  "slotTime": "10:00",
  "status": "Pending",
  "occurredAt": "2026-06-02T10:00:00Z"
}
```

### Worker Service Consumer

The **ConsultationWorker** subscribes to all events and:

- **AppointmentBooked:** Creates notification for doctor
- **AppointmentConfirmed:** Creates notification for patient
- **AppointmentCancelled:** Creates notifications for both parties
- **ConsultationCompleted:** Creates notification for patient to leave review

### Dead Letter Queue (DLQ)

If message processing fails:
1. Message is retried 3 times with exponential backoff
2. On final failure, message sent to DLQ: `appointment.dlq`
3. Admins can inspect DLQ for failed messages

**Monitor DLQ:**
```bash
# Via RabbitMQ Management UI
http://localhost:15672 → Queues → appointment.dlq
```

## Redis Caching

### Cached Data

| Key Pattern | TTL | Invalidated When |
|-------------|-----|------------------|
| `doctors:list:{filter_hash}` | 5 min | Profile or slot changes |
| `doctors:{id}:profile` | 10 min | Doctor updates profile |
| `doctors:{id}:slots:{date}` | 2 min | Slot is booked/removed |
| `session:active:{sessionId}` | 30 min (sliding) | Session ends |
| `user:token:{userId}` | 15 min (sliding) | User logs out |

### Cache Operations

```csharp
// Service: ICacheService (injected)
var doctors = await _cacheService.GetAsync<List<DoctorDto>>("doctors:list:all");
await _cacheService.SetAsync("key", value, TimeSpan.FromMinutes(5));
await _cacheService.RemoveAsync("key");
await _cacheService.RemoveByPatternAsync("doctors:*");
```

## Authentication & Authorization

### JWT Tokens

**Access Token:** 60 minutes expiration
**Refresh Token:** 7 days expiration

### User Roles

1. **Patient** - Can book appointments, chat, leave reviews
2. **Doctor** - Can manage slots, confirm/start/end consultations
3. **Admin** - Can manage users, view reports (future enhancement)

### Securing Endpoints

```csharp
[HttpPost("appointments")]
[Authorize(Roles = "Patient")]  // Patient only
public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDto dto)
{
    // Logic here
}
```

## Seed Data (Default Test Accounts)

After migrations, the following test accounts are created:

### Patient Account
- **Email:** `patient@test.com`
- **Password:** `Patient123!`

### Doctor Accounts
```
1. Email: doctor.cardiology@test.com
   Specialization: Cardiology
   Fee: $50

2. Email: doctor.neurology@test.com
   Specialization: Neurology
   Fee: $75

3. Email: doctor.dermatology@test.com
   Specialization: Dermatology
   Fee: $40
```

All doctor passwords: `Doctor123!`

### Admin Account (Future)
- **Email:** `admin@test.com`
- **Password:** `Admin123!`

## Project Architecture

### Layered Architecture

```
┌─ Presentation Layer (ConsultationApi)
│  ├─ Controllers (HTTP endpoints)
│  ├─ SignalR Hub (Real-time chat)
│  └─ Middleware (JWT validation, exception handling)
│
├─ Application Layer (ConsultationApi.Application)
│  ├─ Services (business logic)
│  ├─ DTOs (data transfer objects)
│  └─ Validators (FluentValidation)
│
├─ Domain Layer (ConsultationApi.Domain)
│  ├─ Entities (core models)
│  ├─ Enums (status types)
│  └─ Interfaces (repository contracts)
│
└─ Infrastructure Layer (ConsultationApi.Infrastructure)
   ├─ EF Core DbContext (data access)
   ├─ Redis CacheService
   ├─ RabbitMQ Publishers & Consumer
   └─ Repository Implementations
```

### Dependency Injection

```csharp
// Program.cs
builder.Services
    .AddInfrastructure(configuration)      // EF Core, Redis, RabbitMQ
    .AddApplication(configuration)          // Services, validators
    .AddAuthentication(configuration)       // JWT
    .AddSignalR();                           // Real-time hub
```

## Error Handling

### Global Exception Middleware

All unhandled exceptions are caught and returned in a consistent format:

```json
{
  "type": "about:blank",
  "title": "Error message",
  "status": 500,
  "detail": "Full exception details",
  "traceId": "request-id"
}
```

### Common HTTP Status Codes

- `400` - Bad Request (validation failed)
- `401` - Unauthorized (missing/invalid JWT)
- `403` - Forbidden (insufficient permissions)
- `404` - Not Found (resource doesn't exist)
- `500` - Internal Server Error (unhandled exception)

## Logging

### Log Levels

- **Information:** Application events, API calls
- **Warning:** Retry failures, cache misses
- **Error:** Exceptions, failed operations

### Log Output

Logs are written to:
- Console (development)
- Application Insights (if configured)

### Important Log Prefixes

- `[RabbitMqPublisher]` - Message publishing logs
- `[AppointmentEventConsumer]` - Worker consumer logs
- `[CacheService]` - Redis cache operations

## Testing

### Run Unit Tests

```bash
dotnet test ConsultationApi.Tests
```

### Test Coverage

- Appointment service booking logic
- User authentication
- Doctor availability validation
- Notification creation

## Troubleshooting

### Issue: "RabbitMQ.Client' does not contain a definition for 'CreateConnection'"

**Cause:** RabbitMQ.Client v7.0+ only exposes async API (`CreateConnectionAsync`)

**Solution:** ✅ Already fixed in `RabbitMqPublisher.cs` using reflection with async fallback

### Issue: Redis connection refused

**Solution:**
```bash
docker-compose ps    # Check if redis is running
docker-compose logs redis  # View Redis logs
```

### Issue: Database migration fails

**Solution:**
```bash
# Delete database and recreate
dotnet ef database drop --project ConsultationApi.Infrastructure --startup-project ConsultationApi
dotnet ef database update --project ConsultationApi.Infrastructure --startup-project ConsultationApi
```

### Issue: Worker not consuming messages

**Check:**
1. RabbitMQ is running: `docker-compose ps`
2. Worker is running and shows "AppointmentEventConsumer starting"
3. Check RabbitMQ Management UI for queues and messages

## Performance Considerations

### Caching Strategy

- Doctor profiles cached for 10 minutes
- Doctor slots cached for 2 minutes (rapid changes)
- Active sessions cached for 30 minutes (sliding window)
- Cache invalidated on writes (cache-aside pattern)

### Database Indexes

- All foreign keys indexed for fast joins
- Email indexed for user lookups
- Appointment status indexed for filtering

### Connection Pooling

- EF Core connection pooling enabled
- Redis connection pooled via StackExchange.Redis
- RabbitMQ connections created per-publish (short-lived)

## Deployment Notes

### Environment Variables (Production)

```bash
export ConnectionStrings__DefaultConnection="..."
export RabbitMq__HostName="rabbitmq.prod.com"
export Redis__Configuration="redis.prod.com:6379"
export Jwt__SecretKey="your-prod-secret-key"
export ASPNETCORE_ENVIRONMENT="Production"
```

### Scaling Considerations

1. **API Instances:** Can run multiple instances behind load balancer
2. **Worker Instances:** Multiple workers can consume from same RabbitMQ queue (auto-scale by load)
3. **Redis:** Use Redis Cluster or Redis Sentinel for HA
4. **Database:** Consider read replicas for reporting

## Support & Debugging

### Enable Detailed Logging

In `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "RabbitMQ.Client": "Debug",
      "StackExchange.Redis": "Debug"
    }
  }
}
```

### RabbitMQ Management UI

Access at: `http://localhost:15672`
- Default credentials: `guest` / `guest`
- Monitor exchanges, queues, messages in real-time

### Database Query Profiling

Enable EF Core query logging:
```csharp
optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
```

---

**Last Updated:** June 2026  
**Backend Requirements:** ✅ All implemented  
**Status:** Production-ready (with Docker infrastructure)

dotnet add ConsultationApi.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.4

dotnet add ConsultationApi.Infrastructure package BCrypt.Net-Next --version 4.0.3

dotnet add ConsultationApi.Infrastructure package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.4

dotnet add ConsultationApi.Infrastructure package Microsoft.Extensions.DependencyInjection.Abstractions --version 8.0.0

dotnet add ConsultationApi.Infrastructure package StackExchange.Redis --version 2.8.0

dotnet add package FluentValidation.AspNetCore --version 11.3.0

dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1
# Migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

## RabbitMQ publisher

The solution includes a typed RabbitMQ publisher for the API and a worker consumer. To enable end-to-end messaging:

- Ensure RabbitMQ is running (Docker Compose or local). Configure `RabbitMqSettings` in the API `appsettings.json`.
- The API publishes to the `appointment.exchange` (direct). The worker consumes from the queues and uses a DLX named `appointment.dlx`.

## Applying migrations and seed

This repository provides both an EF Core migration scaffold and SQL seed files.

1. To apply EF Core migrations (recommended):

```powershell
cd backend
dotnet ef database update --project ConsultationApi.Infrastructure --startup-project ConsultationApi
```

2. Alternatively, apply the canonical schema and seed directly using psql (if you prefer the SQL scripts):

```powershell
# from repository root
psql -h <db-host> -U <db-user> -d <db-name> -f db/schema.sql
psql -h <db-host> -U <db-user> -d <db-name> -f db/seed.sql
```

## Running tests

There is a basic xUnit test project `ConsultationApi.Tests`. Run it with:

```powershell
cd backend
dotnet test
```
