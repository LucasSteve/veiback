FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY VeiCards.sln .
COPY src/Dominio/VeiCards.Dominio.csproj src/Dominio/
COPY src/Aplicacao/VeiCards.Aplicacao.csproj src/Aplicacao/
COPY src/Infraestrutura/VeiCards.Infraestrutura.csproj src/Infraestrutura/
COPY src/Api/VeiCards.Api.csproj src/Api/
RUN dotnet restore src/Api/VeiCards.Api.csproj

COPY src/ src/
RUN dotnet publish src/Api/VeiCards.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "VeiCards.Api.dll"]
