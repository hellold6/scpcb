// ParticleSystem.cs — ports Particles.bb

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SCPCB360.Engine;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public class Emitter
    {
        public int Obj = -1;
        public float RandAngle;
        public float Speed;
        public float SizeChange;
        public float AChange;
        public float Gravity;
        public RoomInstance Room;
    }

    public class Particle
    {
        public int Obj = -1;
        public int Pvt = -1;
        public int Image;
        public float R = 255, G = 255, B = 255, A = 1f;
        public float Size = 1f;
        public float Speed;
        public float YSpeed;
        public float Gravity = 1f;
        public float AChange;
        public float SizeChange;
        public float Lifetime = 200f;
    }

    public static class ParticleSystem
    {
        private static readonly List<Particle> _particles = new();
        private static readonly List<Particle> _pendingRemove = new();
        private static readonly List<Emitter> _emitters = new();

        public static Emitter CreateEmitter(float x, float y, float z, int emitterType)
        {
            var e = new Emitter { Obj = CreatePivot() };
            PositionEntity(e.Obj, x, y, z, true);
            _emitters.Add(e);
            return e;
        }

        public static Particle Create(float x, float y, float z, int image, float size,
            float gravity = 1f, float lifetime = 200f)
        {
            var p = new Particle
            {
                Image = image,
                Size = size,
                Gravity = gravity * 0.004f,
                Lifetime = lifetime,
                Pvt = CreatePivot(),
                Obj = CreatePivot(),
            };

            PositionEntity(p.Pvt, x, y, z, true);
            PositionEntity(p.Obj, x, y, z, true);
            ScaleEntity(p.Obj, size, size, size);

            _particles.Add(p);
            return p;
        }

        public static void Update()
        {
            foreach (var p in _particles)
            {
                MoveEntity(p.Pvt, 0, 0, p.Speed * GameState.FpsFactor);
                if (p.Gravity != 0f)
                    p.YSpeed -= p.Gravity * GameState.FpsFactor;

                var pvt = Get(p.Pvt);
                if (pvt != null)
                {
                    var pos = pvt.GetWorldPosition();
                    pos.Y += p.YSpeed * GameState.FpsFactor;
                    PositionEntity(p.Pvt, pos.X, pos.Y, pos.Z, true);
                    PositionEntity(p.Obj,
                        pos.X, pos.Y, pos.Z, true);
                }

                if (p.AChange != 0f)
                    p.A = MathHelper.Clamp(p.A + p.AChange * GameState.FpsFactor, 0f, 1f);

                if (p.SizeChange != 0f)
                    p.Size += p.SizeChange * GameState.FpsFactor;

                p.Lifetime -= GameState.FpsFactor;
                if (p.Lifetime <= 0f || p.Size < 0.00001f || p.A <= 0f)
                    _pendingRemove.Add(p);
            }

            foreach (var p in _pendingRemove)
                Remove(p);
            _pendingRemove.Clear();
        }

        public static void Remove(Particle p)
        {
            FreeEntity(p.Obj);
            FreeEntity(p.Pvt);
            _particles.Remove(p);
        }

        public static void FreeAll()
        {
            foreach (var p in _particles.ToArray())
                Remove(p);
        }
    }
}