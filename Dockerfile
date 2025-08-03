# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

USER root

RUN apt-get update && \
    apt-get install -y python3-full python3-pip && \
    ln -sf /usr/bin/python3 /usr/bin/python && \
    python --version && \
    python -m venv /opt/venv && \
    /opt/venv/bin/pip install --upgrade pip

ENV PATH="/opt/venv/bin:$PATH"


EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NostalgiaBackend/NostalgiaBackend.csproj", "NostalgiaBackend/"]
RUN dotnet restore "./NostalgiaBackend/NostalgiaBackend.csproj"
COPY . .
WORKDIR "/src/NostalgiaBackend"
RUN dotnet build "./NostalgiaBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NostalgiaBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NostalgiaBackend.dll"]