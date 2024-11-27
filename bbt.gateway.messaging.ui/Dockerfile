#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["bbt.gateway.messaging.ui/bbt.gateway.messaging.ui.csproj", "bbt.gateway.messaging.ui/"]
COPY ["bbt.gateway.common/bbt.gateway.common.csproj", "bbt.gateway.common/"]
RUN dotnet restore "bbt.gateway.messaging.ui/bbt.gateway.messaging.ui.csproj"
COPY . .
WORKDIR "/src/bbt.gateway.messaging.ui"
RUN dotnet build "bbt.gateway.messaging.ui.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bbt.gateway.messaging.ui.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "bbt.gateway.messaging.ui.dll"]