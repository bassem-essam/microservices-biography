FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source/aspnetapp

# copy csproj and restore as distinct layers
# COPY *.sln .
COPY *.csproj .
RUN dotnet restore

# copy everything else and build app
COPY . .
# WORKDIR /source/aspnetapp
RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=docker

COPY --from=build /app ./

EXPOSE 5114

ENTRYPOINT ["dotnet", "AnalyticsService.dll"]
