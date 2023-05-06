public record Artist(int TrackID, string Name);

public record Album(int AlbumID, int ArtistID, string Title);

public record Track(int TrackID, int AlbumID, string Title, string Url);

public record TrackSummary(int ArtistID, string ArtistName, int AlbumID, string AlbumTitle, int TrackID, string TrackTitle, string Url);
