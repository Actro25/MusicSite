namespace MusicProject.Servers
{
    public class JamedoMusicService
    {
        private static string client_id;
        private static string client_secret;
        private static string code;

        private static bool _isInitialized = false;
        public static async void Initialize(IConfiguration configuration)
        {
            if (_isInitialized) return;

            client_id = configuration["JamendoMusic:Tokens:JAMENDOMUSIC_CLIENT_ID"];
            client_secret = configuration["JamendoMusic:Tokens:JAMENDOMUSIC_CLIENT_SECRET"];

            _isInitialized = true;
            Console.WriteLine("✅ FreeSoundService initialized");
        }
        public static async Task GetJamedoMusicTrack(string name, string artist_name)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync($"https://api.jamendo.com/v3.0/tracks/?client_id={client_id}&format=jsonpretty&artist_name={artist_name}&name={name}");
            var responseContent = await response.Result.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
        }
    }
}
