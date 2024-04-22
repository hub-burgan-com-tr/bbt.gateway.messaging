FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV ASPNETCORE_URLS=http://*:5000
EXPOSE 5000
WORKDIR /app
RUN apt-get update \
 && DEBIAN_FRONTEND=noninteractive \
    apt-get install --no-install-recommends --assume-yes \
      libgdiplus
RUN adduser -u 5679 --disabled-password --gecos "" smsgatewayuser && chown -R smsgatewayuser:smsgatewayuser /app
USER smsgatewayuser

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
#COPY ["bbt.gateway.messaging/bbt.gateway.messaging.csproj", "bbt.gateway.messaging/"]
#RUN ls -l bbt.gateway.messaging/
COPY . .
RUN dotnet restore -s https://burgan-baget.burgan.com.tr/v3/index.json -verbosity detailed "bbt.gateway.messaging/bbt.gateway.messaging.csproj"
WORKDIR "/src/bbt.gateway.messaging"
RUN dotnet build "bbt.gateway.messaging.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bbt.gateway.messaging.csproj" -c Release -o /app/publish 

## SECURITY SCAN
#RUN cd /src \
#    && curl https://static.snyk.io/cli/latest/snyk-linux -o snyk \
#    && chmod +x snyk \
#    && ls -l \
#    && ./snyk auth SNYK_TOKEN \
#    && ./snyk test --severity-threshold=critical  --file=bbt.gateway.messaging.sln

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "bbt.gateway.messaging.dll"]
