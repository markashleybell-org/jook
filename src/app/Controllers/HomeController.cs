using Microsoft.AspNetCore.Mvc;
using jook.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Dapper;

namespace jook.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly Settings _settings;

    public HomeController(
        ILogger<HomeController> logger,
        IOptions<Settings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    public IActionResult Index()
    {
        var viewModel = new IndexViewModel {
            CDN = _settings.CDN
        };

        return View(viewModel);
    }

    public IActionResult Tracks()
    {
        const string sql = @"
SELECT 
    aa.ArtistID AS AlbumArtistID,
    aa.Name AS AlbumArtist,
    ta.ArtistID,
    ta.Name AS Artist,
    a.AlbumID,
    a.Title AS Album,
    t.TrackID,
    t.Title AS Title,
    t.Url AS Url
FROM 
    Tracks t
INNER JOIN
    Albums a ON a.AlbumID = t.AlbumID
INNER JOIN
    Artists aa ON aa.ArtistID = a.ArtistID
LEFT JOIN
    Artists ta ON ta.ArtistID = t.ArtistID
ORDER BY
    aa.Name,
    ta.Name,
    a.Title,
    t.Title";

        using var conn = new SqlConnection(_settings.ConnectionString);

        var tracks = conn.Query<TrackSummary>(sql);

        return Json(new { tracks });
    }
}
