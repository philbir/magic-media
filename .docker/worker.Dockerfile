#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ["global.json", "/"]
COPY ["Directory.Build.props", "/"]
COPY ["src/Directory.Build.props", "/src"]
COPY ["src/Versions.props", "/src"]
COPY ["src/Services/Worker/Worker.csproj", "src/Services/Worker/"]
COPY ["src/Services/AzureAI/AzureAI.csproj", "src/Services/AzureAI/"]
COPY ["src/Services/BingMaps/BingMaps.csproj", "src/Services/BingMaps/"]
COPY ["src/Services/Core/Core.csproj", "Csrc/Services//"]
COPY ["src/Services/Store.MongoDb/Store.MongoDb.csproj", "src/Services/Store.MongoDb/"]

#RUN dotnet restore "src/Services/Worker/Worker.csproj"

COPY . .
WORKDIR "/src/"
RUN dotnet build "src/Services/Worker/Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Services/Worker/Worker.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app

RUN apt-get update && apt-get install ffmpeg --yes

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagicMedia.Worker.dll"]
