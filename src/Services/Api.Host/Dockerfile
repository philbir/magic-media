#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "/"]
COPY ["Versions.props", "/"]
COPY ["global.json", "/"]
COPY ["Api.Host/Api.Host.csproj", "Api.Host/"]
COPY ["AzureAI/AzureAI.csproj", "AzureAI/"]
COPY ["BingMaps/BingMaps.csproj", "BingMaps/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["GraphQL/GraphQL.csproj", "GraphQL/"]
COPY ["Store.MongoDb/Store.MongoDb.csproj", "Store.MongoDb/"]

RUN dotnet restore "Api.Host/Api.Host.csproj"

COPY . .
WORKDIR "/src/"
RUN dotnet build "Api.Host/Api.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.Host/Api.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagicMedia.Api.Host.dll"]
