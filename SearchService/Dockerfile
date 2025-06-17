# Stage 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Kopiramo solution i projekte
COPY CarAuction.sln ./
COPY SearchService/SearchService.csproj ./SearchService/
COPY AuctionService/AuctionService.csproj ./AuctionService/
COPY Contracts/Contracts.csproj ./Contracts/

# Restore
RUN dotnet restore ./SearchService/SearchService.csproj

# Kopiraj ceo source (posle restore da se koristi cache)
COPY . .

# Publish
RUN dotnet publish ./SearchService/SearchService.csproj -c Debug -o /app/publish

# Stage 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SearchService.dll"]
