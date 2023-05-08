<Query Kind="Program">
  <NuGetReference>AWSSDK.S3</NuGetReference>
  <NuGetReference>Dapper</NuGetReference>
  <NuGetReference>Microsoft.Data.SqlClient</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>System.Linq.Async</NuGetReference>
  <NuGetReference>z440.atl.core</NuGetReference>
  <Namespace>Amazon.S3</Namespace>
  <Namespace>Amazon.S3.Model</Namespace>
  <Namespace>ATL</Namespace>
  <Namespace>Dapper</Namespace>
  <Namespace>Microsoft.Data.SqlClient</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Encodings.Web</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
  <RemoveNamespace>System.Xml</RemoveNamespace>
  <RemoveNamespace>System.Xml.Linq</RemoveNamespace>
  <RemoveNamespace>System.Xml.XPath</RemoveNamespace>
</Query>

async Task Main()
{    
    var queryPath = Path.GetDirectoryName(Util.CurrentQueryPath);
    var audioFilesPath = Path.Combine(queryPath, "audio");

    var credentials = JsonConvert.DeserializeObject<S3Credentials>(File.ReadAllText(queryPath + @"\config.json")).Dump();
    
    BackblazeClient.Initialise(credentials);

    var artists = new Dictionary<string, int>();
    var albums = new Dictionary<string, int>();
    var tracks = new Dictionary<string, int>();
    var trackUrls = new Dictionary<string, int>();

    using var conn = new SqlConnection("Server=localhost;Database=jook;Trusted_Connection=yes;TrustServerCertificate=true;");

    var existingArtists = conn.Query<(int ArtistID, string Name)>("SELECT ArtistID, Name FROM Artists ORDER BY Name");
    var existingAlbums = conn.Query<(int AlbumID, string Key)>("SELECT AlbumID, CAST(ArtistID AS NVARCHAR) + '~' + Title AS [Key] FROM Albums ORDER BY Title");
    var existingTracks = conn.Query<(int TrackID, string Key)>("SELECT TrackID, CAST(ArtistID AS NVARCHAR) + '~' + Title AS [Key] FROM Tracks ORDER BY Title");
    var existingTrackUrls = conn.Query<(int TrackID, string Url)>("SELECT TrackID, Url FROM Tracks ORDER BY Title");

    foreach (var artist in existingArtists)
    {
        artists.Add(artist.Name, artist.ArtistID);
    }

    foreach (var album in existingAlbums)
    {
        albums.Add(album.Key, album.AlbumID);
    }

    foreach (var track in existingTracks)
    {
        tracks.Add(track.Key, track.TrackID);
    }

    foreach (var track in existingTrackUrls)
    {
        trackUrls.Add(track.Url, track.TrackID);
    }

    // trackUrls.Dump();

    // Yes, it's horrible, but none of the many, many .NET variants of URLEncode actually seem to do it properly (still, in 2023)
    string urlEncode(string s) =>
        s.Replace(@"\", "/")
         .Replace(" ", "%20")
         .Replace("'", "%27")
         .Replace("(", "%28")
         .Replace(")", "%29")
         .Replace(",", "%2C")
         .Replace("&", "%26")
         .Replace("&", "%26")
         .Replace("[", "%5B")
         .Replace("]", "%5D")
         .Replace("#", "%23")
         .Replace("+", "%2B")
         .Replace("=", "%3D")
         .Replace("$", "%24");
         
    string normalise(string s) =>
        !string.IsNullOrWhiteSpace(s)
            ? s.Trim()
            : null;

    var progress = new DumpContainer().Dump("Processing");

    var files = await BackblazeClient
        // .ListAllFiles()
        .ListAllFiles("Compilations/SXSW 2011")
        //.Skip(10000)
        //.Take(4000)
        .ToArrayAsync();
    
    // files.Dump();

    var data = new List<TrackData>(files.Length);
    var results = new List<string>();
    var errors = new List<ProcessingError>();

    foreach (var file in files)
    {
        var tmp = audioFilesPath + @"\" + Path.GetFileName(file);
        
        var path = "/" + urlEncode(file);
        var url = credentials.CDNUrl + path;

        progress.Content = $"FETCH {file}";
        progress.Refresh();

        if (trackUrls.ContainsKey(path))
        {
            results.Add($"SKIPPED {file}");

            continue;
        }

        try
        {
            await BackblazeClient.DownloadFile(url, tmp);

            // https://github.com/Zeugma440/atldotnet
            var t = new Track(tmp);

            File.Delete(tmp);

            var track = new TrackData {
                TrackNo = t.TrackNumber,
                AlbumArtist = normalise(t.AlbumArtist),
                Album = normalise(t.Album),
                Artist = normalise(t.Artist),
                Title = normalise(t.Title),
                Url = path
            };

            data.Add(track);
            
            results.Add($"PROCESSED ID3 DATA {file}");
        }
        catch (Exception ex)
        {
            var err = new ProcessingError {
                Url = url,
                TempFile = tmp,
                Exception = ex
            };
            
            errors.Add(err);
        }
    }

    var orderedData = data.OrderBy(s => s.Artist)
        .ThenBy(s => s.Album)
        .ThenBy(s => s.TrackNo)
        .ThenBy(s => s.Title);
        
    orderedData.Dump("All Tracks", toDataGrid: true);
        
    foreach (var track in orderedData)
    {
        var id = $"{track.Artist}/{track.Title}";

        progress.Content = $"STORE {id}";
        progress.Refresh();

        try
        {            
            var trackArtistID = artists.TryGetValue(track.Artist, out var taID) ? taID : default(int?);

            if (track.Artist is not null && !trackArtistID.HasValue)
            {
                // Create a new artist record
                trackArtistID = (int)conn.ExecuteScalar("INSERT INTO Artists (Name) VALUES (@Name); SELECT CAST(SCOPE_IDENTITY() AS INT);", new { Name = track.Artist });

                artists.Add(track.Artist, trackArtistID.Value);
            }

            // Assume the album artist is the same as the track artist, unless specified
            var albumArtistID = trackArtistID;

            if (track.AlbumArtist is not null)
            {
                albumArtistID = artists.TryGetValue(track.AlbumArtist, out var aaID) ? aaID : default(int?);

                if (!albumArtistID.HasValue)
                {
                    // Create a new artist record
                    albumArtistID = (int)conn.ExecuteScalar("INSERT INTO Artists (Name) VALUES (@Name); SELECT CAST(SCOPE_IDENTITY() AS INT);", new { Name = track.AlbumArtist });

                    artists.Add(track.AlbumArtist, albumArtistID.Value);
                }
            }

            var albumKey = albumArtistID + "~" + track.Album;

            var albumID = albums.TryGetValue(albumKey, out var abID) ? abID : default(int?);
            
            if (track.Album is not null && !albumID.HasValue)
            {
                var albumParameters = new { 
                    ArtistID = albumArtistID ?? trackArtistID, 
                    Title = track.Album 
                };
                
                // Create a new album record
                albumID = (int)conn.ExecuteScalar("INSERT INTO Albums (ArtistID, Title) VALUES (@ArtistID, @Title); SELECT CAST(SCOPE_IDENTITY() AS INT);", albumParameters);

                albums.Add(albumKey, albumID.Value);
            }

            var trackKey = trackArtistID + "~" + track.Title;

            var trackID = artists.TryGetValue(trackKey, out var tID) ? tID : default(int?);

            if (track.Title is not null && !trackID.HasValue && !tracks.ContainsKey(trackKey))
            {
                var trackParameters = new { 
                    albumID,
                    ArtistID = trackArtistID,
                    TrackNumber = track.TrackNo,
                    track.Title, 
                    track.Url 
                };
                
                // Create a new track record
                trackID = (int)conn.ExecuteScalar("INSERT INTO Tracks (AlbumID, ArtistID, TrackNumber, Title, Url) VALUES (@AlbumID, @ArtistID, @TrackNumber, @Title, @Url); SELECT CAST(SCOPE_IDENTITY() AS INT);", trackParameters);

                tracks.Add(trackKey, trackID.Value);
            }
            
            results.Add($"STORED RECORD {id}");
        } 
        catch (Exception ex)
        {
            var err = new ProcessingError {
                Url = track.Url,
                TempFile = "DB INSERT",
                Exception = ex
            };
            
            errors.Add(err);
        }
    }
    
    errors.Dump("Errors");
    results.Dump("Log", toDataGrid: true);
}

public static class BackblazeClient
{
    private static readonly HttpClient _httpClient = new HttpClient();

    private static readonly string[] _extensions = new[] {
        ".mp3",
        ".m4a",
        ".aac"
    };

    private static AmazonS3Client _s3;
    
    private static string _bucketName;
    
    public static void Initialise(S3Credentials credentials)
    {
        _s3 = new AmazonS3Client(
            credentials.AccessKey, 
            credentials.SecretKey, 
            new AmazonS3Config {
                ServiceURL = credentials.ServiceUrl,
                ForcePathStyle = true
            });
        
        _bucketName = credentials.BucketName;
    }

    public static async Task DownloadFile(string url, string path)
    {
        // url.Dump();
        
        var result = await _httpClient.GetAsync(url);
        
        var bytes = result.IsSuccessStatusCode ? await result.Content.ReadAsByteArrayAsync() : null;
        
        if (bytes is null || bytes.Length == 0)
        {
            throw new Exception($"No content (response code {result.StatusCode})");
        }

        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(path);
        }

        await File.WriteAllBytesAsync($"{path}", bytes);            
    }

    public static async IAsyncEnumerable<string> ListAllFiles(string prefix = null)
    {
        bool moreFiles = true;
        string continuationToken = null;

        while (moreFiles)
        {
            var request = new ListObjectsV2Request {
                BucketName = _bucketName,
                Prefix = prefix,
                ContinuationToken = continuationToken
            };

            var response = await _s3.ListObjectsV2Async(request);

            foreach (var file in response.S3Objects.Where(f => _extensions.Contains(Path.GetExtension(f.Key), StringComparer.OrdinalIgnoreCase)))
            {
                yield return file.Key;
            }

            continuationToken = response.NextContinuationToken;
            
            moreFiles = response.IsTruncated;
        }
    }
}

public class ProcessingError
{
    public string Url { get; set; }
    
    public string TempFile { get; set; }
    
    public Exception Exception { get; set; }
}

public class TrackData
{
    public int? TrackNo { get; set; }

    public string Album { get; set; }

    public string AlbumArtist { get; set; }

    public string Artist { get; set; }

    public string Title { get; set; }

    public string Url { get; set; }
}

public class S3Credentials
{
    public string BucketName { get; set; }
    
    public string BucketUrl { get; set; }
    
    public string ServiceUrl { get; set; }
    
    public string AccessKey { get; set; }
    
    public string SecretKey { get; set; }
    
    public string CDNUrl { get; set; }
}

public static string SerializeObject<T>(T value)
{
    var sb = new StringBuilder(256);
    var sw = new StringWriter(sb, CultureInfo.InvariantCulture);

    using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
    {
        jsonWriter.Formatting = Formatting.Indented;
        jsonWriter.IndentChar = ' ';
        jsonWriter.Indentation = 4;

        JsonSerializer.CreateDefault().Serialize(jsonWriter, value, typeof(T));
    }

    return sw.ToString();
}
