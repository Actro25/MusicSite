namespace MusicProject.Models;

public class TrackInfo
{
    public string? Img640 { get; set; }
    public string? Img300 { get; set; }
    public string? Img64 { get; set; }
    public string? TrackName { get; set; }
    public List<ArtistModel>? ArtistsNames { get; set; }
    public string? TrackId { get; set; }
}