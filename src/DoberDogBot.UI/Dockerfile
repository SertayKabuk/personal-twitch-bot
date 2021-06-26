#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DoberDogBot.UI/DoberDogBot.UI.csproj", "DoberDogBot.UI/"]
COPY ["DoberDogBot.Infrastructure/DoberDogBot.Infrastructure.csproj", "DoberDogBot.Infrastructure/"]
COPY ["DoberDogBot.Application/DoberDogBot.Application.csproj", "DoberDogBot.Application/"]
COPY ["DoberDogBot.Domain/DoberDogBot.Domain.csproj", "DoberDogBot.Domain/"]
RUN dotnet restore "DoberDogBot.UI/DoberDogBot.UI.csproj"
COPY . .
WORKDIR "/src/DoberDogBot.UI"
RUN dotnet build "DoberDogBot.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoberDogBot.UI.csproj" -c Release -o /app/publish

ENV TZ=Europe/Istanbul

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "DoberDogBot.UI.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet DoberDogBot.UI.dll