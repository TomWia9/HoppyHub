FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy .NET project files
COPY ./*.sln ./
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done
COPY tests/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p tests/${file%.*}/ && mv $file tests/${file%.*}/; done
# Restore project with layers
RUN dotnet restore

COPY . ./
RUN dotnet publish -c release -o published --no-cache

# Run
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build app/published ./
ENV ASPNETCORE_URLS=http://+:5005
ENTRYPOINT ["dotnet", "Api.dll"]