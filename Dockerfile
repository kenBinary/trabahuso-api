FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5176

ENV ASPNETCORE_URLS=http://+:5176

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["trabahuso-api.csproj", "./"]
RUN dotnet restore "trabahuso-api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "trabahuso-api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "trabahuso-api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "trabahuso-api.dll"]
