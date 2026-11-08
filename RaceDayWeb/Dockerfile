# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RaceDayWeb/RaceDayWeb.csproj", "RaceDayWeb/"]
RUN dotnet restore "RaceDayWeb/RaceDayWeb.csproj"
COPY . .
WORKDIR "/src/RaceDayWeb"
RUN dotnet publish "RaceDayWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RaceDayWeb.dll"]
