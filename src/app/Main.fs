module jook.Main

open Dapper
open Falco
open Falco.Routing
open Falco.HostBuilder
open jook.Data
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open System
open System.Data
open Microsoft.Data.SqlClient

let env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")

let config = configuration [||] {
    required_json "appsettings.json"
    optional_json $"appsettings.{env}.json"
}

let connectionString = config.GetValue<string>("Settings:ConnectionString")
let cdn = config.GetValue<string>("Settings:Cdn")

type ApplicationLogger = class end

let configureLogging (log : ILoggingBuilder) =
    log.ClearProviders() |> ignore
    log.AddConsole() |> ignore
    log

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
        logging configureLogging

        endpoints [
            get "/" (fun _ cf q -> 
                let title = q.TryGet ("title")
                let artist = q.TryGet ("artist")
                let genre = q.TryGet ("genre")
                
                let tracks = trackList cf title artist genre

                Response.ofJson tracks)
            get "/config" (fun logger _ _ -> 
                logger.LogInformation "TEST"
                Response.ofJson cdn)
        ]
    }
    0
