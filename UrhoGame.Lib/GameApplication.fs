namespace UrhoGame.Lib

open Urho.Resources
open Urho.Gui
open Urho
open Urho.Urho2D
open Urho.Physics


[<AutoOpen>]
module GameKit =
    let log = System.Diagnostics.Debug.WriteLine
    
    module Sprite = 
        let createChild path (scene:Scene) (cache:ResourceCache)  = 
            let node = scene.CreateChild("StaticSprite2D/" + path)
            let sprite = node.CreateComponent<StaticSprite2D>()
            sprite.Sprite <- cache.GetSprite2D path

            node
        
        let setCollisionShape (node:Node) = 
            let body = node.CreateComponent<RigidBody>()
            body.Mass <- 1.f
            body.Kinematic <- true
            let collShape = node.CreateComponent<CollisionShape> ();
            collShape.SetSphere (0.3f,Vector3.Zero,Quaternion.Identity);
            node

        let inline onCollision f (node:Node) = 
            let subscription = node.SubscribeToNodeCollisionStart <| System.Action<_> f
            { new System.IDisposable with member __.Dispose() = subscription.Unsubscribe() }

        let setPosition x y z (node:Node) = 
            node.Position <- Vector3(x,y,z)
            node

        let movePosition (pos:IntVector2) (node:Node) = 
            let currPos = node.Position
            node.Position <- Vector3(currPos.X + (float32 pos.X)/100.f,
                                     currPos.Y - (float32 pos.Y)/100.f,
                                     currPos.Z)
            node

type GameApplication() = 
    inherit Application(ApplicationOptions("Data"))
    let mutable ball = null
    let rnd = new System.Random()
    let mutable textElement : Text = null

    member __.next(maxValue) = rnd.NextDouble() * 2.0 * maxValue - maxValue |> float32

    override self.Start() =
        let scene = new Scene()
        scene.CreateComponent<DebugRenderer>() |> ignore
        scene.CreateComponent<Octree>() |> ignore
        let physicsWorld = scene.CreateComponent<PhysicsWorld2D>()
        physicsWorld.Gravity <- Vector2.Zero
        let camera = scene.CreateChild("Camera")
        camera.Position <- new Vector3(0.0f, 0.0f, -10.0f)

        let camera = camera.CreateComponent<Camera>()
        camera.Orthographic <- true
        camera.OrthoSize <- float32 self.Graphics.Height * 0.01f;

        self.Renderer.SetViewport(0u, new Viewport(self.Context, scene, camera.GetComponent<Camera>(), null));

        let node = scene.CreateChild("StaticSprite2D")
        node.Position <- Vector3(0.0f, -3.0f, 0.0f)

        let sprite = node.CreateComponent<StaticSprite2D>()
        for i=0 to 100 do
            Sprite.createChild "Urho2D/Ball.png" scene self.ResourceCache 
            |> Sprite.setPosition (self.next 3.) (self.next 3.) 0.f
            |> Sprite.setCollisionShape
            |> ignore

        ball<- Sprite.createChild "Urho2D/Ball.png" scene self.ResourceCache 
               |> Sprite.setPosition 0.f -3.f 0.f
               |> Sprite.setCollisionShape
        ball |> Sprite.onCollision (fun args -> sprintf "%i vs %i" args.OtherNode.ID args.Body.Node.ID |> log
                                                args.OtherNode.Remove()) |> ignore


        let text = new Text(Value = "text", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center)
        text.SetFont(self.ResourceCache.GetFont("Fonts/Anonymous Pro.ttf"), 15) |> ignore
        self.UI.Root.AddChild text
        textElement <- text

    override self.OnUpdate(timeStep) = 
        textElement.Value <- sprintf "FPS %f" (1.f/timeStep)

        ball |> Sprite.movePosition self.Input.MouseMove |> ignore
