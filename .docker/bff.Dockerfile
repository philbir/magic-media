#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0.100 AS build
WORKDIR /src

COPY ["global.json", "/"]
COPY ["Directory.Build.props", "/"]
COPY ["Directory.Packages.props", "/"]
COPY ["src/Directory.Build.props", "/src"]
COPY ["src/Versions.props", "/src"]
COPY ["src/Shared/AspNetCore/Shared.AspNetCore.csproj", "src/Shared/AspNetCore/"]
COPY ["src/Shared/Telemetry/Telemetry.csproj", "src/Shared/Telemetry/"]
COPY ["src/Services/Bff.Host/Bff.Host.csproj", "src/Services/Bff.Host/"]

RUN dotnet restore "src/Services/Bff.Host/Bff.Host.csproj"

COPY . .
WORKDIR "/src/"
RUN dotnet build "src/Services/Bff.Host/Bff.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Services/Bff.Host/Bff.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagicMedia.Bff.dll"]


