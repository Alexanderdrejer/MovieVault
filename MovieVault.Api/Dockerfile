FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /api

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "MovieVault.Api.dll"]
