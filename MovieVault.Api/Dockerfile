FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /

COPY *.csproj ./api/
WORKDIR /api/
RUN dotnet restore

COPY . ./api/
WORKDIR /api
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "MovieVault.Api.dll"]
