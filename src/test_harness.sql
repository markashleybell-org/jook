-- SELECT * FROM Artists
-- SELECT * FROM Albums
-- SELECT * FROM Tracks

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