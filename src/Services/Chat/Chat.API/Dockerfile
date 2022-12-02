#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Chat/Chat.API/Chat.API.csproj", "Services/Chat/Chat.API/"]
COPY ["EventBus/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "EventBus/EventBus.RabbitMQ/"]
COPY ["EventBus/EventBus.Core/EventBus.Core.csproj", "EventBus/EventBus.Core/"]
COPY ["Services/Chat/Chat.Application/Chat.Application.csproj", "Services/Chat/Chat.Application/"]
COPY ["Services/Chat/Chat.Domain/Chat.Domain.csproj", "Services/Chat/Chat.Domain/"]
COPY ["Services/Chat/Chat.Infrastructure/Chat.Infrastructure.csproj", "Services/Chat/Chat.Infrastructure/"]
RUN dotnet restore "Services/Chat/Chat.API/Chat.API.csproj"
COPY . .
WORKDIR "/src/Services/Chat/Chat.API"
RUN dotnet build "Chat.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Chat.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chat.API.dll"]