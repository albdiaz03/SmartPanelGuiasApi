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

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartPanelGuiasApi.dll"]