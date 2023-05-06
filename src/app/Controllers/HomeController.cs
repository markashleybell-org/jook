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
    ar.ArtistID,
    ar.Name AS ArtistName,
    ab.AlbumID,
    ab.Title AS AlbumTitle,
    t.TrackID,
    t.Title AS TrackTitle,
    t.Url AS Url
FROM 
    Tracks t
INNER JOIN
    Albums ab ON ab.AlbumID = t.AlbumID
INNER JOIN
    Artists ar ON ar.ArtistID = ab.ArtistID
ORDER BY
    ar.Name,
    ab.Title,
    t.Title";

        using var conn = new SqlConnection(_settings.ConnectionString);

        var tracks = conn.Query<TrackSummary>(sql);

        return Json(new { tracks });
    }
}
