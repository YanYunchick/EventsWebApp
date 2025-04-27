# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/EventsWebApp.WebApi/EventsWebApp.WebApi.csproj", "src/EventsWebApp.WebApi/"]
COPY ["src/EventsWebApp.Infrastructure/EventsWebApp.Infrastructure.csproj", "src/EventsWebApp.Infrastructure/"]
COPY ["src/EventsWebApp.Application/EventsWebApp.Application.csproj", "src/EventsWebApp.Application/"]
COPY ["src/EventsWebApp.Domain/EventsWebApp.Domain.csproj", "src/EventsWebApp.Domain/"]
RUN dotnet restore "./src/EventsWebApp.WebApi/EventsWebApp.WebApi.csproj"
COPY . .
WORKDIR "/src/src/EventsWebApp.WebApi"
RUN dotnet build "./EventsWebApp.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EventsWebApp.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventsWebApp.WebApi.dll"]