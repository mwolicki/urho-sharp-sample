namespace UrhoHame.iOS

open System

open UIKit
open Urho.iOS
open Foundation

[<Register ("AppDelegate")>]
type AppDelegate () =
    inherit UIApplicationDelegate ()

    override val Window = null with get, set

    // This method is invoked when the application is ready to run.
    override this.FinishedLaunching (app, options) =
        let syncContext = System.Threading.SynchronizationContext.Current
        async{
         printfn "sdfsdgfjhgjshd"
         do! Async.SwitchToContext syncContext
         let app = new UrhoGame.Lib.GameApplication()
         app.Run() |> ignore } |> Async.Start |> ignore
        true
