FROM mcr.microsoft.com/dotnet/sdk:9.0 AS part-one

WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY Backend/. ./
RUN dotnet publish \
    -c Release \
    -o /app/publish \
    --no-restore



FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

RUN mkdir -p ./publish/Database/

EXPOSE 5050

COPY --from=part-one /app/publish/. ./publish

WORKDIR /app/publish

ENTRYPOINT [ "dotnet", "Servico Backend.dll" ]