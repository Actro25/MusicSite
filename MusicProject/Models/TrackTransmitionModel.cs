namespace MusicProject.Models;

public class TrackTransmitionModel
{
    public string NameTrack { get; set; }
    public string IdTrack { get; set; }
    public string TypeTrack { get; set; }
    public ArtistModel Artist { get; set; }
    public ImageModel Image { get; set; }
    
    
}