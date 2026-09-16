FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["VaultCore.slnx", "./"]
COPY ["VaultCore.SharedKernel/VaultCore.SharedKernel.csproj", "VaultCore.SharedKernel/"]
COPY ["VaultCore.Modules.Catalog/VaultCore.Modules.Catalog.csproj", "VaultCore.Modules.Catalog/"]
COPY ["VaultCore.Modules.Orders/VaultCore.Modules.Orders.csproj", "VaultCore.Modules.Orders/"]
COPY ["VaultCore.Modules.Inventory/VaultCore.Modules.Inventory.csproj", "VaultCore.Modules.Inventory/"]
COPY ["VaultCore.Api/VaultCore.Api.csproj", "VaultCore.Api/"]
COPY ["VaultCore.Tests.Unit/VaultCore.Tests.Unit.csproj", "VaultCore.Tests.Unit/"]
COPY ["VaultCore.Tests.Integration/VaultCore.Tests.Integration.csproj", "VaultCore.Tests.Integration/"]

RUN dotnet restore "VaultCore.slnx"

COPY . .
WORKDIR "/src/VaultCore.Api"
RUN dotnet build "VaultCore.Api.csproj" -c Release -o /app/build
RUN dotnet publish "VaultCore.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "VaultCore.Api.dll"]
