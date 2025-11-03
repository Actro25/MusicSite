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
                // Парсимо JSON і витягуємо масив треків
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
                            .ToArray()
                    })
                    .Select(track => $"{track.Name} - {string.Join(", ", track.Artists)}")
                    .ToArray();

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
                    })
                    .Select(title =>  $"{title.Name} - {string.Join(", ", title.Artist)}")
                    .ToArray();
                
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
}