# OrderDesk.Web · Linux container (Module 7).
#
# Build context = the OrderDesk.Web project folder (docker-compose.yml sets it; by hand:
#   cd OrderDesk.Web && docker build -f deploy/docker/Dockerfile -t orderdesk-web:7.0.0-capstone .
# Multi-stage: the SDK image publishes the net10.0 (Kestrel, no Windows) target; the runtime image only
# carries the published output. Linux is case-sensitive and the container is ephemeral — the storage root
# App_Data is a volume, the health check is the /health endpoint from Startup.cs.

# ── build ──────────────────────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish OrderDesk.Web.csproj -c Release -f net10.0 -o /app/publish /p:UseAppHost=false

# ── runtime ────────────────────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
# curl for the HEALTHCHECK below (the aspnet image ships without it).
RUN apt-get update \
 && apt-get install -y --no-install-recommends curl \
 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .

# Kestrel listens on 8080 inside the container (compose maps 5607:8080). HTTPS terminates at the reverse proxy.
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_PRINT_TELEMETRY_MESSAGE=false
EXPOSE 8080

# The storage root (Web.config OrderDesk.StorageRoot = App_Data, relative to the content root /app):
# uploads, exports, reports and logs live here — mount it, or the next deployment loses them.
RUN mkdir -p /app/App_Data
VOLUME ["/app/App_Data"]

# What the orchestrator polls: the plain ASP.NET Core endpoint, no Wisej session created.
HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
  CMD curl -fsS http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "OrderDesk.dll"]
