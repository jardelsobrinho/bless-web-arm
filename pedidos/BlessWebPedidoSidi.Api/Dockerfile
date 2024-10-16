#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV BS_DATASOURCE=LOCALHOST
ENV BS_PORT=3050
ENV BS_DATABASE=SIDI

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BlessWebPedidoSidi.Api/BlessWebPedidoSidi.Api.csproj", "BlessWebPedidoSidi.Api/"]
COPY ["BlessWebPedidoSidi.Application/BlessWebPedidoSidi.Application.csproj", "BlessWebPedidoSidi.Application/"]
COPY ["BlessWebPedidoSidi.Domain/BlessWebPedidoSidi.Domain.csproj", "BlessWebPedidoSidi.Domain/"]
COPY ["BlessWebPedidoSidi.Infra/BlessWebPedidoSidi.Infra.csproj", "BlessWebPedidoSidi.Infra/"]
COPY ["BlessWebPedidoSidi.Shared/BlessWebPedidoSidi.Shared.csproj", "BlessWebPedidoSidi.Shared/"]
RUN dotnet restore "./BlessWebPedidoSidi.Api/BlessWebPedidoSidi.Api.csproj"
COPY . .
WORKDIR "/src/BlessWebPedidoSidi.Api"
RUN dotnet build "./BlessWebPedidoSidi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build 

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlessWebPedidoSidi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlessWebPedidoSidi.Api.dll"]