# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["HandbookChatbot.csproj", "./"]
RUN dotnet restore "HandbookChatbot.csproj"

# Copy the rest of the files and build
COPY . .
RUN dotnet build "HandbookChatbot.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "HandbookChatbot.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Environment variables for Railway
# Railway injects the PORT environment variable
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
EXPOSE 8080

ENTRYPOINT ["dotnet", "HandbookChatbot.dll"]
