// DecalSystem.cs — ports CreateDecal + UpdateDecals from Main.bb

using System.Collections.Generic;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public class Decal
    {
        public int Id;
        public int Obj = -1;
        public float Size = 1f;
        public float SizeChange;
        public float MaxSize;
        public float Alpha = 1f;
        public float AlphaChange;
        public float Timer;
        public float Lifetime;
        public int BlendMode;
        public int Fx;
    }

    public static class DecalSystem
    {
        private static readonly List<Decal> _decals = new();
        public static IReadOnlyList<Decal> All => _decals;

        public static Decal Create(int id, float x, float y, float z, float pitch, float yaw, float roll)
        {
            var d = new Decal
            {
                Id = id,
                Obj = CreatePivot(),
                Lifetime = -1f,
            };
            PositionEntity(d.Obj, x, y, z, true);
            RotateEntity(d.Obj, pitch, yaw, roll);
            _decals.Add(d);
            return d;
        }

        public static void Update()
        {
            for (int i = _decals.Count - 1; i >= 0; i--)
            {
                var d = _decals[i];
                if (d.Obj == -1)
                {
                    _decals.RemoveAt(i);
                    continue;
                }

                if (d.SizeChange != 0f)
                {
                    d.Size += d.SizeChange * GameState.FpsFactor;
                    if (d.MaxSize > 0f && d.Size >= d.MaxSize)
                    {
                        d.SizeChange = 0f;
                        d.Size = d.MaxSize;
                    }

                    if (d.Id == 0 && d.Timer <= 0f)
                    {
                        float angle = System.Random.Shared.Next(360);
                        float temp = System.Random.Shared.NextSingle() * d.Size;
                        float ox = EntityX(d.Obj, true) + (float)System.Math.Cos(angle * System.Math.PI / 180.0) * temp;
                        float oz = EntityZ(d.Obj, true) + (float)System.Math.Sin(angle * System.Math.PI / 180.0) * temp;
                        var d2 = Create(1, ox, EntityY(d.Obj, true) - 0.0005f, oz,
                            EntityPitch(d.Obj), System.Random.Shared.NextSingle() * 360f, EntityRoll(d.Obj));
                        d2.Size = System.Random.Shared.NextSingle() * 0.4f + 0.1f;
                        d.Timer = System.Random.Shared.Next(50, 101);
                    }
                    else if (d.Id == 0)
                        d.Timer -= GameState.FpsFactor;
                }

                if (d.AlphaChange != 0f)
                    d.Alpha = System.Math.Min(d.Alpha + GameState.FpsFactor * d.AlphaChange, 1f);

                if (d.Lifetime > 0f)
                    d.Lifetime = System.Math.Max(d.Lifetime - GameState.FpsFactor, 5f);

                if (d.Size <= 0f || d.Alpha <= 0f || d.Lifetime == 5f)
                {
                    FreeEntity(d.Obj);
                    _decals.RemoveAt(i);
                }
            }
        }

        public static void FreeAll()
        {
            foreach (var d in _decals)
                if (d.Obj != -1) FreeEntity(d.Obj);
            _decals.Clear();
        }
    }
}