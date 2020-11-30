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
COPY ["src/Shared/AspNetCore/Shared.AspNetCore.csproj", "src/Shared/AspNetCore/"]
COPY ["src/Identity/Identity.Host/Identity.Host.csproj", "src/Identity/Identity.Host/"]
COPY ["src/Identity/Identity.Abstractions/Identity.Abstractions.csproj", "src/Identity/Identity.Abstractions/"]
COPY ["src/Identity/Identity.Core/Identity.Core.csproj", "src/Identity/Identity.Core/"]
COPY ["src/Identity/Identity.Data.Mongo/Identity.Data.Mongo.csproj", "src/Identity/Identity.Data.Mongo/"]

RUN dotnet restore "./src/Identity/Identity.Host/Identity.Host.csproj"

COPY . .
WORKDIR "/src/"
RUN dotnet build "./src/Identity/Identity.Host/Identity.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./src/Identity/Identity.Host/Identity.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagicMedia.Identity.Host.dll"]
