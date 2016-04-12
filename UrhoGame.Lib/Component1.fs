namespace UrhoGame.Lib

open Urho.Resources
open Urho.Gui
open Urho
open Urho.Urho2D

type GameApplication() = 
    inherit Application(ApplicationOptions("Data"))
    let mutable ball = null
    let rnd = new System.Random()

    member __.next(maxValue) =
        float32 <| rnd.NextDouble() * 2.0 * maxValue - maxValue

    override self.Start() =
        let scene = new Scene()
        scene.CreateComponent<DebugRenderer>() |> ignore
        scene.CreateComponent<Octree>() |> ignore
        let camera = scene.CreateChild("Camera")
        camera.Position <- new Vector3(0.0f, 0.0f, -10.0f)

        let camera = camera.CreateComponent<Camera>()
        camera.Orthographic <- true
        camera.OrthoSize <- float32 self.Graphics.Height * 0.01f;

        self.Renderer.SetViewport(0u, new Viewport(self.Context, scene, camera.GetComponent<Camera>(), null));

        let node = scene.CreateChild("StaticSprite2D")
        node.Position <- (new Vector3(0.0f, -3.0f, 0.0f));

        let sprite = node.CreateComponent<StaticSprite2D>()
        ball<-node
        let ball = self.ResourceCache.GetSprite2D("Urho2D/Ball.png")

        sprite.Sprite <- ball

    override self.OnUpdate(timeStep) = 
        ball.Position <- Vector3(self.next 3.0, self.next 3.0, 0.0f)
        