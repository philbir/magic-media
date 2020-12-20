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
COPY ["src/Services/Api.Host/Api.Host.csproj", "src/Services/Api.Host/"]
COPY ["src/Services/Authorization/Authorization.csproj", "src/Services/Authorization/"]
COPY ["src/Services/AzureAI/AzureAI.csproj", "src/Services/AzureAI/"]
COPY ["src/Services/BingMaps/BingMaps.csproj", "src/Services/BingMaps/"]
COPY ["src/Services/Core/Core.csproj", "Csrc/Services//"]
COPY ["src/Services/GraphQL/GraphQL.csproj", "src/Services/GraphQL/"]
COPY ["src/Services/Store.MongoDb/Store.MongoDb.csproj", "src/Services/Store.MongoDb/"]

RUN dotnet restore "src/Services/Api.Host/Api.Host.csproj"

COPY . .
WORKDIR "/src/"
RUN dotnet build "src/Services/Api.Host/Api.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Services/Api.Host/Api.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagicMedia.Api.Host.dll"]
