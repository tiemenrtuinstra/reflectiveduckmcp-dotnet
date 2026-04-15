# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Kopieer project-bestanden en herstel NuGet packages (laag wordt gecached)
COPY src/ReflectieveDuck/ReflectieveDuck.csproj src/ReflectieveDuck/
COPY src/ReflectieveDuck.McpServer/ReflectieveDuck.McpServer.csproj src/ReflectieveDuck.McpServer/
RUN dotnet restore src/ReflectieveDuck.McpServer/ReflectieveDuck.McpServer.csproj

# Kopieer broncode en publiceer
COPY src/ src/
RUN dotnet publish src/ReflectieveDuck.McpServer/ReflectieveDuck.McpServer.csproj \
    -c Release \
    -o /app/publish

# ── Runtime stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Kopieer gepubliceerde bestanden
COPY --from=build /app/publish .

# Data-directory voor SQLite volume
RUN mkdir -p /data

# Standaard configuratie
ENV MCP_TRANSPORT=http
ENV DB_PATH=/data/local.db
ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "ReflectieveDuck.McpServer.dll"]
