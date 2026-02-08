# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/KernelMind.Api/KernelMind.Api.csproj
RUN dotnet publish src/KernelMind.Api/KernelMind.Api.csproj -c Release -o /app/publish

# Production stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS production
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "KernelMind.Api.dll"]
