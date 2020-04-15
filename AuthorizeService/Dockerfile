#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["AuthorizeService/AuthenticationService.csproj", "AuthorizeService/"]
COPY ["BSCWeb/CommonExtention/CoreExtention.csproj", "BSCWeb/CommonExtention/"]
COPY ["Entity/Entity/Entity.csproj", "Entity/Entity/"]
COPY ["UtilsStandard/UtilsStandardLib.csproj", "UtilsStandard/"]
COPY ["RepositoryNetStandard/Repository.csproj", "RepositoryNetStandard/"]
COPY ["ServiceNetStandard/Service.csproj", "ServiceNetStandard/"]
RUN dotnet restore "AuthorizeService/AuthenticationService.csproj"
COPY . .
WORKDIR "/src/AuthorizeService"
RUN dotnet build "AuthenticationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthenticationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthenticationService.dll"]