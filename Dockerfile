# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and projects
COPY GameOfLifeSolution.sln .
COPY src/GameOfLifeAPI/*.csproj ./src/GameOfLifeAPI/
COPY src/GameOfLifeAPI.Tests/*.csproj ./src/GameOfLifeAPI.Tests/

# Restore dependencies
RUN dotnet restore GameOfLifeSolution.sln

# Copy the rest
COPY . .

# Build and publish
WORKDIR /app/src/GameOfLifeAPI
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GameOfLifeAPI.dll"]
