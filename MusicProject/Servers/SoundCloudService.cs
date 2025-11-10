using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace MusicProject.Models;

public class SoundCloudService
{
    private static string client_id;
    private static string client_secret;
    
    private static string _souncCloudToken;
    private static string _refreshToken;
    private static long _tokenExpiryTime;
    private static bool _isInitialized = false;

    public static async void Initialize(IConfiguration configuration)
    {
        if (_isInitialized) return;
        
        client_id = configuration["SoundCloud:Tokens:SOUNDCLOUD_CLIENT_ID"];
        client_secret = configuration["SoundCloud:Tokens:SOUNDCLOUD_CLIENT_SECRET"];

        _isInitialized = true;
        GetSoundCloudToken();
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

    private static async Task<string> GetValidToken()
    {
        if (!string.IsNullOrWhiteSpace(_souncCloudToken) && DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() < _tokenExpiryTime - 300000)
        {
            return _souncCloudToken;
        }

        if (string.IsNullOrWhiteSpace(_souncCloudToken))
            await GetSoundCloudToken();
        
        await RefreshToken();
        return _souncCloudToken;
    }
    private static async Task RefreshToken()
    {
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var RequestBody = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "refresh_token"),
            new("client_id", client_id),
            new("client_secret", client_secret),
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
        
        _souncCloudToken =  tokenResponse.AccessToken;
        _refreshToken = tokenResponse.RefreshToken;
        _tokenExpiryTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + (tokenResponse.ExpiresIn * 1000);
    }

    public static async Task<string> SearchTrack(string query)
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("SoundCloudService not initialized");
        }

        try
        {
            var token = await GetValidToken();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", token);
            var url = $"https://api.soundcloud.com/tracks?q={query}&limit=5&offset=0";
            var response = await client.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = $"SoundCloud search error: {response.StatusCode}. URL {url}";
            
                if (!string.IsNullOrEmpty(responseContent))
                {
                    errorMessage += $". Response: {responseContent}";
                }
            
                throw new HttpRequestException(errorMessage);
            }
            Console.WriteLine(responseContent);
            return responseContent;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Помилка пошуку: {ex.Message}");
            Console.WriteLine($"🔍 Stack trace: {ex.StackTrace}");
            return null;
        }
    }
    public static async Task<string> GetStreamableTrack(string query)
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("SoundCloudService not initialized");
        }
        try {
            var token = await GetValidToken();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", token);
            var url = $"https://api.soundcloud.com/tracks/{query}/streams";
            var response = await client.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        } 
        catch (Exception ex) {
            Console.WriteLine($"❌ Помилка пошуку: {ex.Message}");
            Console.WriteLine($"🔍 Stack trace: {ex.StackTrace}");
            return null;
        }
    }
    public static async Task<string> GetOneTrack(string query) {
        if (!_isInitialized) 
        {
            throw new InvalidOperationException("SoundCloudService not initialized");
        }
        try 
        {
            var token = await GetValidToken();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", token);
            var url = $"https://api.soundcloud.com/tracks/{query}";
            var response = await client.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        } 
        catch(Exception ex) 
        {
            Console.WriteLine($"❌ Помилка пошуку: {ex.Message}");
            Console.WriteLine($"🔍 Stack trace: {ex.StackTrace}");
            return null;
        }
    }
    public static bool IsInitialized() => _isInitialized;
}