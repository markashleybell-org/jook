CREATE OR ALTER PROCEDURE TrackSearch
    @Title NVARCHAR(256),
    @Artist NVARCHAR(256),
    @Genre NVARCHAR(256)
AS
BEGIN
    SET @Title = NULLIF(ISNULL(@Title, ''), '')
    SET @Artist = NULLIF(ISNULL(@Artist, ''), '')
    SET @Genre = NULLIF(ISNULL(@Genre, ''), '')

    SELECT 
        aa.ArtistID AS AlbumArtistID,
        aa.Name AS AlbumArtist,
        a.AlbumID,
        a.Title AS Album,
        ta.ArtistID,
        ta.Name AS Artist,
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
    WHERE
        (@Title IS NULL OR t.Title LIKE '%' + @Title + '%')
    AND
        (@Artist IS NULL OR aa.Name LIKE '%' + @Artist + '%')
    AND
        (@Genre IS NULL OR g.Name LIKE '%' + @Genre + '%')
    ORDER BY
        aa.Name,
        a.Title,
        t.TrackNumber,
        ta.Name,
        t.Title
END
GO

-- EXEC TrackSearch @Title = NULL, @Artist = NULL, @Genre = NULL
