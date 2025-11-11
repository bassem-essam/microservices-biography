FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source/aspnetapp

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# copy everything else and build app
COPY . .

# migrate
# RUN dotnet tool install --global dotnet-ef
ENV ASPNETCORE_ENVIRONMENT=docker
# RUN rm -rf ./Migrations
# RUN /root/.dotnet/tools/dotnet-ef migrations add AdditionalMigration
# RUN /root/.dotnet/tools/dotnet-ef database update

RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=docker

COPY --from=build /app ./

EXPOSE 5113

ENTRYPOINT ["dotnet", "UserService.dll"]
