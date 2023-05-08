-- SELECT * FROM Artists
-- SELECT * FROM Albums
-- SELECT * FROM Tracks

/*
SELECT 
    ar.Name,
    ab.Title,
    t.Title,
    t.Url
FROM 
    Tracks t
INNER JOIN
    Albums ab ON ab.AlbumID = t.AlbumID
INNER JOIN
    Artists ar ON ar.ArtistID = ab.ArtistID
ORDER BY
    ar.Name,
    ab.Title,
    t.Title
*/

SELECT 
    aa.ArtistID AS AlbumArtistID,
    aa.Name AS AlbumArtist,
    ta.ArtistID,
    ta.Name AS Artist,
    a.AlbumID,
    a.Title AS Album,
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
ORDER BY
    aa.Name,
    ta.Name,
    a.Title,
    t.TrackNumber,
    t.Title