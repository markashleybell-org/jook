<Query Kind="Program">
  <NuGetReference>AWSSDK.S3</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Amazon.S3</Namespace>
  <Namespace>Amazon.S3.Model</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Encodings.Web</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <RemoveNamespace>System.Xml</RemoveNamespace>
  <RemoveNamespace>System.Xml.Linq</RemoveNamespace>
  <RemoveNamespace>System.Xml.XPath</RemoveNamespace>
</Query>

async Task Main()
{    
    var queryPath = Path.GetDirectoryName(Util.CurrentQueryPath);
    
    var credentials = JsonConvert.DeserializeObject<S3Credentials>(File.ReadAllText(queryPath + @"\config.json")).Dump();
    
    BackblazeClient.Initialise(credentials);
    
    await foreach (var file in BackblazeClient.ListAllFiles())
    {
        file.Dump();
    }
}

public static class BackblazeClient
{
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

            foreach (S3Object file in response.S3Objects)
            {
                yield return file.Key;
            }

            continuationToken = response.NextContinuationToken;
            
            moreFiles = response.IsTruncated;
        }
    }
}

public class S3Credentials
{
    public string BucketName { get; set; }
    
    public string BucketUrl { get; set; }
    
    public string ServiceUrl { get; set; }
    
    public string AccessKey { get; set; }
    
    public string SecretKey { get; set; }
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
