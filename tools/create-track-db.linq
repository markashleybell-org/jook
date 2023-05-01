<Query Kind="Program">
  <NuGetReference>Dapper</NuGetReference>
  <NuGetReference>Microsoft.Data.SqlClient</NuGetReference>
  <NuGetReference>z440.atl.core</NuGetReference>
  <Namespace>ATL</Namespace>
  <Namespace>Dapper</Namespace>
  <Namespace>Microsoft.Data.SqlClient</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Text.Encodings.Web</Namespace>
  <RemoveNamespace>System.Xml</RemoveNamespace>
  <RemoveNamespace>System.Xml.Linq</RemoveNamespace>
  <RemoveNamespace>System.Xml.XPath</RemoveNamespace>
</Query>

 void Main()
{    
    var queryPath = Path.GetDirectoryName(Util.CurrentQueryPath);
    var audioFilesPath = Path.Combine(queryPath, "audio");

    var extensions = new[] { 
        ".mp3"
    };
    
    var files = Directory
        .EnumerateFiles(audioFilesPath, "*.*", new EnumerationOptions { RecurseSubdirectories = true })
        .Where(f => extensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase));

    var songs = files.Select((f, i) => {
        // https://github.com/Zeugma440/atldotnet
        var track = new Track(f);
        
        return new {
            // Yes, it's horrible, but none of the many, many .NET variants of URLEncode actually seem to do it properly (still, in 2023)
            url = f.Replace(audioFilesPath, string.Empty).Replace(@"\", "/").Replace(" ", "%20").Replace("'", "%27").Replace("(", "%28").Replace(")", "%29"),
            artist = track.Artist,
            album = track.Album,
            title = track.Title
        };
    });

    var orderedSongs = songs.OrderBy(s => s.artist).ThenBy(s => s.album).ThenBy(s => s.title);

    var artists = new Dictionary<string, int>();
    var albums = new Dictionary<string, int>();
    var tracks = new Dictionary<string, int>();

    using var conn = new SqlConnection("Server=localhost;Database=jook;Trusted_Connection=yes;TrustServerCertificate=true;");
    
    var existingArtists = conn.Query<(int ArtistID, string Name)>("SELECT ArtistID, Name FROM Artists ORDER BY Name");
    var existingAlbums = conn.Query<(int AlbumID, string Title)>("SELECT AlbumID, Title FROM Albums ORDER BY Title");
    var existingTracks = conn.Query<(int TrackID, string Title)>("SELECT TrackID, Title FROM Tracks ORDER BY Title");
    
    foreach (var artist in existingArtists)
    {
        artists.Add(artist.Name, artist.ArtistID);
    }

    foreach (var album in existingAlbums)
    {
        albums.Add(album.Title, album.AlbumID);
    }
    
    foreach (var track in existingTracks)
    {
        tracks.Add(track.Title, track.TrackID);
    }

    foreach (var song in orderedSongs)
    {
        if (!artists.TryGetValue(song.artist, out var artistID))
        {
            // Create a new artist record
            artistID = (int)conn.ExecuteScalar("INSERT INTO Artists (Name) VALUES (@Name); SELECT CAST(SCOPE_IDENTITY() AS INT);", new { Name = song.artist });
            
            artists.Add(song.artist, artistID);
        }

        if (!albums.TryGetValue(song.album, out var albumID))
        {
            // Create a new album record
            albumID = (int)conn.ExecuteScalar("INSERT INTO Albums (ArtistID, Title) VALUES (@ArtistID, @Title); SELECT CAST(SCOPE_IDENTITY() AS INT);", new { artistID, Title = song.album });
            
            albums.Add(song.album, albumID);
        }

        if (!tracks.TryGetValue(song.title, out var trackID))
        {
            // Create a new track record
            trackID = (int)conn.ExecuteScalar("INSERT INTO Tracks (AlbumID, Title, Url) VALUES (@AlbumID, @Title, @Url); SELECT CAST(SCOPE_IDENTITY() AS INT);", new { albumID, song.title, song.url });

            tracks.Add(song.title, trackID);
        }
    }
    
    artists.Dump();
    albums.Dump();
    tracks.Dump();
}
