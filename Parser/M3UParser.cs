using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace Tv7Playlist.Parser
{
    internal class M3UParser : IPlaylistParser
    {
        private const string ExtInfStartTag = "#EXTINF:";
        private const string ExtFileStartTag = "#EXTM3U"; 

        private readonly ILogger<M3UParser> _logger;

        public M3UParser(ILogger<M3UParser> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<ParsedTrack>> ParseFromStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            _logger.LogInformation(LoggingEvents.ParsingM3uPlayList, "Parsing m3u file content");
            
            EnsureStreamIsAtBeginning(stream);
            
            using (var reader = new StreamReader(stream))
            {
                var tracks = await ParseTracksFromStreamAsync(reader);
                
                _logger.LogInformation(LoggingEvents.ParsingM3uPlayList, "Parsing m3u file finished");

                return tracks;
            }
        }

        private async Task<IReadOnlyCollection<ParsedTrack>> ParseTracksFromStreamAsync(StreamReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            if (!await StreamHasValidStartTagAsync(reader))
            {
                _logger.LogError(LoggingEvents.ParsingM3uPlayList, $"Could not parse stream as it did not start with {ExtFileStartTag}");
                return new List<ParsedTrack>(300);
            }

            var tracks = new List<ParsedTrack>(300);
            var currentId = 1000;

            while (!reader.EndOfStream)
            {
                await ParseTracksAsync(reader, tracks, currentId);
                currentId++;
            }

            return tracks;
        }

        private static async Task<bool> StreamHasValidStartTagAsync(StreamReader reader)
        {
            var startLine = await ReadLineSafeAsync(reader);
            var stramHasValidStartTag = !startLine.Trim().ToUpper().Equals(ExtInfStartTag);
            return stramHasValidStartTag;
        }

        private void EnsureStreamIsAtBeginning(Stream stream)
        {
            if (stream.Position == 0) return;
            
            _logger.LogWarning(LoggingEvents.ParsingM3uPlayList, "Stream not positioned at the beginning. Repositioning!");
            stream.Position = 0;
        }

        private async Task ParseTracksAsync(TextReader reader, ICollection<ParsedTrack> tracks, int currentId)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (tracks == null) throw new ArgumentNullException(nameof(tracks));
            if (currentId <= 0) throw new ArgumentOutOfRangeException(nameof(currentId));

            var metaLine = await ReadLineSafeAsync(reader);            
            if (!metaLine.StartsWith(ExtInfStartTag))
            {
                _logger.LogDebug(LoggingEvents.ParsingM3uPlayList,
                    "Line {lineNumber} {metaLine} is not a valid start channel start line",
                    currentId.ToString(), metaLine);
                return;
            }

            var url = await ReadLineSafeAsync(reader);
            var track = CreateTrack(currentId, metaLine, url);
            if (track != null)
            {
                tracks.Add(track);
                _logger.LogDebug(LoggingEvents.ParsingM3uPlayList, "Parsed track {track}", track);
            }
            else
            {
                _logger.LogWarning(LoggingEvents.ParsingM3uPlayList,
                    "Could not parse lines {metaLine} with url {url}", metaLine, url);
            }
        }

        private static async Task<string> ReadLineSafeAsync(TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            
            var line = await reader.ReadLineAsync();
            
            return line ?? string.Empty;
        }

        private ParsedTrack CreateTrack(int currentId, string metaLine, string url)
        {
            if (string.IsNullOrWhiteSpace(metaLine))
                return null;

            if (string.IsNullOrWhiteSpace(url))
                return null;

            var fields = metaLine.Replace(ExtInfStartTag, string.Empty).Split(",");
            var name = fields.Length >= 2 ? fields[1] : $"{currentId}-unknown";
            
            return new ParsedTrack(currentId, name, url);
        }
    }
}