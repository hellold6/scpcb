// RenderSystem.cs
// Draws all visible BlitzEntity objects each frame using XNA's BasicEffect pipeline.
// The Xbox 360's Xenos GPU is a DirectX 9-class part; BasicEffect maps cleanly.
//
// Key design decisions for Xbox 360:
//   • Batch draw calls by material to minimize state changes on the Xenos GPU.
//   • Use the 10 MB eDRAM as the render target — never read back from it mid-frame.
//   • Models are pre-baked into XNB with vertex buffers already PowerPC-aligned;
//     no runtime byte-swapping occurs here.
//   • Alpha-blended entities are depth-sorted and drawn after opaques (painter's algorithm).

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using SCPCB360.Engine;

namespace SCPCB360.Engine
{
    public static class RenderSystem
    {
        private static GraphicsDevice _gfx;
        private static BasicEffect    _effect;

        // Ambient + fog parameters (CB uses heavy fog to mask render distance)
        public static Color  AmbientColor    = new Color(40, 40, 45);
        public static Color  FogColor        = new Color(10, 10, 12);
        public static float  FogStart        = 8f;
        public static float  FogEnd          = 40f;
        public static bool   FogEnabled      = true;

        // Camera matrices (set once per frame from the active camera entity)
        private static Matrix _view;
        private static Matrix _projection;

        // Sorted draw lists (rebuilt each frame)
        private static readonly List<BlitzEntity> _opaques = new(256);
        private static readonly List<BlitzEntity> _alphas  = new(64);

        public static void Initialize(GraphicsDevice gfx)
        {
            _gfx = gfx;
            _effect = new BasicEffect(gfx)
            {
                TextureEnabled      = true,
                VertexColorEnabled  = true,
                LightingEnabled     = false, // CB uses pre-baked vertex lighting
                FogEnabled          = FogEnabled,
                FogColor            = FogColor.ToVector3(),
                FogStart            = FogStart,
                FogEnd              = FogEnd,
            };
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Per-frame draw
        // ─────────────────────────────────────────────────────────────────────────

        public static void Draw()
        {
            // 1. Build camera matrices from active camera entity
            BuildCameraMatrices();

            // 2. Sort entities into opaque / alpha lists
            _opaques.Clear();
            _alphas.Clear();

            foreach (var e in B3D.AllEntities())
            {
                if (!e.Visible || e.XnaModel == null) continue;
                if (e.Alpha < 1f || e.BlendMode > 1)
                    _alphas.Add(e);
                else
                    _opaques.Add(e);
            }

            // 3. Sort alpha entities back-to-front (painter's algorithm)
            var camPos = GetCameraPosition();
            _alphas.Sort((a, b) =>
            {
                float da = Vector3.DistanceSquared(a.GetWorldPosition(), camPos);
                float db = Vector3.DistanceSquared(b.GetWorldPosition(), camPos);
                return db.CompareTo(da); // farthest first
            });

            // 4. Set common render state
            _gfx.DepthStencilState = DepthStencilState.Default;
            _gfx.SamplerStates[0]  = SamplerState.LinearWrap;

            // 5. Draw opaques
            _gfx.BlendState = BlendState.Opaque;
            foreach (var e in _opaques) DrawEntity(e);

            // 6. Draw alpha-blended entities
            foreach (var e in _alphas)
            {
                _gfx.BlendState = BlendModeToState(e.BlendMode);
                DrawEntity(e);
            }
        }

        private static void DrawEntity(BlitzEntity e)
        {
            var world = e.GetWorldMatrix();

            foreach (var mesh in e.XnaModel.Meshes)
            {
                foreach (BasicEffect fx in mesh.Effects)
                {
                    fx.World      = world;
                    fx.View       = _view;
                    fx.Projection = _projection;

                    // Apply entity-level colour tint and alpha
                    fx.DiffuseColor = e.EntityColor.ToVector3();
                    fx.Alpha        = e.Alpha;

                    // Fog — match parameters in case they changed this frame
                    fx.FogEnabled = FogEnabled;
                    fx.FogColor   = FogColor.ToVector3();
                    fx.FogStart   = FogStart;
                    fx.FogEnd     = FogEnd;
                }
                mesh.Draw();
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Camera matrix helpers
        // ─────────────────────────────────────────────────────────────────────────

        private static void BuildCameraMatrices()
        {
            int camHandle = B3D.ActiveCamera;
            if (camHandle == -1) return;

            var cam    = B3D.Get(camHandle);
            var camMat = cam.GetWorldMatrix();
            _view      = Matrix.Invert(camMat);

            // CB uses ~73° vertical FOV and a near clip of 0.05 to avoid wall clipping
            _projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(73f),
                _gfx.Viewport.AspectRatio,
                0.05f,
                500f);
        }

        private static Vector3 GetCameraPosition()
        {
            var cam = B3D.Get(B3D.ActiveCamera);
            if (cam == null) return Vector3.Zero;
            var m = cam.GetWorldMatrix();
            return new Vector3(m.M41, m.M42, m.M43);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Blend mode mapping
        // Blitz3D blend modes: 1=solid, 2=alpha, 3=add, 4=multiply
        // ─────────────────────────────────────────────────────────────────────────

        private static BlendState BlendModeToState(int mode) => mode switch
        {
            2 => BlendState.AlphaBlend,
            3 => BlendState.Additive,
            4 => BlendState.NonPremultiplied, // approximate multiply
            _ => BlendState.Opaque,
        };
    }
}
