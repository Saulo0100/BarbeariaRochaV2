# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy csproj first for better layer caching
COPY *.csproj ./
RUN dotnet restore

# Copy the rest and publish
COPY . ./
RUN dotnet publish -c Release -o /app/out --no-restore

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app

# App will listen on 8080 (matches your compose mapping 5000:8080)
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "BarbeariaRocha.dll"]
