// SCPCB360Game.cs
// Temporary debug boot: loads one cooked 173 room model directly in front of the camera.

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SCPCB360.Engine;
using SCPCB360.Input;
using SCPCB360.GameLogic;

namespace SCPCB360
{
    public class SCPCB360Game : Game
    {
        private GraphicsDeviceManager _gdm;
        private SpriteBatch _sb;

        private int _camPivot;
        private int _cam;
        private int _playerEnt;

        private float _playerPitch = 0f;
        private float _playerYaw = 0f;
        private const float PitchLimit = 75f;
        private const float PlayerMoveSpeed = 3.0f;

        private bool _paused = false;

        public SCPCB360Game()
        {
            _gdm = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080,
                IsFullScreen = true,
                SynchronizeWithVerticalRetrace = true,
            };

            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);
        }

        protected override void Initialize()
        {
            B3D.Initialize(GraphicsDevice, Content);
            RenderSystem.Initialize(GraphicsDevice);
            XInputRouter.InitializeMouseLook(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            XInputRouter.Update();

            RenderSystem.AmbientColor = new Color(40, 40, 45);
            RenderSystem.FogEnabled = true;
            RenderSystem.FogColor = new Color(10, 10, 12);
            RenderSystem.FogStart = 0.5f;
            RenderSystem.FogEnd = 2000f; // Wide debug fog so a giant model does not vanish instantly.

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _sb = new SpriteBatch(GraphicsDevice);

            Console.WriteLine("SCPCB360 debug LoadContent started.");

            // Camera/player rig
            _playerEnt = B3D.CreatePivot();
            B3D.EntityType(_playerEnt, 1);
            B3D.EntityRadius(_playerEnt, 0.4f);
            B3D.PositionEntity(_playerEnt, 0f, 0f, 0f);
            B3D.RotateEntity(_playerEnt, 0f, 0f, 0f);

            _camPivot = B3D.CreatePivot(_playerEnt);
            B3D.PositionEntity(_camPivot, 0f, 1.7f, 0f);

            _cam = B3D.CreateCamera(_camPivot);
            B3D.RotateEntity(_cam, 0f, 0f, 0f);

            // Load test mesh
            int room = B3D.LoadMesh("173");
            Console.WriteLine("Room handle = " + room);

            if (room == -1)
                throw new Exception("B3D.LoadMesh returned -1 for 173");

            var roomEntity = B3D.Get(room);
            if (roomEntity == null)
                throw new Exception("B3D.Get(room) returned null");

            if (roomEntity.XnaModel == null)
                throw new Exception("173 loaded an entity, but XnaModel is null");

            Console.WriteLine("Room model loaded: " + roomEntity.Name);

            // Position and scale room
            B3D.PositionEntity(room, 0f, 0f, -20f);
            B3D.ScaleEntity(room, 0.01f, 0.01f, 0.01f);

            // Manually load collision mesh from RMESH file (test)
            string rmeshPath = FindRMeshPath("173_opt.rmesh") ?? FindRMeshPath("173.rmesh");

            if (rmeshPath != null)
            {
                var renderMesh = Engine.RMeshReader.LoadRenderMesh(rmeshPath);
                if (renderMesh != null)
                {
                    roomEntity.RMeshRenderMesh = renderMesh;
                    Console.WriteLine("Loaded render mesh: " + renderMesh.Surfaces.Count + " textured surfaces");

                    var visibleCollisionMesh = Engine.RMeshReader.BuildVisibleCollisionMesh(renderMesh);
                    if (visibleCollisionMesh != null)
                    {
                        Console.WriteLine("Loaded visible collision mesh: " + visibleCollisionMesh.TriangleCount + " triangles");
                        roomEntity.CollisionMesh = visibleCollisionMesh;
                        B3D.EntityType(room, 2);
                    }
                    else
                        Console.WriteLine("Failed to build visible collision mesh");
                }
                else
                    Console.WriteLine("Failed to load render mesh");

                var hiddenCollisionMesh = Engine.RMeshReader.LoadCollisionMesh(rmeshPath);
                Console.WriteLine(hiddenCollisionMesh != null
                    ? "Hidden RMESH collision ignored for now: " + hiddenCollisionMesh.TriangleCount + " triangles"
                    : "Hidden RMESH collision not available");
            }
            else
                Console.WriteLine("RMESH file not found for 173 room");

            B3D.Collisions(1, 2, 2, 2);
        }

        protected override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            XInputRouter.Update();
            XInputRouter.UpdateRumble(delta);

            if (XInputRouter.IsPressed(CBAction.PauseMenu))
                _paused = !_paused;

            if (_paused)
            {
                base.Update(gameTime);
                return;
            }

            var look = XInputRouter.GetLookDelta();
            _playerYaw += look.X;
            _playerPitch = MathHelper.Clamp(_playerPitch - look.Y, -PitchLimit, PitchLimit);

            B3D.RotateEntity(_playerEnt, 0f, _playerYaw, 0f);
            B3D.RotateEntity(_cam, _playerPitch, 0f, 0f);

            float speed = XInputRouter.GetMoveSpeed(PlayerMoveSpeed) * delta;
            float fwd = XInputRouter.GetForwardAxis();
            float strafe = XInputRouter.GetStrafeAxis();

            // Convert local input (forward/strafe) into world-space displacement using player yaw.
            if (fwd != 0f || strafe != 0f)
            {
                var playerWorld = B3D.Get(_playerEnt).GetWorldMatrix();
                var forward = Microsoft.Xna.Framework.Vector3.TransformNormal(Microsoft.Xna.Framework.Vector3.Forward, playerWorld);
                var right = Microsoft.Xna.Framework.Vector3.TransformNormal(Microsoft.Xna.Framework.Vector3.Right, playerWorld);
                forward.Y = 0f;
                right.Y = 0f;
                if (forward.LengthSquared() > 0.0001f) forward.Normalize();
                if (right.LengthSquared() > 0.0001f) right.Normalize();

                var moveVec = forward * fwd + right * strafe;
                if (moveVec.LengthSquared() > 1f) moveVec.Normalize();
                moveVec *= speed;

                // Apply movement in world space by setting global position.
                var currentPos = B3D.Get(_playerEnt).GetWorldPosition();
                B3D.PositionEntity(_playerEnt, currentPos.X + moveVec.X, currentPos.Y + moveVec.Y, currentPos.Z + moveVec.Z, true);
            }

            if (XInputRouter.IsHeld(CBAction.Crouch))
                B3D.PositionEntity(_camPivot, 0f, 1.0f, 0f);
            else
                B3D.PositionEntity(_camPivot, 0f, 1.7f, 0f);

            PhysicsSystem.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(10, 10, 12));
            RenderSystem.Draw();
            base.Draw(gameTime);
        }

        private static string FindRMeshPath(string fileName)
        {
            string[] roots =
            {
                AppDomain.CurrentDomain.BaseDirectory,
                Environment.CurrentDirectory
            };

            foreach (string root in roots)
            {
                var dir = new System.IO.DirectoryInfo(root);
                while (dir != null)
                {
                    string candidate = System.IO.Path.Combine(dir.FullName, "GFX", "map", fileName);
                    if (System.IO.File.Exists(candidate))
                        return candidate;
                    dir = dir.Parent;
                }
            }

            return null;
        }
    }
}
