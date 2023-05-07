public record Artist(int TrackID, string Name);

public record Album(int AlbumID, int ArtistID, string Title);

public record Track(int TrackID, int AlbumID, string Title, string Url);

public record TrackSummary(int AlbumArtistID, string AlbumArtist, int ArtistID, string Artist, int AlbumID, string Album, int TrackID, string Title, string Url);
