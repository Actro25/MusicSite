using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using MusicProject.Models;
using MusicProject.Servers;

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

    public async Task GetOneTrack(string idTrack, string platform)
    {
        if (platform == "Spotify")
        {
            var dataTrack = await SpotifyService.FindOneTrack(idTrack);
            if (!string.IsNullOrEmpty(dataTrack))
            {
                var jsonDocument = JsonDocument.Parse(dataTrack);
                var root = jsonDocument.RootElement;
                var track = new List<TrackInfo>
                {
                    new TrackInfo
                    {
                        Img640 = root.GetProperty("album").GetProperty("images").EnumerateArray()
                            .FirstOrDefault(img => img.GetProperty("width").GetInt32() == 640)
                            .GetProperty("url").GetString(),
                        Img300 = root.GetProperty("album").GetProperty("images").EnumerateArray()
                            .FirstOrDefault(img => img.GetProperty("width").GetInt32() == 300)
                            .GetProperty("url").GetString(),
                        Img64 = root.GetProperty("album").GetProperty("images").EnumerateArray()
                            .FirstOrDefault(img => img.GetProperty("width").GetInt32() == 64)
                            .GetProperty("url").GetString(),
                        TrackName = root.GetProperty("name").GetString(),
                        ArtistsNames = root.GetProperty("artists")
                            .EnumerateArray()
                            .Select(artist => new ArtistModel
                            {
                                NameArtist = artist.GetProperty("name").GetString(),
                                IdArtist = artist.GetProperty("id").GetString(),
                                TypeArtist = artist.GetProperty("type").GetString()
                            }).ToList(),
                        TrackId = root.GetProperty("id").GetString()
                    }
                };
                await Clients.Caller.SendAsync("ReceiveOneTrack", track);
            }
            else
            {
                
            }
        }
        else if (platform == "SoundCloud")
        {
            
        }
        await Clients.Caller.SendAsync("ReceiveOneTrack", idTrack, platform);
    }
}