# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy .NET project files
COPY ./*.sln ./
COPY src/ ./src/
COPY tests/ ./tests/

# Restore project with layers
RUN dotnet restore

# Copy the entire solution and build
COPY . .
RUN dotnet publish -c release -o published --no-cache

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/published ./

ENV ASPNETCORE_URLS=http://+:5005
ENTRYPOINT ["dotnet", "Api.dll"]
