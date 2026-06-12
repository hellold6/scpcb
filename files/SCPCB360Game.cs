// SCPCB360Game.cs — main game loop orchestrating all ported CB systems

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        private Texture2D _whitePixel;

        private float _fps;
        private float _fpsTimer;
        private int _fpsFrames;

        private static readonly Dictionary<char, string[]> TechFont = new()
        {
            [' '] = new[] { "000", "000", "000", "000", "000", "000", "000" },
            [':'] = new[] { "0", "1", "1", "0", "1", "1", "0" },
            ['.'] = new[] { "0", "0", "0", "0", "0", "1", "1" },
            ['-'] = new[] { "000", "000", "000", "111", "000", "000", "000" },
            ['0'] = new[] { "111", "101", "101", "101", "101", "101", "111" },
            ['1'] = new[] { "010", "110", "010", "010", "010", "010", "111" },
            ['2'] = new[] { "111", "001", "001", "111", "100", "100", "111" },
            ['3'] = new[] { "111", "001", "001", "111", "001", "001", "111" },
            ['4'] = new[] { "101", "101", "101", "111", "001", "001", "001" },
            ['5'] = new[] { "111", "100", "100", "111", "001", "001", "111" },
            ['6'] = new[] { "111", "100", "100", "111", "101", "101", "111" },
            ['7'] = new[] { "111", "001", "001", "010", "010", "010", "010" },
            ['8'] = new[] { "111", "101", "101", "111", "101", "101", "111" },
            ['9'] = new[] { "111", "101", "101", "111", "001", "001", "111" },
            ['A'] = new[] { "111", "101", "101", "111", "101", "101", "101" },
            ['B'] = new[] { "110", "101", "101", "110", "101", "101", "110" },
            ['C'] = new[] { "111", "100", "100", "100", "100", "100", "111" },
            ['D'] = new[] { "110", "101", "101", "101", "101", "101", "110" },
            ['E'] = new[] { "111", "100", "100", "111", "100", "100", "111" },
            ['F'] = new[] { "111", "100", "100", "111", "100", "100", "100" },
            ['H'] = new[] { "101", "101", "101", "111", "101", "101", "101" },
            ['M'] = new[] { "101", "111", "111", "101", "101", "101", "101" },
            ['N'] = new[] { "101", "111", "111", "111", "101", "101", "101" },
            ['O'] = new[] { "111", "101", "101", "101", "101", "101", "111" },
            ['P'] = new[] { "111", "101", "101", "111", "100", "100", "100" },
            ['S'] = new[] { "111", "100", "100", "111", "001", "001", "111" },
            ['T'] = new[] { "111", "010", "010", "010", "010", "010", "010" },
            ['U'] = new[] { "101", "101", "101", "101", "101", "101", "111" },
            ['W'] = new[] { "101", "101", "101", "101", "111", "111", "101" },
            ['X'] = new[] { "101", "101", "101", "010", "101", "101", "101" },
            ['Y'] = new[] { "101", "101", "101", "010", "010", "010", "010" },
            ['Z'] = new[] { "111", "001", "001", "010", "100", "100", "111" },
        };

        public SCPCB360Game()
        {
            _gdm = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
                IsFullScreen = false,
                SynchronizeWithVerticalRetrace = true,
            };

            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);
        }

        protected override void Initialize()
        {
            DifficultySystem.Initialize();
            AchievementSystem.Initialize();
            MenuSystem.Initialize();
            SaveSystem.LoadSaveGames();

            B3D.Initialize(GraphicsDevice, Content);
            RenderSystem.Initialize(GraphicsDevice);
            XInputRouter.InitializeMouseLook(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            RenderSystem.AmbientColor = new Color(40, 40, 45);
            RenderSystem.FogEnabled = true;
            RenderSystem.FogColor = new Color(10, 10, 12);
            RenderSystem.FogStart = 0.5f;
            RenderSystem.FogEnd = 40f;

            GameState.ShowFps = IniConfig.GetInt(GameState.OptionFile, "options", "show FPS", 0) != 0;
            GameState.SfxVolume = IniConfig.GetFloat(GameState.OptionFile, "audio", "sound volume", 1f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _sb = new SpriteBatch(GraphicsDevice);
            _whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            _whitePixel.SetData(new[] { Color.White });

            TextRenderer.Initialize(GraphicsDevice);
            GuiSystem.Initialize(GraphicsDevice);
            BlurFilter.Initialize(GraphicsDevice);
            PortalRenderer.Initialize(GraphicsDevice);

            AudioSystem.Initialize(Content);
            MapAssets.Initialize();
            DoorSystem.Initialize();
            SecurityCamSystem.Initialize();
            SkyboxSystem.Initialize();

            int collider = B3D.CreatePivot();
            B3D.EntityType(collider, 1);
            B3D.EntityRadius(collider, 0.4f);

            int camPivot = B3D.CreatePivot(collider);
            B3D.PositionEntity(camPivot, 0f, 1.7f, 0f);

            int cam = B3D.CreateCamera(camPivot);
            int head = B3D.CreatePivot(collider);
            B3D.PositionEntity(head, 0f, 1.7f, 0f);

            PlayerSystem.Initialize(collider, head, camPivot, cam);
            B3D.Collisions(1, 2, 2, 2);
        }

        protected override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GameState.FpsFactor = delta * 60f;
            GameState.FpsFactor2 = GameState.FpsFactor;

            XInputRouter.Update();
            XInputRouter.UpdateRumble(delta);

            switch (GameState.Screen)
            {
                case GameScreen.MainMenu:
                    MenuSystem.Update();
                    break;

                case GameScreen.Loading:
                    if (MenuSystem.LoadingProgress >= 1f)
                        GameState.Screen = GameScreen.Playing;
                    break;

                case GameScreen.Playing:
                    UpdatePlaying(delta);
                    break;

                case GameScreen.Paused:
                    if (XInputRouter.IsPressed(CBAction.PauseMenu))
                        GameState.Screen = GameScreen.Playing;
                    break;

                case GameScreen.Dead:
                    if (XInputRouter.IsPressed(CBAction.Interact))
                        GameState.Screen = GameScreen.MainMenu;
                    break;
            }

            if (XInputRouter.IsPressed(CBAction.PauseMenu) && GameState.Screen == GameScreen.Playing)
                GameState.Screen = GameScreen.Paused;

            AudioSystem.UpdateMusic();

            UpdateFps(delta);
            base.Update(gameTime);
        }

        private void UpdatePlaying(float delta)
        {
            PlayerSystem.MouseLook();
            PlayerSystem.MovePlayer();

            GameState.Crouch = XInputRouter.IsHeld(CBAction.Crouch);

            if (XInputRouter.IsPressed(CBAction.Inventory))
            {
                GameState.InvOpen = !GameState.InvOpen;
                if (GameState.InvOpen && GameState.InvHoverSlot == 66)
                    GameState.InvHoverSlot = 0;
            }

            if (XInputRouter.IsPressed(CBAction.DropItem))
            {
                var drop = ItemSystem.SelectedItem ?? ItemSystem.GetHoveredInventoryItem();
                if (drop != null)
                    ItemSystem.DropItem(drop);
            }

            if (XInputRouter.IsPressed(CBAction.Interact) && !EventSystem.HandleIntroInteract())
                HandleInteract();

            void HandleInteract()
            {
                if (GameState.InvOpen)
                {
                    var slotItem = ItemSystem.GetHoveredInventoryItem();
                    if (slotItem != null)
                        ItemUseSystem.UseInventorySlot(GameState.InvHoverSlot);
                    return;
                }

                if (ItemSystem.SelectedItem != null)
                {
                    if (GameState.ClosestDoor != null)
                        DoorSystem.UseDoor(GameState.ClosestDoor);
                    else
                        ItemUseSystem.UseSelectedItem();
                    return;
                }

                if (GameState.ClosestDoor != null)
                {
                    DoorSystem.UseDoor(GameState.ClosestDoor);
                    return;
                }

                if (ItemSystem.ClosestItem != null)
                    ItemSystem.PickupClosest();
            }

            DoorSystem.Update();
            ItemSystem.Update(GameState.Collider);
            NPCSystem.Update(delta, GameState.Collider);
            EventSystem.Update();
            ParticleSystem.Update();
            DevilParticleSystem.UpdateParticlesDevil();
            SkyboxSystem.Update(GameState.Camera);
            AudioSystem.UpdateLoops(GameState.Camera);
            MapSystem.UpdateRooms();
            DecalSystem.Update();
            SecurityCamSystem.Update();
            ItemUseSystem.UpdateWearProgress();

            PortalRenderer.UpdateAll();
            BlurFilter.Update(delta);
            GameState.BlurVolume = BlurFilter.BlurVolume;

            if (GameState.MsgTimer > 0f)
                GameState.MsgTimer -= GameState.FpsFactor;
        }

        protected override void Draw(GameTime gameTime)
        {
            if (GameState.Screen == GameScreen.Playing ||
                GameState.Screen == GameScreen.Paused ||
                GameState.Screen == GameScreen.Dead)
            {
                RenderSystem.Draw(captureScene: true);

                _sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp);
                var scene = RenderSystem.SceneTexture;
                if (scene != null)
                    _sb.Draw(scene, GraphicsDevice.Viewport.Bounds, Color.White);
                else
                    _sb.Draw(_whitePixel, GraphicsDevice.Viewport.Bounds, new Color(10, 10, 12));
                _sb.End();

                BlurFilter.Draw(GameState.BlurVolume, RenderSystem.SceneTexture);
            }
            else
            {
                GraphicsDevice.Clear(new Color(10, 10, 12));
            }

            DrawOverlay();
            base.Draw(gameTime);
        }

        private void DrawOverlay()
        {
            if (_whitePixel == null || _sb == null) return;

            _sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            switch (GameState.Screen)
            {
                case GameScreen.MainMenu:
                    DrawMainMenu();
                    break;
                case GameScreen.Loading:
                    DrawText("LOADING...", 18, 18, 4, new Color(220, 235, 255));
                    DrawText(((int)(MenuSystem.LoadingProgress * 100f)).ToString() + "%", 18, 54, 3, Color.White);
                    break;
                case GameScreen.Playing:
                case GameScreen.Paused:
                case GameScreen.Dead:
                    if (GameState.Screen == GameScreen.Paused)
                        DrawText("PAUSED", 18, 18, 4, Color.White);
                    GuiSystem.Draw(_sb);
                    break;
            }

            if (GameState.ShowFps)
                DrawText("FPS: " + _fps.ToString("0.0"), 18, GraphicsDevice.Viewport.Height - 30, 2, new Color(170, 220, 170));

            _sb.End();
        }

        private void DrawMainMenu()
        {
            DrawText("SCP: CONTAINMENT BREACH", 18, 18, 3, new Color(200, 30, 30));
            DrawText("XBOX 360 PORT", 18, 50, 2, new Color(180, 180, 180));
            DrawText(MenuSystem.MenuStr, 18, 80, 2, new Color(80, 80, 80));

            int y = 110;
            foreach (var line in MenuSystem.GetVisibleLines())
            {
                var color = line.StartsWith('>') ? new Color(255, 220, 100) : new Color(200, 200, 200);
                if (line.Contains("INCOMPATIBLE")) color = Color.Red;
                DrawText(line, 30, y, 2, color);
                y += 22;
            }
        }

        private void UpdateFps(float delta)
        {
            _fpsTimer += delta;
            _fpsFrames++;
            if (_fpsTimer >= 0.25f)
            {
                _fps = _fpsFrames / _fpsTimer;
                _fpsFrames = 0;
                _fpsTimer = 0f;
            }
        }

        private void DrawText(string text, int x, int y, int scale, Color color)
        {
            int cursor = x;
            foreach (char raw in text.ToUpperInvariant())
            {
                if (!TechFont.TryGetValue(raw, out var glyph))
                    glyph = TechFont[' '];

                for (int row = 0; row < glyph.Length; row++)
                {
                    for (int col = 0; col < glyph[row].Length; col++)
                    {
                        if (glyph[row][col] == '1')
                            _sb.Draw(_whitePixel, new Rectangle(cursor + col * scale, y + row * scale, scale, scale), color);
                    }
                }

                cursor += (glyph[0].Length + 1) * scale;
            }
        }
    }
}