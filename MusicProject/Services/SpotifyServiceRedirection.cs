using MusicProject.Models;
using MusicProject.Servers;
using MusicProject.Tools;
using System.Text.Json;

namespace MusicProject.Services
{
    public static class SpotifyServiceRedirection
    {
        public static async Task<List<TrackInfo>?> GetSeveralTracksRedirection(string? url) {
            if (url == "" || url == null) return null;
            var dataTrack = await SpotifyService.GetSeveralTracks(url);
            if (dataTrack == null) return null;
            var root2 = JsonDocument.Parse(dataTrack).RootElement.GetProperty("items");
            return root2.EnumerateArray().Select(track => new TrackInfo
            {
                Img = track.SafeProperty("track")?.SafeProperty("album")?.SafeProperty("images")?.EnumerateArray()
                    .FirstOrDefault(img => img.SafeProperty("width")?.GetInt32() == 640)
                    .SafeProperty("url").SafeString(),
                TrackName = track.SafeProperty("track")?.SafeProperty("name").SafeString(),
                TrackId = track.SafeProperty("track")?.SafeProperty("id").SafeString(),
                TrackUrl = track.SafeProperty("track")?.SafeProperty("external_urls")?.SafeProperty("spotify").SafeString(),
                ArtistsNames = track.SafeProperty("track")?.SafeProperty("artists")?.EnumerateArray().Select(artist => new ArtistModel
                {
                    NameArtist = artist.SafeProperty("name").SafeString(),
                    IdArtist = artist.SafeProperty("id").SafeString()
                }).ToList(),
            }).ToList();
        }
    }
}
