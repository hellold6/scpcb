// SCPCB360Game.cs
// Main XNA Game class. Wires together all engine systems in the correct order.
// This is the entry point that replaces CB's Main.bb.
//
// Original Main.bb initialisation sequence (simplified):
//   Graphics 800, 600
//   SetBuffer BackBuffer()
//   AmbientLight 40, 40, 45
//   cam = CreateCamera()
//   FogMode 1 : FogColor 10,10,12 : FogRange 0.001, 0.04
//   ... create player, load first rooms, start game loop ...

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SCPCB360.Engine;
using SCPCB360.GameLogic;
using SCPCB360.Input;

namespace SCPCB360
{
    public class SCPCB360Game : Game
    {
        private GraphicsDeviceManager _gdm;
        private SpriteBatch           _sb;

        // ── Camera setup ──────────────────────────────────────────────────────────
        private int _camPivot;   // Parent pivot (positioned at player head height)
        private int _cam;        // Camera entity (child of pivot, handles pitch)

        // ── Player state ──────────────────────────────────────────────────────────
        private int   _playerEnt;
        private float _playerPitch = 0f;
        private float _playerYaw   = 0f;
        private const float PitchLimit = 75f;
        private const float PlayerMoveSpeed = 0.05f;    // units/tick at 60fps

        // ── Game state ────────────────────────────────────────────────────────────
        private bool _paused = false;
        private int  _mapSeed = 42;    // Replace with title-screen seed input

        public SCPCB360Game()
        {
            _gdm = new GraphicsDeviceManager(this)
            {
                // Xbox 360 native resolution — Xenos outputs 720p natively
                PreferredBackBufferWidth  = 1280,
                PreferredBackBufferHeight = 720,
                IsFullScreen              = true,
                // SynchronizeWithVerticalRetrace — CB targets 30fps; we target 60
                SynchronizeWithVerticalRetrace = true,
            };

            Content.RootDirectory = "Content";
            IsFixedTimeStep        = true;
            TargetElapsedTime      = System.TimeSpan.FromSeconds(1.0 / 60.0);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Initialize — mirrors Blitz3D's Graphics() + global setup calls
        // ─────────────────────────────────────────────────────────────────────────

        protected override void Initialize()
        {
            // Initialise static engine systems
            B3D.Initialize(GraphicsDevice, Content);
            RenderSystem.Initialize(GraphicsDevice);
            XInputRouter.Update(); // warm up state so first-frame delta is zero

            // Replicate AmbientLight 40, 40, 45
            RenderSystem.AmbientColor = new Color(40, 40, 45);

            // FogMode 1, FogColor 10,10,12, FogRange 0.001, 0.04
            // Note: Blitz3D FogRange is in clip-space units; XNA uses world units.
            // CB's range [0.001, 0.04] at a 500-unit far clip ≈ world [0.5, 20] metres.
            RenderSystem.FogEnabled = true;
            RenderSystem.FogColor   = new Color(10, 10, 12);
            RenderSystem.FogStart   = 0.5f;
            RenderSystem.FogEnd     = 20f;

            base.Initialize();
        }

        // ─────────────────────────────────────────────────────────────────────────
        // LoadContent — pre-cache all assets into the 512 MB unified RAM
        // ─────────────────────────────────────────────────────────────────────────

        protected override void LoadContent()
        {
            _sb = new SpriteBatch(GraphicsDevice);

            // ── Player camera rig ──────────────────────────────────────────────────
            // CB structure: camPivot (player position) → cam (child, pitched separately)
            _camPivot = B3D.CreatePivot();
            B3D.PositionEntity(_camPivot, 0, 1.7f, 0); // eye height

            _cam = B3D.CreateCamera(_camPivot);
            B3D.RotateEntity(_cam, 0, 0, 0);

            // ── Player collision entity ─────────────────────────────────────────────
            _playerEnt = B3D.CreatePivot();
            B3D.EntityType(_playerEnt, 1);   // sphere collision
            B3D.EntityRadius(_playerEnt, 0.4f);
            //B3D.PositionEntity(_playerEnt, 0, 0, 0);
            B3D.PositionEntity(_playerEnt, 0, 2, -10);
            B3D.RotateEntity(_playerEnt, 0, 0, 0);

            // Parent camPivot to playerEnt so camera follows player position
            B3D.EntityParent(_camPivot, _playerEnt);

            // ── Generate map ────────────────────────────────────────────────────────
            //MapSystem.GenerateMap(_mapSeed);
            // Test-load one room model
            int room = B3D.LoadMesh("173.fbx");
            System.Diagnostics.Debug.WriteLine("Room handle = " + room);

            if (room == -1)
                throw new System.Exception("LoadMesh returned -1");

            var roomEntity = B3D.Get(room);
            if (roomEntity == null)
                throw new System.Exception("Room entity missing after LoadMesh");

            if (roomEntity.XnaModel == null)
                throw new System.Exception("173.fbx was not loaded. Check Content.mgcb and Content/bin/DesktopGL/Content/173.xnb");

            B3D.PositionEntity(room, 0, 0, 0);
            B3D.ScaleEntity(room, 0.01f, 0.01f, 0.01f);


            // ── Spawn starting NPCs ─────────────────────────────────────────────────
            // Spawn 173 in its containment chamber (first heavy room)
            // Actual spawn pivot is provided by the room — this is placeholder
            int spawn173 = B3D.CreatePivot();
            B3D.PositionEntity(spawn173, 5, 0, 40);
            //NPCSystem.Spawn(NPCKind.SCP_173, spawn173);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Update — game loop (60 Hz)
        // ─────────────────────────────────────────────────────────────────────────

        protected override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // 1. Poll controller
            XInputRouter.Update();
            XInputRouter.UpdateRumble(delta);

            // Pause toggle
            if (XInputRouter.IsPressed(CBAction.PauseMenu))
                _paused = !_paused;

            if (_paused)
            {
                base.Update(gameTime);
                return;
            }

            // 2. Player look (right thumbstick → camera pitch + yaw)
            var look = XInputRouter.GetLookDelta();
            _playerYaw   += look.X;
            _playerPitch  = MathHelper.Clamp(_playerPitch + look.Y, -PitchLimit, PitchLimit);

            // Apply yaw to player entity (left-right rotation)
            B3D.RotateEntity(_playerEnt, 0, _playerYaw, 0);
            // Apply pitch to camera child (up-down look, CB keeps them separate)
            B3D.RotateEntity(_cam, _playerPitch, 0, 0);

            // 3. Player movement (left thumbstick → MoveEntity in player-local XZ)
            float speed = XInputRouter.GetMoveSpeed(PlayerMoveSpeed);
            float fwd   = XInputRouter.GetForwardAxis();
            float strafe= XInputRouter.GetStrafeAxis();

            if (fwd   != 0) B3D.MoveEntity(_playerEnt, 0, 0, fwd   * speed);
            if (strafe!= 0) B3D.MoveEntity(_playerEnt, strafe * speed, 0, 0);

            // 4. Crouch
            if (XInputRouter.IsHeld(CBAction.Crouch))
                B3D.PositionEntity(_camPivot, 0, 1.0f, 0); // lower eye height
            else
                B3D.PositionEntity(_camPivot, 0, 1.7f, 0);

            // 5. Physics resolution
            PhysicsSystem.Update();

            // 6. NPC AI
            NPCSystem.Update(delta, _playerEnt);

            base.Update(gameTime);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Draw
        // ─────────────────────────────────────────────────────────────────────────

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(10, 10, 12));

            RenderSystem.Draw();

            base.Draw(gameTime);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Cleanup
        // ─────────────────────────────────────────────────────────────────────────

        protected override void UnloadContent()
        {
            MapSystem.FreeAllRooms();
            base.UnloadContent();
        }
    }
}
