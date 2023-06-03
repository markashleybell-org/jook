module jook.Domain

[<CLIMutable>]
type Artist = {
    TrackID: int
    Name: string }

[<CLIMutable>]
type Album = { 
    AlbumID: int
    ArtistID: int
    Title: string }

[<CLIMutable>]
type Track = { 
    TrackID: int
    AlbumID: int
    Title: string
    Url: string }

[<CLIMutable>]
type TrackSummary = {
    AlbumArtistID: int
    AlbumArtist: string
    AlbumID: int
    Album: string
    ArtistID: int
    Artist: string
    TrackID: int
    TrackNumber: int option
    Title: string
    Url: string }
