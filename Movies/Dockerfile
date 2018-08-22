FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Movies/Movies.csproj Movies/
RUN dotnet restore Movies/Movies.csproj
COPY . .
WORKDIR /src/Movies
RUN dotnet build Movies.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Movies.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Movies.dll"]
