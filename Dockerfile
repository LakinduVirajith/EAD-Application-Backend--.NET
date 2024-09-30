# USE THE OFFICIAL ASP.NET CORE RUNTIME AS A BASE IMAGE
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# USE THE SDK IMAGE TO BUILD THE APPLICATION
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# COPY THE PROJECT FILE AND RESTORE DEPENDENCIES
COPY ["EAD-Backend-Application--.NET.csproj", "./"]
RUN dotnet restore

# COPY THE REST OF THE APPLICATION CODE
COPY . .

# BUILD THE APPLICATION
RUN dotnet build -c Release -o /app/build

# PUBLISH THE APPLICATION
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# FINAL STAGE, COPY THE PUBLISHED APPLICATION AND SET ENTRY POINT
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EAD-Backend-Application--.NET.dll"]
