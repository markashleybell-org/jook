USE jook
GO

-- SELECT * FROM Artists
-- SELECT * FROM Albums
-- SELECT * FROM Tracks

SELECT 
    aa.ArtistID AS AlbumArtistID,
    aa.Name AS AlbumArtist,
    a.AlbumID,
    a.Title AS Album,
    ta.ArtistID,
    ta.Name AS Artist,
    g.GenreID,
    g.Name AS Genre,
    t.TrackID,
    t.TrackNumber,
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
LEFT JOIN
    Genres g ON g.GenreID = t.GenreID
ORDER BY
    aa.Name,
    a.Title,
    t.TrackNumber,
    ta.Name,
    t.Title

