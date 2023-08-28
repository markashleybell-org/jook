module jook.Data

open Dapper
open jook.Domain
open System.Data

let inline orNull (s: string option) =
    match s with
    | Some(v) -> v
    | None -> null

let trackList (getConnection: unit -> IDbConnection) (title: string option) (artist: string option) (album: string option) (genre: string option) =
    use conn = getConnection ()

    let parameters = {|
        Title = title |> orNull
        Artist = artist |> orNull
        Album = album |> orNull
        Genre = genre |> orNull
    |}

    conn.Query<TrackSummary>("TrackSearch", parameters, commandType=CommandType.StoredProcedure)
