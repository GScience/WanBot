FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App

# Copy files
COPY . ./

# Build release
RUN dotnet restore
RUN dotnet publish -c Release --os linux

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /App

# Create dir
VOLUME [ "/data" ]
RUN mkdir Plugin

COPY --from=build-env /App/WanBot/bin/Release/net6.0/linux-x64/publish/ .
COPY --from=build-env /App/Bin/Plugin/Release/net6.0/linux-x64/publish/ ./Plugin/

# Entry point
ENTRYPOINT ["dotnet", "WanBot.dll", "-config", "/data"]
