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

# Install system dependencies
RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

# Set environment
ENV ASPNETCORE_ENVIRONMENT=docker

# Copy published app
COPY --from=build /app ./

# Create necessary directories and test file
RUN mkdir -p public/avatars
RUN echo 'TEST CONTENT' > public/avatars/test.txt

EXPOSE 5111

ENTRYPOINT ["dotnet", "AvatarService.dll"]
