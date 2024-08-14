# Specify the base image for the build stage
ARG DOTNET_VERSION=6.0
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
ARG TARGETARCH
ARG TARGETVARIANT
WORKDIR /app

# Copy everything and restore as distinct layers
COPY . ./
RUN dotnet restore Tv7Playlist.sln

# Build and publish
RUN if [ "$TARGETARCH" = "amd64" ]; then \
        RID=linux-x64; \
    elif [ "$TARGETARCH" = "arm64" ]; then \
        RID=linux-arm64; \
    elif [ "$TARGETARCH" = "arm" ] && [ "$TARGETVARIANT" = "v7" ]; then \
        RID=linux-arm; \
    elif [ "$TARGETARCH" = "386" ]; then \
        RID=linux-x86; \
    else \
        echo "Unsupported architecture: $TARGETARCH$TARGETVARIANT"; \
        exit 1; \
    fi && \
    dotnet publish -c Release -o out --no-restore --self-contained false -r $RID Tv7Playlist.sln

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}
WORKDIR /app
RUN mkdir /data

COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

VOLUME [ "/data" ]

ENTRYPOINT ["dotnet", "Tv7Playlist.dll"]
