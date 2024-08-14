# TV7-Playlist

## Summary

This application is used to rewrite the TV7 multicast channel list by fiber7 m3u. The updated list will proxy the
multicast stream through udpxy and builds a stream that Plex can handle.

This is a fork of [phaefelfinger/tv7playlist](https://github.com/phaefelfinger/tv7playlist) with updated .NET version
and support for ARMv7 and ARM64 architectures. In case you are upgradeing from the original version, do create a backup
of your database before upgrading.

Features include:

- Resorting of the channel list
- Enable or disable a channel
- Enable or disable multiple channels at once
- Override the channel number -> better EPG Detection support in Plex / Emby
- Override the channel name -> better EPG Detection support in Plex / Emby

This project is licensed under GPLv2. See License file.

## Docker

### Run the application

You can run this application using Docker. To persist the database when updating, create a volume:

```shell
docker volume create tv7playlist_data
```

Next, create and run the Docker container:

```shell
docker run -t --name="tv7playlist" -p 8000:80 -e "UdpxyUrl=http://your.host.ip.of.udpxy:4022/udp" -v tv7playlist_data:/data --restart=unless-stopped ghcr.io/nachtalb/tv7playlist:latest
```

### Using Docker Compose

You can also use Docker Compose to run the application. Create a `docker-compose.yml` file with the following content:

```yaml
version: "3"
services:
  tv7playlist:
    image: ghcr.io/nachtalb/tv7playlist:latest
    container_name: tv7playlist
    ports:
      - "8000:80"
    environment:
      - UdpxyUrl=http://your.host.ip.of.udpxy:4022/udp
    volumes:
      - tv7playlist_data:/data
    restart: unless-stopped

volumes:
  tv7playlist_data:
```

Then run:

```shell
docker-compose up -d
```

### Building the Docker image

If you want to build the Docker image yourself:

1. Clone the repository:

   ```
   git clone https://github.com/Nachtalb/tv7playlist.git
   cd tv7playlist
   ```

2. Build the image:

   ```
   docker build -t tv7playlist .
   ```

3. Run the container:
   ```
   docker run -t --name="tv7playlist" -p 8000:80 -e "UdpxyUrl=http://your.host.ip.of.udpxy:4022/udp" -v tv7playlist_data:/data --restart=unless-stopped tv7playlist
   ```

### Environment variables

- `SourceType`: "M3U" or "Xspf"
- `SqLiteConnectionString`: "Data Source=/data/playlist.db"
- `TV7Url`: "https://api.init7.net/tvchannels.m3u" or "https://api.init7.net/tvchannels.xspf" or any other provider
- `UdpxyUrl`: "http://your.host.ip.of.udpxy:4022/udp" or empty
- `DownloadFileName`: "PlaylistTV7udpxy.m3u" or any name that should be sent as filename while downloading the list
