<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>z440.atl.core</NuGetReference>
  <Namespace>ATL</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
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
            id = Guid.NewGuid(),
            // Yes, it's horrible, but none of the many, many .NET variants of URLEncode actually seem to do it properly (still, in 2023)
            url = f.Replace(audioFilesPath, string.Empty).Replace(@"\", "/").Replace(" ", "%20").Replace("'", "%27").Replace("(", "%28").Replace(")", "%29"),
            artist = track.Artist,
            album = track.Album,
            title = track.Title
        };
    });

    var data = new {
        tracks = songs
    };

    var f = SerializeObject(data).Dump();
    
    File.WriteAllText(Path.Combine(queryPath, @"..\src\data.json"), f);
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
