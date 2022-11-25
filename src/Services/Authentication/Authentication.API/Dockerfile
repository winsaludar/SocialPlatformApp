#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Authentication/Authentication.API/Authentication.API.csproj", "Services/Authentication/Authentication.API/"]
COPY ["EventBus/EventBus.Core/EventBus.Core.csproj", "EventBus/EventBus.Core/"]
COPY ["EventBus/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "EventBus/EventBus.RabbitMQ/"]
COPY ["Services/Authentication/Authentication.Infrastructure/Authentication.Infrastructure.csproj", "Services/Authentication/Authentication.Infrastructure/"]
COPY ["Services/Authentication/Authentication.Core/Authentication.Core.csproj", "Services/Authentication/Authentication.Core/"]
RUN dotnet restore "Services/Authentication/Authentication.API/Authentication.API.csproj"
COPY . .
WORKDIR "/src/Services/Authentication/Authentication.API"
RUN dotnet build "Authentication.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Authentication.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authentication.API.dll"]