using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using MusicProject.Models;
using MusicProject.Servers;
using MusicProject.Tools;
namespace MusicProject.Hubs;

public class MusicHub : Hub
{
    public async Task SendSpotifyMusic(string nameMusic)
    {
        try
        {
            var dataMusic = await SpotifyService.SearchTrack(nameMusic);
            
            if (!string.IsNullOrEmpty(dataMusic))
            {
                var jsonDocument = JsonDocument.Parse(dataMusic);
                var tracks = jsonDocument.RootElement
                    .GetProperty("tracks")
                    .GetProperty("items")
                    .EnumerateArray()
                    .Select(track => new
                    {
                        Name = track.GetProperty("name").GetString(),
                        Artists = track.GetProperty("artists")
                            .EnumerateArray()
                            .Select(artist => artist.GetProperty("name").GetString())
                            .ToArray(),
                        Id = track.GetProperty("id").GetString(),
                        Image = track.GetProperty("album").GetProperty("images").EnumerateArray()
                        .FirstOrDefault(img => img.GetProperty("width").GetInt32() == 640)
                        .GetProperty("url").GetString()

                    })
                    .ToList();
                await Clients.Caller.SendAsync("ReceiveSpotifyMusic", tracks);
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveSpotifyMusic", Array.Empty<string>());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка в MusicHub: {ex.Message}");
            await Clients.Caller.SendAsync("ReceiveSpotifyMusic", Array.Empty<string>());
        }
    }

    public async Task SendSoundCloudMusic(string nameMusic)
    {
        try
        {
            var dataMusic = await SoundCloudService.SearchTrack(nameMusic);

            if (!string.IsNullOrEmpty(dataMusic))
            {
                var jsonDocument = JsonDocument.Parse(dataMusic);
                var tracks = jsonDocument.RootElement
                    .EnumerateArray()
                    .Select(title => new
                    {
                        Name = title.GetProperty("title").GetString(),
                        Artist = title.GetProperty("user").GetProperty("username").GetString(),
                        Id = title.GetProperty("id").GetRawText(),
                        Image = (!string.IsNullOrEmpty(title.GetProperty("artwork_url").GetString())) ? title.GetProperty("artwork_url").GetString() : title.GetProperty("user").GetProperty("avatar_url").GetString()
                    })
                    .ToList();
                
                await Clients.Caller.SendAsync("ReceiveSoundCloudMusic", tracks);
            }
            else
            {
               await Clients.Caller.SendAsync("ReceiveSoundCloudMusic", Array.Empty<string>()); 
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка в MusicHub: {ex.Message}");
            await Clients.Caller.SendAsync("ReceiveSoundCloudMusic", Array.Empty<string>());
        }
    }

    public async Task GetOneTrack(string idTrack, string platform, string name, string artist)
    {
        if (platform == "Spotify")
        {
            var audios = new StreamableTrackModel();
            var dataMusic = await SoundCloudService.SearchTrack(name);

            if (!string.IsNullOrEmpty(dataMusic))
            {
                var jsonDocument2 = JsonDocument.Parse(dataMusic);
                var tracks = jsonDocument2.RootElement
                    .EnumerateArray()
                    .Select(title => new
                    {
                        Name = title.GetProperty("title").GetString(),
                        ArtistUserName = title.GetProperty("user").GetProperty("username").GetString(),
                        Id = title.GetProperty("id").GetRawText(),
                        Image = (!string.IsNullOrEmpty(title.GetProperty("artwork_url").GetString())) ? title.GetProperty("artwork_url").GetString() : title.GetProperty("user").GetProperty("avatar_url").GetString(),
                        ArtistFullName = title.GetProperty("user").GetProperty("full_name").GetString()
                    })
                    .ToList();

                foreach (var itemTrack in tracks) {
                    var nameTrack = (itemTrack.Name == null)?"":itemTrack.Name;
                    var artistTrack = (itemTrack.ArtistUserName == null) ? "" : itemTrack.ArtistUserName;
                    var artistFullNameTrack = (itemTrack.ArtistFullName == null) ? "" : itemTrack.ArtistFullName;

                    name = (name == null) ? "" : name;
                    artist = (artist == null) ? "" : artist;

                    if ((FuzzySharp.Fuzz.Ratio(nameTrack, name) >= 75) && (FuzzySharp.Fuzz.Ratio(itemTrack.ArtistUserName, artist) >= 90 || FuzzySharp.Fuzz.Ratio(artistFullNameTrack, artist) >= 80)) {
                        Console.WriteLine("True");
                        var dataAudio = await SoundCloudService.GetStreamableTrack(itemTrack.Id);
                        var jsonDocument3 = JsonDocument.Parse(dataAudio);
                        audios = new StreamableTrackModel
                        {
                            HttpMp3128Url = (jsonDocument3.RootElement.TryGetProperty("http_mp3_128_url", out var poop1)) ? poop1.GetString() : null,
                            HlsMp3160Url = (jsonDocument3.RootElement.TryGetProperty("hls_mp3_128_url", out var poop2)) ? poop2.GetString() : null,
                            HlsAcc160Url = (jsonDocument3.RootElement.TryGetProperty("hls_aac_160_url", out var poop3)) ? poop3.GetString() : null,
                            HlsOpus64Url = (jsonDocument3.RootElement.TryGetProperty("hls_opus_64_url", out var poop4)) ? poop4.GetString() : null,
                            PreviewMp3128Url = (jsonDocument3.RootElement.TryGetProperty("preview_mp3_128_url", out var poop5)) ? poop5.GetString() : null,
                        };
                    }
                }
            }

            var dataTrack = await SpotifyService.FindOneTrack(idTrack);
            var jsonDocument = JsonDocument.Parse(dataTrack);
            var root = jsonDocument.RootElement;
            var track = new TrackInfo
            {
                Img = root.GetProperty("album").GetProperty("images").EnumerateArray()
                        .FirstOrDefault(img => img.GetProperty("width").GetInt32() == 640)
                        .GetProperty("url").GetString(),
                TrackName = root.GetProperty("name").GetString(),
                ArtistsNames = root.GetProperty("artists")
                        .EnumerateArray()
                        .Select(artist => new ArtistModel
                        {
                            NameArtist = artist.GetProperty("name").GetString(),
                            IdArtist = artist.GetProperty("id").GetString(),
                            TypeArtist = artist.GetProperty("type").GetString(),
                            UrlArtist = artist.GetProperty("external_urls").GetProperty("spotify").GetString()
                        }).ToList(),
                TrackId = root.GetProperty("id").GetString(),
                TrackUrl = root.GetProperty("external_urls").GetProperty("spotify").GetString()
            };
            await Clients.Caller.SendAsync("ReceiveOneTrack", new { track, audios });
        }
        else if (platform == "SoundCloud")
        {
            var dataAudio = await SoundCloudService.GetStreamableTrack(idTrack);
            var jsonDocument = JsonDocument.Parse(dataAudio);
            var audios = new StreamableTrackModel
            {
                HttpMp3128Url = (jsonDocument.RootElement.TryGetProperty("http_mp3_128_url", out var poop1))?poop1.GetString():null,
                HlsMp3160Url = (jsonDocument.RootElement.TryGetProperty("hls_mp3_128_url", out var poop2)) ? poop2.GetString() : null,
                HlsAcc160Url = (jsonDocument.RootElement.TryGetProperty("hls_aac_160_url", out var poop3)) ? poop3.GetString() : null,
                HlsOpus64Url = (jsonDocument.RootElement.TryGetProperty("hls_opus_64_url", out var poop4)) ? poop4.GetString() : null,
                PreviewMp3128Url = (jsonDocument.RootElement.TryGetProperty("preview_mp3_128_url", out var poop5)) ? poop5.GetString() : null,
            };

            var dataTrack = await SoundCloudService.GetOneTrack(idTrack);
            var jsonDocument2 = JsonDocument.Parse(dataTrack);
            var track = new TrackInfo
            {
                Img = jsonDocument2.RootElement.GetProperty("artwork_url").GetString(),
                TrackName = jsonDocument2.RootElement.GetProperty("title").GetString(),
                TrackId = jsonDocument2.RootElement.GetProperty("id").GetRawText(),
                TrackUrl = jsonDocument2.RootElement.GetProperty("permalink_url").GetString(),
                ArtistsNames = new List<ArtistModel>
                {
                    new ArtistModel
                    {
                        NameArtist = jsonDocument2.RootElement.GetProperty("user").GetProperty("username").GetString(),
                        IdArtist = jsonDocument2.RootElement.GetProperty("user").GetProperty("id").GetRawText(),
                        TypeArtist = jsonDocument2.RootElement.GetProperty("user").GetProperty("kind").GetString(),
                        AvatarArtist = jsonDocument2.RootElement.GetProperty("user").GetProperty("avatar_url").GetString(),
                        UrlArtist = jsonDocument2.RootElement.GetProperty("user").GetProperty("permalink_url").GetString()
                    }
                }
            };
            track.Img = (!string.IsNullOrEmpty(track.Img)) ? 
                track.Img.Replace("large", "t500x500") : 
                track.ArtistsNames[0].AvatarArtist.Replace("large", "t500x500");
            await Clients.Caller.SendAsync("ReceiveOneTrack", new { track, audios });
        }
        else {
            await Clients.Caller.SendAsync("ReceiveOneTrack", null);
        }
    }
    public async Task GetPlaylists(string namePlayList, string platform) {
        if (platform == "SoundCloud")
        {
            string data = await SoundCloudService.GetPlayList(namePlayList);
            var jsonDocument = JsonDocument.Parse(data);
            var root = jsonDocument.RootElement.GetProperty("collection");
            var playlists = root.EnumerateArray()
                    .Select(title => new PlayListModel
                    {
                        Name = title.GetProperty("title").GetString(),
                        UrlImage = (!string.IsNullOrEmpty(title.GetProperty("artwork_url").GetString())) ?
                        title.GetProperty("artwork_url").GetString().Replace("large", "t500x500") :
                        title.GetProperty("user").GetProperty("avatar_url").GetString(),
                        Id = title.GetProperty("id").GetRawText(),
                        NextUrlPlayLists = jsonDocument.RootElement.GetProperty("next_href").GetString(),
                        Tracks = title.GetProperty("tracks").EnumerateArray().Select(track => new TrackInfo
                        {
                            Img = (!string.IsNullOrEmpty(track.GetProperty("artwork_url").GetString())) ?
                                track.GetProperty("artwork_url").GetString().Replace("large", "t500x500") :
                                track.GetProperty("user").GetProperty("avatar_url").GetString(),
                            TrackName = track.GetProperty("title").GetString(),
                            TrackId = track.GetProperty("id").GetRawText(),
                            TrackUrl = track.GetProperty("permalink_url").GetString(),
                            ArtistsNames = new List<ArtistModel>
                            {
                                new ArtistModel
                                {
                                    NameArtist = track.GetProperty("user").GetProperty("username").GetString(),
                                    IdArtist = track.GetProperty("user").GetProperty("id").GetRawText()
                                }
                            }
                        }).ToList(),
                        Artist = new List<ArtistModel>
                        {
                            new ArtistModel{
                                NameArtist = title.GetProperty("user").GetProperty("username").GetString(),
                                IdArtist = title.GetProperty("user").GetProperty("id").GetRawText(),
                            }
                        }
                    })
                    .ToList();

            await Clients.Caller.SendAsync("ReceivePlayList", new { playlists, platform });
        }
        else if (platform == "Spotify")
        {
            string data = await SpotifyService.GetPlaylist(namePlayList);
            var jsonDocument = JsonDocument.Parse(data);
            var root = jsonDocument.RootElement.GetProperty("playlists").GetProperty("items");
            var playlists = root.EnumerateArray().Select(title => new PlayListModel
            {
                Name = title.SafeProperty("name")?.GetString() ?? string.Empty,
                Id = title.SafeProperty("id")?.GetRawText() ?? string.Empty,
                UrlImage = title.SafeProperty("images")?.EnumerateArray().FirstOrDefault().SafeProperty("url")?.GetString() ?? string.Empty,
                NextUrlPlayLists = jsonDocument.RootElement.SafeProperty("playlists")?.SafeProperty("next")?.GetString(),
                Tracks = new List<TrackInfo>(),
                Artist = new List<ArtistModel>
            {
                new ArtistModel
                {
                    NameArtist = title.SafeProperty("owner")?.SafeProperty("display_name")?.GetString() ?? string.Empty,
                    IdArtist = title.SafeProperty("owner")?.SafeProperty("id")?.GetString() ?? string.Empty,
                }
            }
            }).ToList();
            List<string> tracksUrl = root.EnumerateArray().Select(trackUrl =>
            {
                return trackUrl.SafeProperty("tracks")?.SafeProperty("href")?.GetString() ?? string.Empty;
            }).Where(url => !string.IsNullOrEmpty(url)).ToList();

            for (int i = 0; i < playlists.Count && i < tracksUrl.Count; i++)
            {
                var dataTracks = await SpotifyService.GetSeveralTracks(tracksUrl[i]);
                var jsDocument = JsonDocument.Parse(dataTracks);
                var root2 = jsDocument.RootElement.SafeProperty("tracks");

                if (root2 == null) continue;

                playlists[i].Tracks = root2.Value.EnumerateArray().Select(track => new TrackInfo
                {
                    Img = track.SafeProperty("album")?.SafeProperty("images")?.EnumerateArray()
                        .FirstOrDefault(img => img.SafeProperty("width")?.GetInt32() == 640)
                        .SafeProperty("url")?.GetString() ?? string.Empty,

                    TrackName = track.SafeProperty("name")?.GetString() ?? "Unknown Track",

                    ArtistsNames = track.SafeProperty("artists")?.EnumerateArray()
                        .Select(artist => new ArtistModel
                        {
                            NameArtist = artist.SafeProperty("name")?.GetString() ?? "Unknown Artist",
                            IdArtist = artist.SafeProperty("id")?.GetString() ?? string.Empty
                        }).ToList() ?? new List<ArtistModel>(),

                    TrackId = track.SafeProperty("id")?.GetString() ?? string.Empty,

                    TrackUrl = track.SafeProperty("external_urls")?.SafeProperty("spotify")?.GetString() ?? string.Empty
                }).ToList();
            }
            Console.WriteLine(JsonSerializer.Serialize(playlists, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
            await Clients.Caller.SendAsync("ReceivePlayList", null);
        }
        else {
            await Clients.Caller.SendAsync("ReceivePlayList", null);
        }
    }
}