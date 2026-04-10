# ---- runtime base (NET 8) ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Railway expone normalmente el puerto 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

# ---- build & publish ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar sln y csproj primero (mejora el cache de restore)
COPY eduspace-platform.sln .
COPY FULLSTACKFURY.EduSpace.API/FULLSTACKFURY.EduSpace.API.csproj FULLSTACKFURY.EduSpace.API/
RUN dotnet restore FULLSTACKFURY.EduSpace.API/FULLSTACKFURY.EduSpace.API.csproj

# Copiar el resto del código y publicar
COPY . .
RUN dotnet publish FULLSTACKFURY.EduSpace.API/FULLSTACKFURY.EduSpace.API.csproj \
    -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ---- final image ----
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FULLSTACKFURY.EduSpace.API.dll"]
