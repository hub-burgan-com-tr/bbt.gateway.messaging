FROM mcr.microsoft.com/dotnet/aspnet8.0 AS base
WORKDIR /app
RUN apt-get update \ && DEBIAN_FRONTEND=noninteractive \    apt-get install --no-install-recommends --assume-yes \      libgdiplus
RUN adduser smsgatewayuser --disabled-password --gecos "" && chown -R smsgatewayuser:smsgatewayuser /app
USER smsgatewayuser

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["bbt.gateway.messaging.csproj", "."]
COPY ["../bbt.gateway.common/bbt.gateway.common.csproj", "../bbt.gateway.common/"]
RUN dotnet restore "./bbt.gateway.messaging.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "bbt.gateway.messaging.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bbt.gateway.messaging.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://*:5000
ENTRYPOINT ["dotnet", "bbt.gateway.messaging.dll"]
