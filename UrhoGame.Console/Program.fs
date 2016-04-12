namespace UrhoGame.Mac
open System
open UrhoGame.Lib
open Urho.Desktop
open Urho.Gui

module main =
    [<EntryPoint>]
    let main args =
        use app = new GameApplication()
        app.Run()
    