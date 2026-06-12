// BlurFilter.cs — ports Dreamfilter.bb (CreateBlurImage, UpdateBlur)

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SCPCB360.Engine;

namespace SCPCB360.GameLogic
{
    public static class BlurFilter
    {
        private static GraphicsDevice _gfx;
        private static SpriteBatch _sb;
        private static RenderTarget2D _blurTargetA;
        private static RenderTarget2D _blurTargetB;
        private static int _screenW;
        private static int _screenH;

        public static float BlurVolume { get; set; }
        public static float BlurTimer { get; set; }

        public static void Initialize(GraphicsDevice gfx)
        {
            _gfx = gfx;
            _sb = new SpriteBatch(gfx);
            Resize(gfx.Viewport.Width, gfx.Viewport.Height);
        }

        public static void Resize(int width, int height)
        {
            if (_gfx == null) return;
            if (_blurTargetA != null && width == _screenW && height == _screenH) return;

            _screenW = width;
            _screenH = height;
            _blurTargetA?.Dispose();
            _blurTargetB?.Dispose();
            _blurTargetA = new RenderTarget2D(_gfx, Math.Max(1, width / 2), Math.Max(1, height / 2), false, SurfaceFormat.Color, DepthFormat.None);
            _blurTargetB = new RenderTarget2D(_gfx, Math.Max(1, width / 4), Math.Max(1, height / 4), false, SurfaceFormat.Color, DepthFormat.None);
        }

        public static void Update(float delta)
        {
            BlurVolume = Math.Min(MathUtil.CurveValue(0f, BlurVolume, 20f), 0.95f);
            if (BlurTimer > 0f)
            {
                BlurVolume = Math.Max(Math.Min(0.95f, BlurTimer / 1000f), BlurVolume);
                BlurTimer = Math.Max(BlurTimer - delta * 60f, 0f);
            }
        }

        /// <summary>Ports UpdateBlur(power#) using captured scene texture.</summary>
        public static void Draw(float power, Texture2D sceneTexture)
        {
            if (_gfx == null || _sb == null || sceneTexture == null || power <= 0.001f) return;

            DrawScaledPass(sceneTexture, _blurTargetA);
            DrawScaledPass(_blurTargetA, _blurTargetB);

            float alpha = MathHelper.Clamp(power, 0f, 0.95f);
            _sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp);
            _sb.Draw(_blurTargetB, new Rectangle(0, 0, _screenW, _screenH), Color.White * alpha);
            _sb.End();
        }

        private static void DrawScaledPass(Texture2D source, RenderTarget2D dest)
        {
            var prev = _gfx.GetRenderTargets();
            _gfx.SetRenderTarget(dest);
            _gfx.Clear(Color.Transparent);
            _sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearClamp);
            _sb.Draw(source, new Rectangle(0, 0, dest.Width, dest.Height), Color.White);
            _sb.End();
            _gfx.SetRenderTargets(prev);
        }
    }
}