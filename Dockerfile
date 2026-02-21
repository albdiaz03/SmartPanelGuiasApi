FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar el csproj desde la subcarpeta correcta
COPY SmartPanelGuiasApi/*.csproj ./SmartPanelGuiasApi/
RUN dotnet restore SmartPanelGuiasApi/SmartPanelGuiasApi.csproj

# Copiar todo el código
COPY . .

WORKDIR /app/SmartPanelGuiasApi
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/SmartPanelGuiasApi/out .

# Variables de entorno
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ConnectionStrings__DefaultConnection=postgresql://smartpaneldb_user:gRWUfpLQpKI6KhGbUISWqd7Asw2qE23w@dpg-d6cfqjjnv86c73e7th4g-a/smartpaneldb
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartPanelGuiasApi.dll"]