// PortalRenderer.cs — ports DrawPortals.bb (CreateDrawPortal, UpdateDrawPortal)

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SCPCB360.Engine;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public class DrawPortal
    {
        public float Width;
        public float Height;
        public int Camera = -1;
        public int PortalMesh = -1;
        public float CamZoom = 1f;
        public float CamPitch, CamYaw, CamRoll;
        public RenderTarget2D Texture;
        public int TexW, TexH;
        public int Id;
    }

    public static class PortalRenderer
    {
        private static readonly List<DrawPortal> _portals = new();
        private static GraphicsDevice _gfx;
        private static int _nextId;

        public static IReadOnlyList<DrawPortal> All => _portals;

        public static void Initialize(GraphicsDevice gfx) => _gfx = gfx;

        public static DrawPortal CreateDrawPortal(
            float x, float y, float z,
            float pitch, float yaw, float roll,
            float w, float h,
            float camX = 0f, float camY = 0f, float camZ = 0f,
            float camPitch = 0f, float camYaw = 0f, float camRoll = 0f,
            float camZoom = 1f,
            int texW = 2048, int texH = 2048)
        {
            var dp = new DrawPortal
            {
                Width = w,
                Height = h,
                CamZoom = camZoom,
                CamPitch = camPitch,
                CamYaw = camYaw,
                CamRoll = camRoll,
                TexW = texW,
                TexH = texH,
                Id = ++_nextId,
            };

            dp.Texture = new RenderTarget2D(_gfx, texW, texH, false, SurfaceFormat.Color, DepthFormat.Depth24);
            dp.Camera = CreateCamera();
            CameraRange(dp.Camera, 0.5f, 20f);
            PositionEntity(dp.Camera, camX, camY, camZ, true);
            RotateEntity(dp.Camera, camPitch, camYaw, camRoll, true);

            dp.PortalMesh = CreatePivot();
            PositionEntity(dp.PortalMesh, x, y, z, true);
            RotateEntity(dp.PortalMesh, pitch, yaw, roll, true);
            ScaleEntity(dp.PortalMesh, w / 2f, h / 2f, 0.05f);

            var ent = Get(dp.PortalMesh);
            if (ent != null)
            {
                ent.PortalTexture = dp.Texture;
                ent.BlendMode = 2;
            }

            _portals.Add(dp);
            return dp;
        }

        public static void DestroyDrawPortal(DrawPortal dp)
        {
            if (dp == null) return;
            if (dp.Camera != -1) FreeEntity(dp.Camera);
            if (dp.PortalMesh != -1) FreeEntity(dp.PortalMesh);
            dp.Texture?.Dispose();
            _portals.Remove(dp);
        }

        public static void UpdateDrawPortal(DrawPortal dp)
        {
            if (dp == null || dp.Camera == -1 || dp.Texture == null) return;

            RotateEntity(dp.Camera, dp.CamPitch, dp.CamYaw, dp.CamRoll, true);

            int vpX = (dp.TexW / 2) - (_gfx.Viewport.Width / 2);
            int vpY = (dp.TexH / 2) - (_gfx.Viewport.Height / 2);

            var prevVp = _gfx.Viewport;
            _gfx.Viewport = new Viewport(vpX, vpY, _gfx.Viewport.Width, _gfx.Viewport.Height);

            RenderSystem.DrawToTarget(dp.Texture, dp.Camera, new Color(10, 10, 12));
            _gfx.Viewport = prevVp;
        }

        public static void UpdateAll()
        {
            foreach (var dp in _portals)
                UpdateDrawPortal(dp);
        }

        public static void FreeAll()
        {
            for (int i = _portals.Count - 1; i >= 0; i--)
                DestroyDrawPortal(_portals[i]);
        }
    }
}