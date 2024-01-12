module jook.Main

open Falco
open Falco.Routing
open Falco.HostBuilder
open jook.Data
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open System
open System.Linq
open System.Data
open Microsoft.Data.SqlClient
open System.Text.Json
open Microsoft.AspNetCore.Cors.Infrastructure

let env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")

let config = configuration [||] {
    required_json "appsettings.json"
    optional_json $"appsettings.{env}.json"
}

let corsPolicyName = "local"

let corsPolicy (policyBuilder: CorsPolicyBuilder) =
    // Note: This is a very lax setting, but a good fit for local development
    policyBuilder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        // Note: The URLs must not end with a /
        .WithOrigins("https://localhost:5001",
                     "http://localhost:5173")
    |> ignore

let corsOptions (options : CorsOptions) =
    options.AddPolicy(corsPolicyName, corsPolicy)

let jsonOptions = JsonSerializerOptions()

jsonOptions.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase

let connectionString = config.GetValue<string>("Settings:ConnectionString")

type ApplicationLogger = class end

let getDatabaseConnection () : IDbConnection =
    new SqlConnection(connectionString) :> IDbConnection

let withServicesGet handler : HttpHandler = fun ctx ->
    let logger = ctx.GetService<ILogger<ApplicationLogger>>()
    let data = Request.getQuery ctx
    handler logger getDatabaseConnection data ctx

let withServicesPost handler : HttpHandler = fun ctx ->
    let logger = ctx.GetService<ILogger<ApplicationLogger>>()
    task {
        let! body = Request.getJsonOptions jsonOptions ctx
        return handler logger getDatabaseConnection body ctx
    }

let get route handler = 
    get route (withServicesGet handler)

let post route handler = 
    post route (withServicesPost handler)

[<EntryPoint>]
let main args =
    webHost args {
        use_default_files
        use_static_files
        use_cors corsPolicyName corsOptions
        endpoints [
            get "/tracks" (fun _ cf q -> 
                let title = q.TryGet ("title")
                let artist = q.TryGet ("artist")
                let album = q.TryGet ("album")
                let genre = q.TryGet ("genre")

                let tracks = trackList cf title artist album genre

                let meta = {|
                    count = tracks.Count()
                |}

                let data = {| 
                    meta = meta
                    tracks = tracks |}

                Response.ofJsonOptions jsonOptions data)
            post "/playlist" (fun _ cf body -> 
                // TODO: Persist the playlist tracks and title etc
                Response.ofJsonOptions jsonOptions body)
        ]
    }
    0
