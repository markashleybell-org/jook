USE master
GO

DECLARE @kill varchar(8000) = ''

SELECT 
    @kill = @kill + 'kill ' + CONVERT(varchar(5), session_id) + ';'  
FROM 
    sys.dm_exec_sessions
WHERE
    database_id  = db_id('jook')

EXEC(@kill)

GO

DROP DATABASE jook
GO

CREATE DATABASE jook
GO

USE jook
GO

CREATE TABLE [dbo].[Artists] (
    [ArtistID]      INT IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (256)      NOT NULL,
    CONSTRAINT [PK_Artists] PRIMARY KEY CLUSTERED ([ArtistID] ASC)
);

CREATE TABLE [dbo].[Albums] (
    [AlbumID]       INT IDENTITY (1, 1) NOT NULL,
    [ArtistID]      INT                 NOT NULL,
    [Title]         NVARCHAR (256)      NOT NULL,
    CONSTRAINT [PK_Albums] PRIMARY KEY CLUSTERED ([AlbumID] ASC),
    CONSTRAINT [FK_Albums_Artists] FOREIGN KEY ([ArtistID]) REFERENCES [dbo].[Artists] ([ArtistID])
);

CREATE TABLE [dbo].[Tracks] (
    [TrackID]       INT IDENTITY (1, 1) NOT NULL,
    [AlbumID]       INT                 NOT NULL,
    [ArtistID]      INT                 NOT NULL,
    [TrackNumber]   INT                 NULL,
    [Title]         NVARCHAR (256)      NOT NULL,
    [Url]           NVARCHAR (256)      NOT NULL,
    CONSTRAINT [PK_Tracks] PRIMARY KEY CLUSTERED ([TrackID] ASC),
    CONSTRAINT [FK_Tracks_Albums] FOREIGN KEY ([AlbumID]) REFERENCES [dbo].[Albums] ([AlbumID]),
    CONSTRAINT [FK_Tracks_Artists] FOREIGN KEY ([ArtistID]) REFERENCES [dbo].[Artists] ([ArtistID])
);

