namespace MusicProject.Models;

public class TrackInfo
{
    public string? Img { get; set; }
    public string? TrackName { get; set; }
    public List<ArtistModel>? ArtistsNames { get; set; }
    public string? TrackId { get; set; }
}