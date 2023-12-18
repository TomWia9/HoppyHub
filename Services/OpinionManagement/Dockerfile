FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY Common/SharedUtilities/SharedEvents/ ./Common/SharedUtilities/SharedEvents/
COPY Common/SharedUtilities/SharedUtilities/ ./Common/SharedUtilities/SharedUtilities/
COPY Services/OpinionManagement/src/ ./Services/OpinionManagement/src/

# Restore as distinct layers
RUN dotnet restore Services/OpinionManagement/src/Api/Api.csproj

# Build and publish a release
RUN dotnet publish -c Release -o out Services/OpinionManagement/src/Api/Api.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "Api.dll"]