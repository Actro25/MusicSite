namespace MusicProject.Models
{
    public class PlayListModel
    {
        public string? Name { get; set; }
        public string? Id { get; set; }
        public string? UrlImage { get; set; }
        public List<TrackInfo>? Tracks { get; set; }
        public string? NextUrlPlayLists { get; set; }
        public List<ArtistModel>? Artist { get; set; }
    }
}
