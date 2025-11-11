FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source/aspnetapp

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=docker

COPY --from=build /app ./

EXPOSE 5115

ENTRYPOINT ["dotnet", "WebApiGateway.dll"]
