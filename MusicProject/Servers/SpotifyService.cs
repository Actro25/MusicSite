using System.Text;
using System.Text.Json;
using MusicProject.Models;

namespace MusicProject.Servers;

public class SpotifyService
{
    
    private static string _spotifyToken;
    private static long _tokenExpiryTime;
    private static string _spotifyClientId;
    private static string _spotifyClientSecret;
    private static bool _isInitialized = false;
    
    public static void Initialize(IConfiguration configuration)
    {
        if (_isInitialized) return;

        _spotifyClientId = configuration["Spotify:Tokens:SPOTIFY_CLIENT_ID"];
        _spotifyClientSecret = configuration["Spotify:Tokens:SPOTIFY_CLIENT_SECRET"];

        _isInitialized = true;
        Console.WriteLine("✅ SpotifyService initialized");
    }

    private static async Task<string> GetValidToken()
    {
        if (!string.IsNullOrEmpty(_spotifyToken) && DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() < _tokenExpiryTime - 300000)
        {
            return _spotifyToken;
        }

        await GetSpotifyToken();
        return _spotifyToken;
    }
    
    private static async Task GetSpotifyToken()
    {
        using var httpClient = new HttpClient();
        
        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(
                $"{_spotifyClientId}:{_spotifyClientSecret}"));

        httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {credentials}");
        
        var requestBody = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "client_credentials"),
        };
        
        var content = new FormUrlEncodedContent(requestBody);
        var response = await httpClient.PostAsync("https://accounts.spotify.com/api/token", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Spotify API error: {response.StatusCode}. {responseContent}");
        }
        
        var tokenResponse = JsonSerializer.Deserialize<SpotifyTokenResponseModel>(
            responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        _spotifyToken = tokenResponse.access_token;
        _tokenExpiryTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + (tokenResponse.expires_in * 1000);
        Console.WriteLine($"Отримано новий токен Spotify: {_spotifyToken}");
        Console.WriteLine($"Токен дійсний до: {DateTimeOffset.FromUnixTimeMilliseconds(_tokenExpiryTime)}");
    }

    public static async Task<string> SearchTrack(string query)
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("SpotifyService is not initialized. Call Initialize() first.");
        }
    
        try
        {
            var token = await GetValidToken();
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"https://api.spotify.com/v1/search?q={encodedQuery}&type=track&limit=10";
        
            Console.WriteLine($"🔍 Виконуємо пошук: {url}");
            Console.WriteLine($"🔑 Використовуємо токен: {token?.Substring(0, Math.Min(20, token?.Length ?? 0))}...");

            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"📡 Статус відповіді: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = $"Spotify search error: {response.StatusCode}. URL: {url}";
            
                if (!string.IsNullOrEmpty(responseContent))
                {
                    errorMessage += $". Response: {responseContent}";
                }
            
                throw new HttpRequestException(errorMessage);
            }

            return responseContent;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Помилка пошуку: {ex.Message}");
            Console.WriteLine($"🔍 Stack trace: {ex.StackTrace}");
            return null;
        }
    }
    public static bool IsInitialized() => _isInitialized;
}