#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DoberDogBot.Worker/DoberDogBot.Worker.csproj", "DoberDogBot.Worker/"]
COPY ["DoberDogBot.Domain/DoberDogBot.Domain.csproj", "DoberDogBot.Domain/"]
COPY ["DoberDogBot.Infrastructure/DoberDogBot.Infrastructure.csproj", "DoberDogBot.Infrastructure/"]
COPY ["DoberDogBot.Application/DoberDogBot.Application.csproj", "DoberDogBot.Application/"]
RUN dotnet restore "DoberDogBot.Worker/DoberDogBot.Worker.csproj"
COPY . .
WORKDIR "/src/DoberDogBot.Worker"
RUN dotnet build "DoberDogBot.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoberDogBot.Worker.csproj" -c Release -o /app/publish

ENV TZ=Europe/Istanbul

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoberDogBot.Worker.dll"]