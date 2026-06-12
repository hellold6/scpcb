// DevilParticleSystem.cs — ports DevilParticleSystem.bb (bytecode77 particle engine)

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SCPCB360.Engine;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public class DevilTemplate
    {
        public readonly DevilTemplate[] SubTemplates = new DevilTemplate[8];
        public int EmitterBlend = 3;
        public int Interval = 1;
        public int ParticlesPerInterval = 1;
        public int MaxParticles = -1;
        public int EmitterMaxTime = 100;
        public int MinTime = 0;
        public int MaxTime = 20;
        public string TexturePath = "";
        public bool AnimTex;
        public float TexFrame;
        public int MaxTexFrames;
        public float TexSpeed = 1f;
        public float MinOx, MaxOx, MinOy, MaxOy, MinOz, MaxOz;
        public float MinXv, MaxXv, MinYv, MaxYv, MinZv, MaxZv;
        public float RotVel1, RotVel2;
        public bool AlignToFall;
        public float AlignToFallOffset;
        public float Gravity;
        public float Alpha = 1f;
        public int AlphaVel;
        public float Sx = 1f, Sy = 1f;
        public float SizeMult1 = 1f, SizeMult2 = 1f;
        public float SizeAdd, SizeMult = 1f;
        public int R1 = 255, G1 = 255, B1 = 255;
        public int R2 = 255, G2 = 255, B2 = 255;
        public int Brightness = 1;
        public float FloorY = -1_000_000f;
        public float FloorBounce = 0.5f;
        public float PitchFix = -1f;
        public float YawFix = -1f;
        public float Yaw;
        public int Handle;
        private static int _nextHandle = 1;

        public DevilTemplate() => Handle = _nextHandle++;
    }

    public class DevilEmitter
    {
        public bool Fixed;
        public int CntLoop;
        public int Age;
        public int MaxTime;
        public DevilTemplate Template;
        public int Owner = -1;
        public int Ent = -1;
        public bool Del;
        public bool Frozen;
    }

    public class DevilParticle
    {
        public DevilEmitter Emitter;
        public int Age;
        public int MaxTime;
        public float X, Y, Z;
        public float Xv, Yv, Zv;
        public float Rot, RotVel;
        public float Sx, Sy;
        public int Obj = -1;
    }

    public static class DevilParticleSystem
    {
        private static readonly List<DevilTemplate> _templates = new();
        private static readonly List<DevilEmitter> _emitters = new();
        private static readonly List<DevilParticle> _particles = new();

        public static int ParticleCam = -1;
        public static int ParticlePivot = -1;

        public static void InitParticles(int cam)
        {
            ParticleCam = cam;
            if (ParticlePivot == -1)
                ParticlePivot = CreatePivot();
            SeedRnd(Environment.TickCount);
        }

        public static void FreeParticles()
        {
            foreach (var e in _emitters.ToArray())
                FreeEmitter(e.Owner, true);
            _templates.Clear();
            _emitters.Clear();
            _particles.Clear();
            if (ParticlePivot != -1)
            {
                FreeEntity(ParticlePivot);
                ParticlePivot = -1;
            }
        }

        public static DevilEmitter CreateDevilEmitter(float x, float y, float z, RoomInstance room,
            int particleId, float maxTime = 2f)
        {
            var dem = new DevilEmitter
            {
                Ent = CreatePivot(),
                Template = GetTemplate(particleId),
                MaxTime = (int)(maxTime * 70f),
            };
            PositionEntity(dem.Ent, x, y, z, true);
            if (room != null)
                EntityParent(dem.Ent, room.obj);
            _emitters.Add(dem);
            return dem;
        }

        public static int CreateTemplate()
        {
            var tmp = new DevilTemplate();
            _templates.Add(tmp);
            SetTemplateEmitterBlend(tmp.Handle, 3);
            SetTemplateInterval(tmp.Handle, 1);
            SetTemplateParticlesPerInterval(tmp.Handle, 1);
            SetTemplateMaxParticles(tmp.Handle, -1);
            SetTemplateEmitterLifeTime(tmp.Handle, 100);
            SetTemplateParticleLifeTime(tmp.Handle, 0, 20);
            SetTemplateAlpha(tmp.Handle, 1f);
            SetTemplateSize(tmp.Handle, 1f, 1f);
            SetTemplateSizeVel(tmp.Handle, 0f, 1f);
            SetTemplateColors(tmp.Handle, 0xFFFFFF, 0xFFFFFF);
            SetTemplateBrightness(tmp.Handle, 1);
            SetTemplateFloor(tmp.Handle, -1_000_000f);
            SetTemplateFixAngles(tmp.Handle, -1f, -1f);
            return tmp.Handle;
        }

        public static void FreeTemplate(int templateHandle)
        {
            var tmp = GetTemplate(templateHandle);
            if (tmp == null) return;
            for (int i = 0; i < tmp.SubTemplates.Length; i++)
                if (tmp.SubTemplates[i] != null)
                    FreeTemplate(tmp.SubTemplates[i].Handle);
            _templates.Remove(tmp);
        }

        public static void SetTemplateEmitterBlend(int template, int emitterBlend)
            => GetTemplate(template).EmitterBlend = emitterBlend;

        public static void SetTemplateInterval(int template, int interval)
            => GetTemplate(template).Interval = Math.Max(1, interval);

        public static void SetTemplateParticlesPerInterval(int template, int count)
            => GetTemplate(template).ParticlesPerInterval = count;

        public static void SetTemplateMaxParticles(int template, int maxParticles)
            => GetTemplate(template).MaxParticles = maxParticles;

        public static void SetTemplateParticleLifeTime(int template, int minTime, int maxTime)
        {
            var t = GetTemplate(template);
            t.MinTime = minTime;
            t.MaxTime = maxTime;
        }

        public static void SetTemplateEmitterLifeTime(int template, int emitterMaxTime)
            => GetTemplate(template).EmitterMaxTime = emitterMaxTime;

        public static void SetTemplateTexture(int template, string path, int mode = 0, int blend = 1)
        {
            var t = GetTemplate(template);
            t.TexturePath = path;
            t.AnimTex = false;
        }

        public static void SetTemplateOffset(int template,
            float minOx, float maxOx, float minOy, float maxOy, float minOz, float maxOz)
        {
            var t = GetTemplate(template);
            t.MinOx = minOx; t.MaxOx = maxOx;
            t.MinOy = minOy; t.MaxOy = maxOy;
            t.MinOz = minOz; t.MaxOz = maxOz;
        }

        public static void SetTemplateVelocity(int template,
            float minXv, float maxXv, float minYv, float maxYv, float minZv, float maxZv)
        {
            var t = GetTemplate(template);
            t.MinXv = minXv; t.MaxXv = maxXv;
            t.MinYv = minYv; t.MaxYv = maxYv;
            t.MinZv = minZv; t.MaxZv = maxZv;
        }

        public static void SetTemplateRotation(int template, float rotVel1, float rotVel2)
        {
            var t = GetTemplate(template);
            t.RotVel1 = rotVel1;
            t.RotVel2 = rotVel2;
        }

        public static void SetTemplateAlignToFall(int template, bool alignToFall, float offset = 0f)
        {
            var t = GetTemplate(template);
            t.AlignToFall = alignToFall;
            t.AlignToFallOffset = offset;
        }

        public static void SetTemplateGravity(int template, float gravity)
            => GetTemplate(template).Gravity = gravity;

        public static void SetTemplateSize(int template, float sx, float sy,
            float sizeMult1 = 1f, float sizeMult2 = 1f)
        {
            var t = GetTemplate(template);
            t.Sx = sx; t.Sy = sy;
            t.SizeMult1 = sizeMult1; t.SizeMult2 = sizeMult2;
        }

        public static void SetTemplateSizeVel(int template, float sizeAdd, float sizeMult)
        {
            var t = GetTemplate(template);
            t.SizeAdd = sizeAdd;
            t.SizeMult = sizeMult;
        }

        public static void SetTemplateAlpha(int template, float alpha)
            => GetTemplate(template).Alpha = alpha;

        public static void SetTemplateAlphaVel(int template, int alphaVel)
            => GetTemplate(template).AlphaVel = alphaVel;

        public static void SetTemplateColors(int template, int col1, int col2)
        {
            var t = GetTemplate(template);
            t.R1 = (col1 >> 16) & 0xFF;
            t.G1 = (col1 >> 8) & 0xFF;
            t.B1 = col1 & 0xFF;
            t.R2 = (col2 >> 16) & 0xFF;
            t.G2 = (col2 >> 8) & 0xFF;
            t.B2 = col2 & 0xFF;
        }

        public static void SetTemplateBrightness(int template, int brightness)
            => GetTemplate(template).Brightness = Math.Max(1, brightness);

        public static void SetTemplateFloor(int template, float floorY, float floorBounce = 0.5f)
        {
            var t = GetTemplate(template);
            t.FloorY = floorY;
            t.FloorBounce = floorBounce;
        }

        public static void SetTemplateFixAngles(int template, float pitchFix, float yawFix)
        {
            var t = GetTemplate(template);
            t.PitchFix = pitchFix;
            t.YawFix = yawFix;
        }

        public static void SetTemplateSubTemplate(int template, int subTemplate)
        {
            var t = GetTemplate(template);
            var sub = GetTemplate(subTemplate);
            if (sub == null) return;
            for (int i = 0; i < t.SubTemplates.Length; i++)
            {
                if (t.SubTemplates[i] == null)
                {
                    t.SubTemplates[i] = sub;
                    break;
                }
            }
        }

        public static void SetTemplateYaw(int template, float yaw)
            => GetTemplate(template).Yaw = yaw;

        public static int SetEmitter(int owner, int template, bool fixedEmitter = false)
        {
            var tmp = GetTemplate(template);
            if (tmp == null || owner == -1) return -1;

            var e = new DevilEmitter { Template = tmp };
            if (fixedEmitter)
            {
                e.Owner = CreatePivot();
                PositionEntity(e.Owner,
                    EntityX(owner), EntityY(owner), EntityZ(owner));
                e.Fixed = true;
            }
            else
            {
                e.Owner = owner;
            }

            e.Ent = CreatePivot(e.Owner);
            e.MaxTime = tmp.EmitterMaxTime;
            EntityBlend(e.Ent, tmp.EmitterBlend);
            _emitters.Add(e);

            for (int i = 0; i < tmp.SubTemplates.Length; i++)
            {
                if (tmp.SubTemplates[i] != null && !string.IsNullOrEmpty(tmp.SubTemplates[i].TexturePath))
                    SetEmitter(owner, tmp.SubTemplates[i].Handle, fixedEmitter);
            }

            return e.Ent;
        }

        public static void FreeEmitter(int ent, bool deleteParticles = true)
        {
            foreach (var e in _emitters.ToArray())
            {
                if (e.Owner != ent && e.Ent != ent) continue;

                if (deleteParticles)
                {
                    _particles.RemoveAll(p => p.Emitter == e);
                    FreeEntity(e.Ent);
                    if (e.Fixed && e.Owner != -1) FreeEntity(e.Owner);
                    _emitters.Remove(e);
                }
                else
                {
                    e.Del = true;
                }
            }
        }

        public static void FreezeEmitter(int ent)
        {
            foreach (var e in _emitters)
                if (e.Owner == ent || e.Ent == ent) e.Frozen = true;
        }

        public static void UnfreezeEmitter(int ent)
        {
            foreach (var e in _emitters)
                if (e.Owner == ent || e.Ent == ent) e.Frozen = false;
        }

        public static void UpdateParticlesDevil()
        {
            if (ParticleCam == -1) return;

            PositionEntity(ParticlePivot,
                EntityX(ParticleCam, true),
                EntityY(ParticleCam, true),
                EntityZ(ParticleCam, true),
                true);

            float camPitch = EntityPitch(ParticleCam);
            float camYaw = EntityYaw(ParticleCam);
            float camRoll = EntityRoll(ParticleCam);

            foreach (var e in _emitters.ToArray())
            {
                int cntParticles = 0;
                if (e.Template.MaxParticles > -1)
                {
                    foreach (var p in _particles)
                        if (p.Emitter == e) cntParticles++;
                }

                if (e.MaxTime > -1)
                {
                    if (e.Age > e.MaxTime) e.Del = true;
                    else e.Age++;
                }

                if (!e.Frozen && !e.Del)
                {
                    e.CntLoop = (e.CntLoop + 1) % e.Template.Interval;
                    if (e.CntLoop == 0)
                    {
                        for (int i = 0; i < e.Template.ParticlesPerInterval; i++)
                        {
                            bool canSpawn = e.Template.MaxParticles == -1 || cntParticles < e.Template.MaxParticles;
                            if (!canSpawn) continue;

                            float sm = Rnd(e.Template.SizeMult1, e.Template.SizeMult2);
                            var p = new DevilParticle
                            {
                                Emitter = e,
                                MaxTime = Rand(e.Template.MinTime, e.Template.MaxTime),
                                X = Rnd(e.Template.MinOx, e.Template.MaxOx),
                                Y = Rnd(e.Template.MinOy, e.Template.MaxOy),
                                Z = Rnd(e.Template.MinOz, e.Template.MaxOz),
                                Xv = Rnd(e.Template.MinXv, e.Template.MaxXv),
                                Yv = Rnd(e.Template.MinYv, e.Template.MaxYv),
                                Zv = Rnd(e.Template.MinZv, e.Template.MaxZv),
                                RotVel = Rnd(e.Template.RotVel1, e.Template.RotVel2),
                                Sx = e.Template.Sx * sm,
                                Sy = e.Template.Sy * sm,
                                Obj = CreatePivot(e.Ent),
                            };
                            ScaleEntity(p.Obj, p.Sx, p.Sy, 0.01f);
                            _particles.Add(p);
                            cntParticles++;
                        }
                    }
                }

                if (e.Template.AnimTex)
                {
                    e.Template.TexFrame += e.Template.TexSpeed;
                    if (e.Template.TexFrame > e.Template.MaxTexFrames - 1)
                        e.Template.TexFrame = 0f;
                }

                if (e.Del)
                {
                    bool allDead = true;
                    foreach (var p in _particles)
                        if (p.Emitter == e) { allDead = false; break; }

                    if (allDead)
                    {
                        FreeEntity(e.Ent);
                        if (e.Fixed && e.Owner != -1) FreeEntity(e.Owner);
                        _emitters.Remove(e);
                    }
                }
            }

            foreach (var p in _particles.ToArray())
            {
                if (p.Age > p.MaxTime)
                {
                    FreeEntity(p.Obj);
                    _particles.Remove(p);
                    continue;
                }

                var tmp = p.Emitter.Template;
                if (!p.Emitter.Frozen)
                {
                    p.Age++;
                    if (tmp.AlignToFall)
                        p.Rot = tmp.AlignToFallOffset - ATan2(p.Xv, p.Yv);
                    else
                        p.Rot += p.RotVel;

                    p.Yv -= tmp.Gravity * GameState.FpsFactor;
                    p.X += p.Xv * GameState.FpsFactor;
                    p.Y += p.Yv * GameState.FpsFactor;
                    p.Z += p.Zv * GameState.FpsFactor;

                    if (p.Y < tmp.FloorY)
                        p.Yv = -p.Yv * tmp.FloorBounce;

                    p.Sx = (p.Sx + tmp.SizeAdd) * tmp.SizeMult;
                    p.Sy = (p.Sy + tmp.SizeAdd) * tmp.SizeMult;
                }

                float t = p.MaxTime > 0 ? (float)p.Age / p.MaxTime : 0f;
                int r = (int)(tmp.R1 + (tmp.R2 - tmp.R1) * t);
                int g = (int)(tmp.G1 + (tmp.G2 - tmp.G1) * t);
                int b = (int)(tmp.B1 + (tmp.B2 - tmp.B1) * t);
                float alpha = tmp.AlphaVel != 0 ? (1f - t) * tmp.Alpha : tmp.Alpha;

                RotateEntity(ParticlePivot, camPitch, camYaw, camRoll + p.Rot + tmp.AlignToFallOffset);
                if (tmp.PitchFix > -1f)
                    RotateEntity(ParticlePivot, tmp.PitchFix, EntityYaw(ParticlePivot), EntityRoll(ParticlePivot));
                if (tmp.YawFix > -1f)
                    RotateEntity(ParticlePivot, EntityPitch(ParticlePivot), tmp.YawFix, EntityRoll(ParticlePivot));

                PositionEntity(p.Obj, p.X, p.Y, p.Z, true);
                ScaleEntity(p.Obj, p.Sx, p.Sy, 0.01f);
                EntityAlpha(p.Obj, alpha);
                EntityColor(p.Obj, r, g, b);
            }
        }

        private static DevilTemplate GetTemplate(int handle)
            => _templates.Find(t => t.Handle == handle);
    }
}