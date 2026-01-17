FROM mcr.microsoft.com/dotnet/sdk:9.0 AS part-one

WORKDIR /app

COPY *.sln ./
COPY Backend/. ./Backend/
RUN dotnet publish "./Backend/Servico Backend.csproj"\
    -c Release \
    -o /app/publish



FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

RUN mkdir -p ./publish/Database/

EXPOSE 5050

COPY --from=part-one /app/publish/. ./publish

WORKDIR /app/publish

ENTRYPOINT [ "dotnet", "Servico Backend.dll" ]