# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /src

# Copy solution and project files
COPY . .

# Restore and build
RUN dotnet restore DoctorConsultationSystem.sln
RUN dotnet build DoctorConsultationSystem.sln -c Release --no-restore

# Publish API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS api-stage
WORKDIR /src
COPY --from=builder /src .
RUN dotnet publish ConsultationApi/ConsultationApi.csproj -c Release -o /app/api --no-build

# Publish Worker
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS worker-stage
WORKDIR /src
COPY --from=builder /src .
RUN dotnet publish ConsultationWorker/ConsultationWorker.csproj -c Release -o /app/worker --no-build

# Runtime API stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=api-stage /app/api .
ENTRYPOINT ["dotnet", "ConsultationApi.dll"]
