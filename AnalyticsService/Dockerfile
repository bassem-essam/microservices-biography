FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy csproj and restore as distinct layers for better caching
COPY *.csproj .
RUN dotnet restore

# Copy everything else and build app
COPY . .
RUN dotnet publish -c Release -o /app --no-restore

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Set environment
ENV ASPNETCORE_ENVIRONMENT=docker

# Copy published app
COPY --from=build /app ./

EXPOSE 5114

ENTRYPOINT ["dotnet", "AnalyticsService.dll"]
