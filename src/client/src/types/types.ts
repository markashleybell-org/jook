export interface Config {
    CDN: string
}

export interface TrackData {
    trackID: number
    trackNumber: number
    title: string
    url: string
    albumArtistID: number
    albumArtist: string
    albumID: number
    album: string
    artistID: number
    artist: string
}

export interface TrackListItem extends TrackData {
    index: number
}

export interface TrackSearchQuery {
    title: string | null
    artist: string | null
    album: string | null
    genre: string | null
}
