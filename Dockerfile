FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-one

WORKDIR /app

COPY ./Backend/*.csproj /app/
RUN dotnet restore

COPY ./Backend/* /app/
RUN dotnet build