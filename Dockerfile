FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Claims.sln ./
COPY Claims/Claims.csproj Claims/
COPY Claims.Tests/Claims.Tests.csproj Claims.Tests/
RUN dotnet restore Claims/Claims.csproj

COPY . ./
RUN dotnet publish Claims/Claims.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Claims.dll"]
