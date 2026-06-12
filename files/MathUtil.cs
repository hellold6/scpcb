// MathUtil.cs — ports CB utility functions from Main.bb

using System;
using Microsoft.Xna.Framework;
using SCPCB360.Engine;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static class MathUtil
    {
        public static float CurveValue(float number, float old, float smooth)
        {
            if (smooth <= 0f) return number;
            return old + (number - old) / smooth * GameState.FpsFactor;
        }

        public static float CurveAngle(float val, float old, float smooth)
        {
            float delta = WrapAngle(val - old);
            return WrapAngle(old + delta / smooth * GameState.FpsFactor);
        }

        public static float WrapAngle(float angle)
        {
            while (angle >= 360f) angle -= 360f;
            while (angle < 0f) angle += 360f;
            return angle;
        }

        public static float GetAngle(float x1, float y1, float x2, float y2)
            => (float)(Math.Atan2(y2 - y1, x2 - x1) * (180.0 / Math.PI));

        public static float PointDirection(float x1, float z1, float x2, float z2)
            => GetAngle(x1, z1, x2, z2);

        public static float PointDistance(float x1, float z1, float x2, float z2)
            => (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (z2 - z1) * (z2 - z1));

        public static float Distance(float x1, float y1, float x2, float y2)
            => (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

        public static float AngleDist(float a0, float a1)
        {
            float d = Math.Abs(WrapAngle(a0) - WrapAngle(a1));
            return d > 180f ? 360f - d : d;
        }

        public static float Min(float a, float b) => a < b ? a : b;
        public static float Max(float a, float b) => a > b ? a : b;

        public static int GenerateSeedNumber(string seed)
        {
            if (string.IsNullOrEmpty(seed)) return Environment.TickCount;
            int hash = 0;
            foreach (char c in seed)
                hash = hash * 31 + c;
            return Math.Abs(hash);
        }

        public static string F2S(float n, int count)
            => n.ToString("F" + count);

        public static float DeltaYaw(int from, int to)
        {
            float angle = GetAngle(EntityX(from, true), EntityZ(from, true), EntityX(to, true), EntityZ(to, true));
            return WrapAngle(angle - EntityYaw(from));
        }

        public static float DeltaPitch(int from, int to)
        {
            float dx = EntityX(to, true) - EntityX(from, true);
            float dy = EntityY(to, true) - EntityY(from, true);
            float dz = EntityZ(to, true) - EntityZ(from, true);
            float horiz = (float)Math.Sqrt(dx * dx + dz * dz);
            float pitch = (float)(Math.Atan2(-dy, horiz) * (180.0 / Math.PI));
            return WrapAngle(pitch - EntityPitch(from));
        }
    }
}