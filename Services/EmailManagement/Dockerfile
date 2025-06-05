FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /App

# Copy everything
COPY Common/SharedUtilities/SharedEvents/ ./Common/SharedUtilities/SharedEvents/
COPY Common/SharedUtilities/SharedUtilities/ ./Common/SharedUtilities/SharedUtilities/
COPY Services/EmailManagement/src/ ./Services/EmailManagement/src/

# Restore as distinct layers
RUN dotnet restore Services/EmailManagement/src/App/App.csproj

# Build and publish a release
RUN dotnet publish -c Release -o out Services/EmailManagement/src/App/App.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "App.dll"]