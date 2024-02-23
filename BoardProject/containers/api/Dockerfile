FROM mcr.microsoft.com/dotnet/core/sdk:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:8.0 AS build
WORKDIR /project
COPY ["src/BoardProject.API/BoardProject.API.csproj", "src/BoardProject.API/"]
COPY . .
WORKDIR "/project/src/BoardProject.API"
RUN dotnet build "BoardProject.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BoardProject.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BoardProject.API.dll"]