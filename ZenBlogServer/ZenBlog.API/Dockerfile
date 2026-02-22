# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy everything and publish API
COPY . .
RUN dotnet restore "ZenBlog.API/ZenBlog.API.csproj"
RUN dotnet publish "ZenBlog.API/ZenBlog.API.csproj" -c Release -o /app/publish --no-restore

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Tools used by the wait script (curl + nc)
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl netcat-openbsd \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
COPY docker/wait-for-services.sh /app/wait-for-services.sh
RUN chmod +x /app/wait-for-services.sh

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["/app/wait-for-services.sh"]
