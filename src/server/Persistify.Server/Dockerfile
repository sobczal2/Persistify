FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 50051

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/server/Persistify.Server/Persistify.Server.csproj", "src/server/Persistify.Server/"]
COPY ["src/server/Persistify.Server.RequestHandlers/Persistify.Server.RequestHandlers.csproj", "src/server/Persistify.Server.RequestHandlers/"]
COPY ["src/shared/Persistify.Responses/Persistify.Responses.csproj", "src/shared/Persistify.Responses/"]
COPY ["src/shared/Persistify.Dtos/Persistify.Dtos.csproj", "src/shared/Persistify.Dtos/"]
COPY ["src/server/Persistify.Server.ErrorHandling/Persistify.Server.ErrorHandling.csproj", "src/server/Persistify.Server.ErrorHandling/"]
COPY ["src/server/Persistify.Server.Domain/Persistify.Server.Domain.csproj", "src/server/Persistify.Server.Domain/"]
COPY ["src/server/Persistify.Server.Management/Persistify.Server.Management.csproj", "src/server/Persistify.Server.Management/"]
COPY ["src/shared/Persistify.Concurrency/Persistify.Concurrency.csproj", "src/shared/Persistify.Concurrency/"]
COPY ["src/shared/Persistify.Helpers/Persistify.Helpers.csproj", "src/shared/Persistify.Helpers/"]
COPY ["src/server/Persistify.Server.Files/Persistify.Server.Files.csproj", "src/server/Persistify.Server.Files/"]
COPY ["src/server/Persistify.Server.Configuration/Persistify.Server.Configuration.csproj", "src/server/Persistify.Server.Configuration/"]
COPY ["src/server/Persistify.Server.Persistence/Persistify.Server.Persistence.csproj", "src/server/Persistify.Server.Persistence/"]
COPY ["src/server/Persistify.Server.Serialization/Persistify.Server.Serialization.csproj", "src/server/Persistify.Server.Serialization/"]
COPY ["src/server/Persistify.Server.Indexes/Persistify.Server.Indexes.csproj", "src/server/Persistify.Server.Indexes/"]
COPY ["src/server/Persistify.Server.Fts/Persistify.Server.Fts.csproj", "src/server/Persistify.Server.Fts/"]
COPY ["src/server/Persistify.Server.Indexes.DataStructures/Persistify.Server.Indexes.DataStructures.csproj", "src/server/Persistify.Server.Indexes.DataStructures/"]
COPY ["src/server/Persistify.Server.Mappers/Persistify.Server.Mappers.csproj", "src/server/Persistify.Server.Mappers/"]
COPY ["src/server/Persistify.Server.Security/Persistify.Server.Security.csproj", "src/server/Persistify.Server.Security/"]
COPY ["src/server/Persistify.Server.Validation/Persistify.Server.Validation.csproj", "src/server/Persistify.Server.Validation/"]
COPY ["src/shared/Persistify.Requests/Persistify.Requests.csproj", "src/shared/Persistify.Requests/"]
COPY ["src/server/Persistify.Server.HostedServices/Persistify.Server.HostedServices.csproj", "src/server/Persistify.Server.HostedServices/"]
COPY ["src/shared/Persistify.Services/Persistify.Services.csproj", "src/shared/Persistify.Services/"]

COPY ["src/server/Persistify.Server/localhost.pfx", "/etc/persistify/localhost.pfx"]

RUN dotnet restore "src/server/Persistify.Server/Persistify.Server.csproj"
COPY . .
WORKDIR "/src/src/server/Persistify.Server"
RUN dotnet build "Persistify.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Persistify.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /etc/persistify/localhost.pfx /etc/persistify/localhost.pfx
ENTRYPOINT ["dotnet", "Persistify.Server.dll"]
