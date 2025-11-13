using Microsoft.AspNetCore.Mvc;
using MusicProject.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MusicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiProxyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _soundCloudToken;
        public ApiProxyController(IConfiguration configuration, IHttpClientFactory httpClientFactory) {
            _configuration = configuration;
            _soundCloudToken = SoundCloudService.GetSoundCloudToken(_configuration["SoundCloud:Tokens:SOUNDCLOUD_CLIENT_ID"], _configuration["SoundCloud:Tokens:SOUNDCLOUD_CLIENT_SECRET"]);
        }
        [HttpGet("stream/{url_track_music}")]
        public async Task<IActionResult> GetHls(string url_track_music)
        {
            var url = url_track_music.Replace("%2F", "/");
            Console.WriteLine("APICONTROLLER: " + url_track_music);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", _soundCloudToken);

            
            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }

                var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/vnd.apple.mpegurl";
                var content = await response.Content.ReadAsByteArrayAsync();

                return File(content, contentType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Proxy error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
