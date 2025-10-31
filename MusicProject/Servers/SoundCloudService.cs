using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace MusicProject.Models;

public class SoundCloudService
{
    private static string client_id;
    private static string client_secret;
    
    private static string _souncCloundToken;
    private static string _refreshToken;
    private static long _tokenExpiryTime;
    private static bool _isInitialized = false;

    public static void Initialize(IConfiguration configuration)
    {
        if (_isInitialized) return;
        
        client_id = configuration["SoundCloud:Tokens:SOUNDCLOUD_CLIENT_ID"];
        client_secret = configuration["SoundCloud:Tokens:SOUNDCLOUD_CLIENT_SECRET"];

        _isInitialized = true;
        Console.WriteLine("✅ SoundCloudService initialized");
    }

    private static async Task GetSoundCloudToken()
    {
        byte[] clients = Encoding.UTF8.GetBytes($"{client_id}:{client_secret}");
        string result = Convert.ToBase64String(clients);
        
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", result);
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        
        
        var RequestBody = new List<KeyValuePair<string, string>>
        {
            new ("grant_type", "client_credentials"),
        };
        
        var content = new FormUrlEncodedContent(RequestBody);
        
        var response = client.PostAsync("https://secure.soundcloud.com/oauth/token", content);
        await DeserializeToken(response);
        
    }

    private async Task<string> GetValidToken()
    {
        if (!string.IsNullOrWhiteSpace(_souncCloundToken) && DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() < _tokenExpiryTime - 300000)
        {
            return _souncCloundToken;
        }
        
        await RefreshToken();
        return _souncCloundToken;
    }
    private static async Task RefreshToken()
    {
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var RequestBody = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "refresh_token"),
            new("client_id", "mmYVKGDNj8JpxnLXvrdvxSFY4ElZrFEU"),
            new("client_secret", "Cgj6tgqpqxboTgaPalW2enKBPzpRTTr4"),
            new("refresh_token", $"{_refreshToken}"),
        };
        
        var content = new FormUrlEncodedContent(RequestBody);
        var response = client.PostAsync("https://secure.soundcloud.com/oauth/token", content);
        await DeserializeToken(response);
    }

    private static async Task DeserializeToken(System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> response)
    {
        var responseContent = response.Result.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<SoundCloudResponseModel>(responseContent.Result);
        
        _souncCloundToken =  tokenResponse.AccessToken;
        _refreshToken = tokenResponse.RefreshToken;
        _tokenExpiryTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + (tokenResponse.ExpiresIn * 1000);
    }

    public static async Task<string> SearchTrack(string query)
    {
        return "";
    }

    public static bool IsInitialized() => _isInitialized;
}