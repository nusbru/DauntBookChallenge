# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and all project files for restore (leverage Docker layer caching)
COPY ["src/Daunt.App/Daunt.App.csproj", "src/Daunt.App/"]
COPY ["src/Daunt.Core/Daunt.Core.csproj", "src/Daunt.Core/"]
COPY ["src/Daunt.Contract/Daunt.Contract.csproj", "src/Daunt.Contract/"]
COPY ["src/Daunt.Persistence/Daunt.Persistence.csproj", "src/Daunt.Persistence/"]
COPY ["src/Daunt.Shared/Daunt.Shared.csproj", "src/Daunt.Shared/"]

# Restore dependencies (cached if project files haven't changed)
RUN dotnet restore "src/Daunt.App/Daunt.App.csproj"

# Copy all source files
COPY src/ src/

# Build and publish in one step (no need for separate build stage)
WORKDIR /src/src/Daunt.App
RUN dotnet publish "Daunt.App.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

# Install dependencies for globalization
RUN apk add --no-cache icu-libs

# Copy published app from build stage
COPY --from=build --chown=app:app /app/publish .

# Switch to non-root user
USER app

# Expose port (only HTTP - HTTPS should be handled by reverse proxy)
EXPOSE 8080

# Health check (optional - adjust endpoint if needed)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

# Set environment variables for production
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "Daunt.App.dll"]
