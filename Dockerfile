# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Development

# Copy solution and projects
COPY GameOfLifeSolution.sln .
COPY src/GameOfLifeAPI/*.csproj ./src/GameOfLifeAPI/
COPY src/GameOfLifeAPI.Tests/*.csproj ./src/GameOfLifeAPI.Tests/

# Copy the rest
COPY . .

# Restore dependencies with all files present
RUN dotnet restore GameOfLifeSolution.sln

# Build the solution
RUN dotnet build GameOfLifeSolution.sln --no-restore

# Run tests
RUN dotnet test src/GameOfLifeAPI.Tests/GameOfLifeAPI.Tests.csproj --no-restore --verbosity normal

# Publish app
WORKDIR /app/src/GameOfLifeAPI
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GameOfLifeAPI.dll"]
