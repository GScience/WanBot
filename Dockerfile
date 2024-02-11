FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy csproject
COPY ./*.sln ./
COPY ./**/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done

# Restore
RUN dotnet restore

# Copy other files
COPY . ./

# Build release
ARG BUILD_NUMBER=0
RUN dotnet publish -c Release --os linux --version-suffix $BUILD_NUMBER

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /App

# Install vulkan
RUN apt-get update && \
    apt-get install -y libvulkan1 libc6-dev && \
    apt-get clean

# Create dir
VOLUME [ "/data" ]
RUN mkdir Plugin

# Set timezone
ENV TZ=Asia/Shanghai

COPY --from=build-env /App/WanBot/bin/Release/net8.0/linux-x64/publish/ .
COPY --from=build-env /App/Bin/Plugin/Release/net8.0/linux-x64/publish/ ./Plugin/

# Entry point
ENTRYPOINT ["dotnet", "WanBot.dll", "-config", "/data"]
