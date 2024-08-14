# Specify the base image for the build stage
ARG DOTNET_VERSION=6.0
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-alpine AS build
ARG TARGETARCH
ARG TARGETVARIANT
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ["Tv7Playlist/Tv7Playlist.csproj", "Tv7Playlist/"]
COPY ["Tv7Playlist.Core/Tv7Playlist.Core.csproj", "Tv7Playlist.Core/"]
COPY ["Tv7Playlist.Data/Tv7Playlist.Data.csproj", "Tv7Playlist.Data/"]
RUN dotnet restore "Tv7Playlist/Tv7Playlist.csproj"

# Copy everything else and build
COPY . .
# Build and publish
RUN if [ "$TARGETARCH" = "amd64" ]; then \
        RID=linux-musl-x64; \
    elif [ "$TARGETARCH" = "arm64" ]; then \
        RID=linux-musl-arm64; \
    elif [ "$TARGETARCH" = "arm" ] && [ "$TARGETVARIANT" = "v7" ]; then \
        RID=linux-musl-arm; \
    else \
        echo "Unsupported architecture: $TARGETARCH$TARGETVARIANT"; \
        exit 1; \
    fi && \
    echo "Building for RID: $RID" && \
    dotnet publish Tv7Playlist/Tv7Playlist.csproj -c Release -o out --no-restore \
    --self-contained true \
    -r $RID \
    /p:PublishSingleFile=true \
    /p:PublishTrimmed=false \
    /p:IncludeNativeLibrariesForSelfExtract=true \
    /p:InvariantGlobalization=true \
    /p:UseSystemResourceKeys=true \
    /p:IlcGenerateStackTraceData=false \
    /p:IlcOptimizationPreference=Size \
    /p:IlcDisableReflection=false \
    /p:DebugType=None \
    /p:DebugSymbols=false \
    /p:StripSymbols=true \
    /p:EnableCompressionInSingleFile=true

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime-deps:${DOTNET_VERSION}-alpine
WORKDIR /app
COPY --from=build /app/out .

# Remove unnecessary files
RUN find . -name '*.pdb' -type f -delete && \
    find . -name '*.xml' -type f -delete && \
    rm -rf *.dev.json

# Create a non-root user and set up permissions
RUN adduser -u 5678 --disabled-password --gecos "" appuser && \
    mkdir -p /app/data && \
    chown -R appuser:appuser /app

USER appuser

ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
EXPOSE 80

VOLUME [ "/app/data" ]

ENTRYPOINT ["./Tv7Playlist"]
