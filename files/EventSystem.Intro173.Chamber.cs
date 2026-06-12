// EventSystem.Intro173.Chamber.cs — 173 observation chamber (Franklin / Class-D / 173 kills)

using Microsoft.Xna.Framework.Audio;
using SCPCB360.Engine;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static partial class EventSystem
    {
        private static readonly SoundEffect[] _introChamberSfx = new SoundEffect[20];
        private static bool _introChamberSfxLoaded;
        private static int _introChamberAmbienceChn;

        private static void EnsureIntroChamberSfx()
        {
            if (_introChamberSfxLoaded) return;
            _introChamberSfx[0] = AudioSystem.Load("SFX/Room/Intro/Scientist/Franklin/EnterChamber.ogg");
            _introChamberSfx[1] = AudioSystem.Load("SFX/Room/Intro/Scientist/Franklin/Approach173.ogg");
            _introChamberSfx[2] = AudioSystem.Load("SFX/Room/Intro/Scientist/Franklin/Problem.ogg");
            for (int i = 4; i <= 6; i++)
                _introChamberSfx[i] = AudioSystem.Load("SFX/Room/Intro/Scientist/Franklin/Refuse" + (i - 3) + ".ogg");
            _introChamberSfx[7] = AudioSystem.Load("SFX/Room/Intro/Bang1.ogg");
            _introChamberSfx[10] = AudioSystem.Load("SFX/Room/Intro/Light1.ogg");
            _introChamberSfx[16] = AudioSystem.Load("SFX/Room/Intro/Horror.ogg");
            _introChamberSfx[17] = AudioSystem.Load("SFX/Room/Intro/See173.ogg");
            _introChamberSfx[18] = AudioSystem.Load("SFX/Room/Intro/173Chamber.ogg");
            _introChamberSfxLoaded = true;
        }

        private static void Update173ChamberObservation(GameEvent e, EventRoomContext ctx, float rs)
        {
            EnsureIntroChamberSfx();
            Ensure173ChamberNpcs(e, ctx, rs);
            float prev = e.EventState;

            if (_introChamberSfx[18] != null && ctx.Objects[4] != -1)
            {
                _introChamberAmbienceChn = AudioSystem.LoopSound2(_introChamberSfx[18], _introChamberAmbienceChn,
                    GameState.Camera, ctx.Objects[4], 6f);
            }

            if (e.EventState < 10000f)
                Update173ChamberPreWalk(e, ctx, rs, prev);
            else if (e.EventState < 14000f)
                Update173ChamberInside(e, ctx, rs, prev);
            else if (e.EventState < 20000f)
                Update173ChamberHorror(e, ctx, rs, prev);
            else if (e.EventState < 30000f)
                Update173ChamberFinale(e, ctx, rs, prev);
            else
                Complete173Intro(e, ctx, rs);
        }

        private static void Update173ChamberPreWalk(GameEvent e, EventRoomContext ctx, float rs, float prev)
        {
            var room = e.Room;
            var franklin = ctx.Npc[6];
            var curr173 = NPCSystem.Curr173;

            if (_introChamberSfx[17] != null && curr173 != null
                && EntityVisible(curr173.Collider, GameState.Collider)
                && EntityVisible(curr173.Obj, GameState.Camera))
            {
                GameState.Msg = "Press RB to blink.";
                GameState.MsgTimer = 70f * 4f;
                _introChamberSfx[17].Play(GameState.SfxVolume, 0f, 0f);
                _introChamberSfx[17] = null;
            }

            e.EventState = MathUtil.Min(e.EventState + GameState.FpsFactor / 3f, 5000f);

            if (prev < 130f && e.EventState >= 130f && franklin != null)
            {
                franklin.Sfx = _introChamberSfx[0];
                if (franklin.Sfx != null)
                    AudioSystem.PlaySound2(franklin.Sfx, GameState.Camera, franklin.Collider);
            }
            else if (e.EventState > 230f)
            {
                bool allAtPosts = true;
                for (int i = 1; i <= 2; i++)
                {
                    var n = ctx.Npc[i];
                    if (n == null || ctx.Objects[i + 2] == -1) continue;

                    float dist = MathUtil.PointDistance(
                        EntityX(n.Collider, true), EntityZ(n.Collider, true),
                        EntityX(ctx.Objects[i + 2], true), EntityZ(ctx.Objects[i + 2], true));

                    if (dist > 0.3f)
                    {
                        PointEntity(n.Obj, ctx.Objects[i + 2]);
                        RotateEntity(n.Collider, 0f,
                            MathUtil.CurveValue(EntityYaw(n.Obj), EntityYaw(n.Collider), 15f), 0f);
                        if (e.EventState > 200f + i * 30f) n.State = 1f;
                        allAtPosts = false;
                    }
                    else
                    {
                        n.State = 0f;
                        if (ctx.Objects[5] != -1)
                        {
                            PointEntity(n.Obj, ctx.Objects[5]);
                            RotateEntity(n.Collider, 0f,
                                MathUtil.CurveValue(EntityYaw(n.Obj), EntityYaw(n.Collider), 15f), 0f);
                        }
                    }
                }

                float limitX = EntityX(room.obj) + 408f * rs;
                if (EntityX(GameState.Collider, true) < limitX)
                {
                    if (prev < 450f && e.EventState >= 450f && franklin != null && _introChamberSfx[4] != null)
                        AudioSystem.PlaySound2(_introChamberSfx[4], GameState.Camera, franklin.Collider);
                    else if (prev < 650f && e.EventState >= 650f && franklin != null && _introChamberSfx[5] != null)
                        AudioSystem.PlaySound2(_introChamberSfx[5], GameState.Camera, franklin.Collider);
                    else if (prev < 850f && e.EventState >= 850f)
                    {
                        var door1 = ctx.RoomDoors[1];
                        if (door1 != null) DoorSystem.UseDoor(door1, false);
                        if (franklin != null && _introChamberSfx[6] != null)
                            AudioSystem.PlaySound2(_introChamberSfx[6], GameState.Camera, franklin.Collider);
                    }
                    else if (e.EventState > 1000f)
                    {
                        if (ctx.Npc[0] != null)
                        {
                            ctx.Npc[0].State = 1f;
                            ctx.Npc[0].State2 = 10f;
                            ctx.Npc[0].State3 = 1f;
                        }
                        if (ctx.Npc[3] != null) ctx.Npc[3].State = 11f;
                        var door2 = ctx.RoomDoors[2];
                        if (door2 != null)
                        {
                            door2.Locked = false;
                            DoorSystem.UseDoor(door2, false);
                            door2.Locked = true;
                        }
                        e.EventState2 = 1f;
                        return;
                    }

                    if (e.EventState > 850f)
                    {
                        PositionEntity(GameState.Collider,
                            MathUtil.Min(EntityX(GameState.Collider), EntityX(room.obj) + 352f * rs),
                            EntityY(GameState.Collider), EntityZ(GameState.Collider));
                    }
                }
                else if (allAtPosts)
                {
                    e.EventState = 10000f;
                    var door1 = ctx.RoomDoors[1];
                    if (door1 != null) DoorSystem.UseDoor(door1, false);
                }
            }

            if (franklin != null)
            {
                franklin.State = 7f;
                PointEntity(franklin.Obj, GameState.Collider);
                RotateEntity(franklin.Collider, 0f,
                    MathUtil.CurveValue(EntityYaw(franklin.Obj), EntityYaw(franklin.Collider), 20f), 0f, true);
            }

            if (curr173 != null && ctx.Objects[5] != -1)
            {
                PositionEntity(curr173.Collider,
                    EntityX(ctx.Objects[5], true), EntityY(curr173.Collider), EntityZ(ctx.Objects[5], true));
                RotateEntity(curr173.Collider, 0f, 0f, 0f, true);
                ResetEntity(curr173.Collider);
            }
        }

        private static void Update173ChamberInside(GameEvent e, EventRoomContext ctx, float rs, float prev)
        {
            var room = e.Room;
            var franklin = ctx.Npc[6];
            var curr173 = NPCSystem.Curr173;

            e.EventState = MathUtil.Min(e.EventState + GameState.FpsFactor, 13000f);

            if (e.EventState < 10300f)
            {
                PositionEntity(GameState.Collider,
                    MathUtil.Max(EntityX(GameState.Collider), EntityX(room.obj) + 352f * rs),
                    EntityY(GameState.Collider), EntityZ(GameState.Collider));
            }

            if (franklin != null)
            {
                franklin.State = 6f;
                if (curr173 != null) PointEntity(franklin.Obj, curr173.Collider);
                RotateEntity(franklin.Collider, 0f,
                    MathUtil.CurveValue(EntityYaw(franklin.Obj), EntityYaw(franklin.Collider), 50f), 0f, true);
            }

            if (prev < 10300f && e.EventState >= 10300f && _introChamberSfx[1] != null)
            {
                _introChamberSfx[1].Play(GameState.SfxVolume, 0f, 0f);
                PositionEntity(GameState.Collider,
                    MathUtil.Max(EntityX(GameState.Collider), EntityX(room.obj) + 352f * rs),
                    EntityY(GameState.Collider), EntityZ(GameState.Collider));
            }
            else if (prev < 10440f && e.EventState >= 10440f)
            {
                var door1 = ctx.RoomDoors[1];
                if (door1 != null) DoorSystem.UseDoor(door1, false);
                _introChamberSfx[7]?.Play(GameState.SfxVolume, 0f, 0f);
            }
            else if (prev < 10740f && e.EventState >= 10740f)
                _introChamberSfx[2]?.Play(GameState.SfxVolume, 0f, 0f);
            else if (prev < 11145f && e.EventState >= 11145f)
            {
                _introChamberSfx[10]?.Play(GameState.SfxVolume, 0f, 0f);
                var dontLike = AudioSystem.Load("SFX/Room/Intro/ClassD/DontLikeThis.ogg");
                if (ctx.Npc[1] != null)
                    AudioSystem.PlaySound2(dontLike, GameState.Camera, ctx.Npc[2]?.Collider ?? ctx.Npc[1].Collider);
            }
            else if (prev < 11561f && e.EventState >= 11561f)
            {
                e.EventState = 14000f;
                _introChamberSfx[16]?.Play(GameState.SfxVolume, 0f, 0f);
                var breen = AudioSystem.Load("SFX/Room/Intro/ClassD/Breen.ogg");
                if (ctx.Npc[2] != null)
                    AudioSystem.PlaySound2(breen, GameState.Camera, ctx.Npc[1]?.Collider ?? ctx.Npc[2].Collider);
            }

            if (e.EventState >= 10440f && prev < 11561f && ctx.Npc[0] != null)
            {
                var door1 = ctx.RoomDoors[1];
                if (door1 != null && EntityX(GameState.Collider, true) < EntityX(door1.FrameObj, true)
                    && ctx.Npc[0].State != 12f)
                {
                    ctx.Npc[0].Sfx = AudioSystem.Load("SFX/Room/Intro/Guard/Balcony/Alert" + Rand(1, 2) + ".ogg");
                    AudioSystem.PlaySound2(ctx.Npc[0].Sfx, GameState.Camera, ctx.Npc[0].Collider, 20f);
                    ctx.Npc[0].State = 12f;
                    ctx.Npc[0].State2 = 1f;
                }
            }

            if (curr173 != null && ctx.Objects[5] != -1)
            {
                PositionEntity(curr173.Collider,
                    EntityX(ctx.Objects[5], true), EntityY(curr173.Collider), EntityZ(ctx.Objects[5], true));
                RotateEntity(curr173.Collider, 0f, 0f, 0f, true);
                ResetEntity(curr173.Collider);
            }
        }

        private static void Update173ChamberHorror(GameEvent e, EventRoomContext ctx, float rs, float prev)
        {
            var curr173 = NPCSystem.Curr173;
            var classD1 = ctx.Npc[1];
            var classD2 = ctx.Npc[2];
            var balconyGuard = ctx.Npc[0];

            if (e.EventState < 14100f)
            {
                GameState.BlinkTimer = MathUtil.Max((14000f - e.EventState) / 2f - Rnd(0f, 1f), -10f);

                if (GameState.BlinkTimer <= -10f && curr173 != null && classD1 != null)
                {
                    PointEntity(curr173.Collider, classD1.Obj);
                    RotateEntity(curr173.Collider, 0f, EntityYaw(curr173.Collider), 0f);
                    MoveEntity(curr173.Collider, 0f, 0f, curr173.Speed * 0.6f * GameState.FpsFactor);
                    AudioSystem.PlayStoneDrag(GameState.Camera, curr173.Collider);
                    curr173.State = MathUtil.CurveValue(1f, curr173.State, 3f);
                }
                else if (curr173 != null)
                {
                    curr173.State = MathUtil.Max(0f, curr173.State - GameState.FpsFactor / 20f);
                }

                if (prev < 14080f && e.EventState >= 14080f)
                {
                    var bang = AudioSystem.Load("SFX/Room/Intro/Bang2.ogg");
                    bang?.Play(GameState.SfxVolume, 0f, 0f);
                    PlayerSystem.SetCameraShake(3f);
                }
            }
            else if (e.EventState < 14200f)
            {
                GameState.BlinkTimer = -10f;
                if (classD1 != null && classD1.State == 0f)
                {
                    var snap = AudioSystem.Load("SFX/Character/NeckSnap" + Rand(0, 2) + ".ogg");
                    AudioSystem.PlaySound2(snap, GameState.Camera, curr173?.Collider ?? classD1.Collider);
                    classD1.State = 6f;
                }

                if (curr173 != null && classD1 != null)
                {
                    PositionEntity(curr173.Collider,
                        EntityX(classD1.Obj), EntityY(curr173.Collider), EntityZ(classD1.Obj));
                    ResetEntity(curr173.Collider);
                    if (classD2 != null) PointEntity(curr173.Collider, classD2.Collider);
                }

                if (classD2 != null)
                {
                    classD2.State = 3f;
                    MoveEntity(classD2.Collider, 0f, 0f, -0.01f * GameState.FpsFactor);
                }

                if (balconyGuard != null)
                {
                    balconyGuard.State = 12f;
                    balconyGuard.Sfx = AudioSystem.Load("SFX/Room/Intro/Guard/Balcony/WTF" + Rand(1, 2) + ".ogg");
                    AudioSystem.PlaySound2(balconyGuard.Sfx, GameState.Camera, balconyGuard.Collider, 20f);
                }

                if (prev < 14100f && e.EventState >= 14100f)
                {
                    var bang = AudioSystem.Load("SFX/Room/Intro/Bang3.ogg");
                    bang?.Play(GameState.SfxVolume, 0f, 0f);
                }

                if (e.EventState < 14130f)
                {
                    GameState.BlinkTimer = -10f;
                    GameState.LightBlink = 1f;
                }
                else if (curr173 != null)
                {
                    curr173.Idle = false;
                }

                PlayerSystem.SetCameraShake(5f);
            }
            else
            {
                e.EventState = MathUtil.Min(e.EventState + GameState.FpsFactor, 19999f);

                if (e.EventState > 14300f && e.EventState < 14700f)
                {
                    GameState.BlinkTimer = -10f;
                    GameState.LightBlink = 1f;
                }

                if (EntityX(GameState.Collider, true) < EntityX(e.Room.obj) + 448f * rs)
                    e.EventState = 20000f;
            }
        }

        private static void Update173ChamberFinale(GameEvent e, EventRoomContext ctx, float rs, float prev)
        {
            var balconyGuard = ctx.Npc[0];
            var curr173 = NPCSystem.Curr173;
            var room = e.Room;

            e.EventState = MathUtil.Min(e.EventState + GameState.FpsFactor, 29999f);

            if (e.EventState < 20100f)
            {
                PlayerSystem.SetCameraShake(2f);
            }
            else if (e.EventState < 20200f)
            {
                if (prev < 20105f && e.EventState >= 20105f)
                {
                    var ohSh = AudioSystem.Load("SFX/Room/Intro/Guard/Balcony/OhSh.ogg");
                    if (balconyGuard != null)
                    {
                        PositionEntity(balconyGuard.Collider,
                            EntityX(room.obj) - 160f * rs,
                            EntityY(balconyGuard.Collider) + 0.1f,
                            EntityZ(room.obj) + 1280f * rs);
                        ResetEntity(balconyGuard.Collider);
                        AudioSystem.PlaySound2(ohSh, GameState.Camera, balconyGuard.Collider, 20f);
                    }

                    var intro9 = AudioSystem.Load("SFX/Room/Intro/Bang3.ogg");
                    intro9?.Play(GameState.SfxVolume, 0f, 0f);
                }

                if (e.EventState > 20105f && curr173 != null && balconyGuard != null)
                {
                    curr173.Idle = true;
                    PointEntity(balconyGuard.Collider, curr173.Obj);
                    PositionEntity(curr173.Collider,
                        EntityX(room.obj) - 608f * rs,
                        EntityY(room.obj) + 480f * rs,
                        EntityZ(room.obj) + 1312f * rs);
                    ResetEntity(curr173.Collider);
                    PointEntity(curr173.Collider, balconyGuard.Collider);
                }

                GameState.BlinkTimer = -10f;
                GameState.LightBlink = 1f;
                PlayerSystem.SetCameraShake(3f);
            }
            else if (e.EventState < 20300f)
            {
                if (balconyGuard != null && curr173 != null)
                {
                    PointEntity(balconyGuard.Collider, curr173.Collider);
                    MoveEntity(balconyGuard.Collider, 0f, 0f, -0.002f);
                    balconyGuard.State = 2f;
                }

                if (prev < 20260f && e.EventState >= 20260f)
                {
                    var bang = AudioSystem.Load("SFX/Room/Intro/Bang2.ogg");
                    bang?.Play(GameState.SfxVolume, 0f, 0f);
                }
            }
            else if (prev < 20300f)
            {
                GameState.BlinkTimer = -10f;
                GameState.LightBlink = 1f;
                PlayerSystem.SetCameraShake(3f);
                var light = AudioSystem.Load("SFX/Room/Intro/Light2.ogg");
                light?.Play(GameState.SfxVolume, 0f, 0f);

                if (balconyGuard != null)
                {
                    var snap = AudioSystem.Load("SFX/Character/NeckSnap1.ogg");
                    AudioSystem.PlaySound2(snap, GameState.Camera, balconyGuard.Collider);
                }

                if (curr173 != null) curr173.Idle = false;

                var vent = AudioSystem.Load("SFX/Room/Intro/173Vent.ogg");
                vent?.Play(GameState.SfxVolume, 0f, 0f);

                if (curr173 != null)
                {
                    PositionEntity(curr173.Collider,
                        EntityX(room.obj) - 400f * rs, 100f, EntityZ(room.obj) + 1072f * rs);
                    ResetEntity(curr173.Collider);
                }

                TeleportIntroToStartRoom(e, ctx, rs);
                e.EventState = 30000f;
            }
        }

        private static void TeleportIntroToStartRoom(GameEvent e, EventRoomContext ctx, float rs)
        {
            var startRoom = FindRoomByName("start");
            if (startRoom == null) return;

            var startCtx = GetContext(startRoom);
            float ox = EntityX(startRoom.obj, true) + 3712f * rs;
            float oy = 384f * rs;
            float oz = EntityZ(startRoom.obj, true) + 1312f * rs;

            GameState.PlayerRoom = startRoom;
            PositionEntity(GameState.Collider,
                ox + (EntityX(GameState.Collider) - EntityX(e.Room.obj)),
                oy + EntityY(GameState.Collider) + 0.4f,
                oz + (EntityZ(GameState.Collider) - EntityZ(e.Room.obj)));
            GameState.DropSpeed = 0f;
            ResetEntity(GameState.Collider);

            for (int i = 0; i <= 2; i++)
            {
                var n = ctx.Npc[i];
                if (n == null) continue;
                PositionEntity(n.Collider,
                    ox + (EntityX(n.Collider) - EntityX(e.Room.obj)),
                    oy + EntityY(n.Collider) + 0.4f,
                    oz + (EntityZ(n.Collider) - EntityZ(e.Room.obj)));
                ResetEntity(n.Collider);
            }

            MusicSystem.ClearForcedTrack();
            RenderSystem.FogEnabled = true;

            if (ctx.Npc[0] != null)
            {
                startCtx.Npc[0] = ctx.Npc[0];
                startCtx.Npc[0].State = 8f;
            }
            if (ctx.Npc[6] != null)
                startCtx.Npc[1] = ctx.Npc[6];

            for (int i = 3; i <= 4; i++)
                if (ctx.Npc[i] != null) NPCSystem.Remove(ctx.Npc[i]);

            e.EventState2 = 1f;
        }

        private static void Ensure173ChamberNpcs(GameEvent e, EventRoomContext ctx, float rs)
        {
            var room = e.Room;

            if (ctx.Npc[1] == null)
            {
                ctx.Npc[1] = NPCSystem.CreateNpc(NPCSystem.NpcTypeD, room.x, 0.5f, room.z - 1f);
                ctx.Npc[1].TextureId = 3;
            }

            if (ctx.Npc[2] == null)
                ctx.Npc[2] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard, room.x, 0.5f, room.z + 528f * rs);

            if (ctx.Npc[0] == null && ctx.Objects[2] != -1)
            {
                ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard,
                    EntityX(ctx.Objects[2], true), EntityY(ctx.Objects[2], true), EntityZ(ctx.Objects[2], true));
                ctx.Npc[0].Frame = 74f;
                ctx.Npc[0].State = 8f;
            }
        }

        private static void Complete173Intro(GameEvent e, EventRoomContext ctx, float rs)
        {
            e.EventState2 = 1f;
            MusicSystem.ClearForcedTrack();
        }
    }
}