module jook.Main

open Falco
open Falco.Routing
open Falco.HostBuilder
open jook.Data
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open System
open System.Data
open Microsoft.Data.SqlClient
open System.Text.Json

let env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")

let config = configuration [||] {
    required_json "appsettings.json"
    optional_json $"appsettings.{env}.json"
}

let jsonOptions = JsonSerializerOptions()

jsonOptions.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase

let connectionString = config.GetValue<string>("Settings:ConnectionString")

type ApplicationLogger = class end

let getDatabaseConnection () : IDbConnection =
    new SqlConnection(connectionString) :> IDbConnection

let withServices getData handler : HttpHandler = fun ctx ->
    let logger = ctx.GetService<ILogger<ApplicationLogger>>()
    let data = getData ctx
    handler logger getDatabaseConnection data ctx

let get route handler = 
    get route (withServices Request.getQuery handler)

[<EntryPoint>]
let main args =
    webHost args {
        use_default_files
        use_static_files
        endpoints [
            get "/tracks" (fun _ cf q -> 
                let title = q.TryGet ("title")
                let artist = q.TryGet ("artist")
                let genre = q.TryGet ("genre")
                
                let data = {| tracks = trackList cf title artist genre |}

                Response.ofJsonOptions jsonOptions data)
        ]
    }
    0
