FROM mcr.microsoft.com/dotnet/core/sdk:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:8.0 AS build
WORKDIR /project
COPY ["src/BoardJob.API/BoardJob.API.csproj", "src/BoardJob.API/"]
COPY . .
WORKDIR "/project/src/BoardJob.API"
RUN dotnet build "BoardJob.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BoardJob.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BoardJob.API.dll"]