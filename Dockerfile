FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Api/WordAnalytics.Api/WordAnalytics.Api.csproj Api/WordAnalytics.Api/
COPY Counting/WordAnalytics.Counting.csproj Counting/
COPY Ranking/WordAnalytics.Ranking.csproj Ranking/

RUN dotnet restore Api/WordAnalytics.Api/WordAnalytics.Api.csproj

COPY Api/WordAnalytics.Api/ Api/WordAnalytics.Api/
COPY Counting/ Counting/
COPY Ranking/ Ranking/

RUN dotnet publish Api/WordAnalytics.Api/WordAnalytics.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "WordAnalytics.Api.dll"]