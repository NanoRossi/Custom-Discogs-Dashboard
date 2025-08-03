# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY ./DiscogsProxy/DiscogsProxy.csproj ./DiscogsProxy/
RUN dotnet restore ./DiscogsProxy/DiscogsProxy.csproj

# Copy everything else and build
COPY ./ ./DiscogsProxy/
WORKDIR /app/DiscogsProxy
RUN dotnet publish ./DiscogsProxy/DiscogsProxy.csproj -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/DiscogsProxy/out ./
ENTRYPOINT ["dotnet", "DiscogsProxy.dll"]
