#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Api/GS_Shop_UserManagement.Api/GS_Shop_UserManagement.Api.csproj", "Api/GS_Shop_UserManagement.Api/"]
COPY ["Application/GS_Shop_UserManagement.Application/GS_Shop_UserManagement.Application.csproj", "Application/GS_Shop_UserManagement.Application/"]
COPY ["Core/GS_Shop_UserManagement.Domain/GS_Shop_UserManagement.Domain.csproj", "Core/GS_Shop_UserManagement.Domain/"]
COPY ["Infrastructure/Infrastructure/GS_Shop_UserManagement.Infrastructure/GS_Shop_UserManagement.Infrastructure.csproj", "Infrastructure/Infrastructure/GS_Shop_UserManagement.Infrastructure/"]
COPY ["Infrastructure/Persistence/GS_Shop_UserManagement.Persistence/GS_Shop_UserManagement.Persistence.csproj", "Infrastructure/Persistence/GS_Shop_UserManagement.Persistence/"]
RUN dotnet restore "./Api/GS_Shop_UserManagement.Api/GS_Shop_UserManagement.Api.csproj"
COPY . .
WORKDIR "/src/Api/GS_Shop_UserManagement.Api"
RUN dotnet build "./GS_Shop_UserManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GS_Shop_UserManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GS_Shop_UserManagement.Api.dll"]