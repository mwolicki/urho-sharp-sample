namespace UrhoHame.iOS

open System

open UIKit
open Urho.iOS
open Foundation

[<Register ("AppDelegate")>]
type AppDelegate () =
    inherit UIApplicationDelegate ()

    override this.FinishedLaunching (app, options) =
        let syncContext = System.Threading.SynchronizationContext.Current
        async{
             do! Async.SwitchToContext syncContext
             let app = new UrhoGame.Lib.GameApplication()
             app.Run() |> ignore } |> Async.Start
        true
