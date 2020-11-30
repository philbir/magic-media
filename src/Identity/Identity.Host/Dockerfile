#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "/"]
COPY ["Versions.props", "/"]
COPY ["global.json", "/"]

COPY ["Identity.Host/Identity.Host.csproj", "Identity.Host/"]
COPY ["Identity.Abstractions/Identity.Abstractions.csproj", "Identity.Abstractions/"]
COPY ["Identity.Core/Identity.Core.csproj", "Identity.Core/"]
COPY ["Identity.Data.Mongo/Identity.Data.Mongo.csproj", "Identity.Data.Mongo/"]

RUN dotnet restore "Identity.Host/Identity.Host.csproj"
RUN dotnet restore "Identity.Abstractions/Identity.Abstractions.csproj"
RUN dotnet restore "Identity.Core/Identity.Core.csproj"
RUN dotnet restore "Identity.Data.Mongo/Identity.Data.Mongo.csproj"

COPY . .
WORKDIR "/src/"
RUN dotnet build "Identity.Host/Identity.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.Host/Identity.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagicMedia.Identity.Host.dll"]
