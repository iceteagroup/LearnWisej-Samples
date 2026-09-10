# EnterpriseOps — sample container image.
#
# ⚠ THIS FILE IS A LAB DELIVERABLE, NOT A BUILD STEP.
#   Nothing in this module builds or runs a container. Read it, review it, then build it on your own
#   infrastructure when you have somewhere to push to. `docker build` is deliberately not part of the lab.
#
# Build context = the module folder (the parent of this one):
#   docker build -f deployment/Dockerfile -t enterpriseops:2.4.2 .

# ---- build -------------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first, on the project file alone, so a code change does not invalidate the package layer.
COPY EnterpriseOps/EnterpriseOps.csproj EnterpriseOps/
RUN dotnet restore EnterpriseOps/EnterpriseOps.csproj -f net10.0

COPY . .
# Only the cross-platform target framework goes into a Linux image.
RUN dotnet publish EnterpriseOps/EnterpriseOps.csproj -f net10.0 -c Release -o /app --no-restore

# ---- runtime -----------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# The image is environment-agnostic. Everything that differs between environments is supplied at
# `docker run` time; nothing below is a secret.
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    EnterpriseOps__Release__NodeName=app-node-unset

# Wisej.NET serves the project folder as the web root; the published output is that folder here.
ENV WEBSITE_PATH=/app

EXPOSE 8080

# Liveness for the orchestrator — never readiness. Readiness (/healthz) belongs to the load balancer,
# because "not ready" must take the node out of rotation, not restart it.
HEALTHCHECK --interval=10s --timeout=3s --start-period=20s --retries=3 \
    CMD wget -q -O - http://localhost:8080/healthz/live || exit 1

# Do not run as root.
USER $APP_UID

ENTRYPOINT ["dotnet", "EnterpriseOps.dll"]

# Secrets are NEVER baked in. At deploy time the platform supplies:
#   EnterpriseOps__ConnectionString
#   EnterpriseOps__IdentityProvider__ClientSecret
# StartupValidation refuses to boot without them, so a misconfigured image fails immediately and
# visibly instead of serving broken pages.
