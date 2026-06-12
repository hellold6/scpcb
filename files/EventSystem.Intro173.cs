// EventSystem.Intro173.cs — ports UpdateEvents.bb Case "173" (intro cinematic)

using System;
using SCPCB360.Engine;
using SCPCB360.Input;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static partial class EventSystem
    {
        private static int _introLight = -1;
        private static bool _introDocHandoff;
        private static float _introCamPrev3;

        public static bool IntroDocHandoffReady => _introDocHandoff;

        public static bool HandleIntroInteract()
        {
            if (!_introDocHandoff) return false;
            foreach (var ev in _events)
            {
                if (ev.EventName == "173" && ev.EventState2 == 0f && ev.EventState3 is >= 905f and <= 910f)
                {
                    CompleteIntroDocHandoff(ev);
                    return true;
                }
            }
            return false;
        }

        private static void Update173(GameEvent e)
        {
            if (GameState.KillTimer < 0 || e.EventState2 != 0f) return;

            PlayerZone = 0;
            var ctx = GetContext(e.Room);
            float rs = GameState.RoomScale;

            if (e.EventState3 > 0f)
            {
                MusicSystem.ForceTrack(13);
                Update173Escort(e, ctx, rs);
            }
            else
            {
                MusicSystem.ClearForcedTrack();
                Update173ChamberIdle(e, ctx, rs);
            }
        }

        private static void Update173ChamberIdle(GameEvent e, EventRoomContext ctx, float rs)
        {
            if (e.EventState == 0f && InPlayerRoom(e))
                SetupIntro173Room(e, ctx, rs);
            else if (e.EventState >= 1f)
                Update173ChamberObservation(e, ctx, rs);
        }

        private static void SetupIntro173Room(GameEvent e, EventRoomContext ctx, float rs)
        {
            var room = e.Room;
            if (NPCSystem.Curr173 == null)
                NPCSystem.Spawn173();

            if (ctx.Npc[3] == null)
            {
                ctx.Npc[3] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard,
                    room.x - 4096f * rs + Rnd(-0.3f, 0.3f), 0.3f, room.z + Rand(860, 896) * rs);
                RotateEntity(ctx.Npc[3].Collider, 0, room.Angle + 180f, 0);
                ctx.Npc[3].State = 7f;
                ctx.Npc[3].IgnorePlayer = true;
            }

            if (ctx.Npc[4] == null)
            {
                ctx.Npc[4] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard,
                    room.x - 3840f * rs, 0.3f, room.z + 768f * rs);
                RotateEntity(ctx.Npc[4].Collider, 0, room.Angle + 135f, 0);
                ctx.Npc[4].State = 7f;
                ctx.Npc[4].IgnorePlayer = true;
            }

            if (ctx.Npc[5] == null)
            {
                ctx.Npc[5] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard,
                    room.x - 8288f * rs, 0.3f, room.z + 1096f * rs);
                ctx.Npc[5].Sfx = AudioSystem.Load("SFX/Room/Intro/Guard/Music" + Rand(1, 5) + ".ogg");
                RotateEntity(ctx.Npc[5].Collider, 0, room.Angle + 180f, 0, true);
                ctx.Npc[5].State = 7f;
                ctx.Npc[5].Sfx2 = AudioSystem.Load("SFX/Room/Intro/Guard/PlayerEscape.ogg");
                ctx.Npc[5].IgnorePlayer = true;
            }

            if (ctx.Npc[6] == null)
            {
                ctx.Npc[6] = NPCSystem.CreateNpc(NPCSystem.NpcTypeD,
                    room.x - 3712f * rs, -0.3f, room.z - 2208f * rs);
                ctx.Npc[6].TextureId = 3;
                ctx.Npc[6].IgnorePlayer = true;
            }

            if (ctx.Npc[7] == null)
            {
                ctx.Npc[7] = NPCSystem.CreateNpc(NPCSystem.NpcTypeD,
                    room.x - 3712f * rs, -0.3f, room.z - 2208f * rs);
                ctx.Npc[7].Sfx = AudioSystem.Load("SFX/Room/Intro/Scientist/Conversation.ogg");
                ctx.Npc[7].TextureId = 2;
                ctx.Npc[7].IgnorePlayer = true;
            }

            for (int i = 8; i <= 10; i++)
            {
                if (ctx.Npc[i] != null) continue;
                float ex = i switch
                {
                    8 => room.x - 3800f * rs,
                    9 => room.x - 4000f * rs,
                    _ => room.x - 4200f * rs,
                };
                int type = i == 9 ? NPCSystem.NpcTypeD : NPCSystem.NpcTypeGuard;
                ctx.Npc[i] = NPCSystem.CreateNpc(type, ex, i == 9 ? 1.1f : 1f, room.z - 3900f * rs);
                ctx.Npc[i].State = 7f;
                ctx.Npc[i].IgnorePlayer = true;
                if (i == 9) ctx.Npc[i].State2 = 1f;
                RotateEntity(ctx.Npc[i].Collider, 0, 90f, 0);
            }

            if (NPCSystem.Curr173 != null && ctx.Objects[5] != -1)
            {
                PositionEntity(NPCSystem.Curr173.Collider,
                    EntityX(ctx.Objects[5], true), 0.5f, EntityZ(ctx.Objects[5], true));
                ResetEntity(NPCSystem.Curr173.Collider);
                NPCSystem.Curr173.Idle = true;
            }

            PositionEntity(GameState.Collider,
                room.x - (3072f + 1024f) * rs, 0.3f, room.z + 192f * rs);
            ResetEntity(GameState.Collider);

            e.EventState = 1f;
            e.EventState3 = 1f;
        }

        private static void Update173Escort(GameEvent e, EventRoomContext ctx, float rs)
        {
            var room = e.Room;
            var ulgrin = ctx.Npc[3];
            if (ulgrin != null)
            {
                float guardDist = EntityDistance(ulgrin.Collider, GameState.Collider);
                if (guardDist > 0.01f)
                {
                    float slow = GameState.CurrSpeed * (0.008f / guardDist) * GameState.FpsFactor;
                    GameState.CurrSpeed = Math.Max(GameState.CurrSpeed - slow, 0f);
                }
            }

            if (e.EventState3 < 170f)
                Update173CameraPhase(e, ctx, rs);
            else if (e.EventState3 < 700f)
                Update173CellPhase(e, ctx, rs);
            else if (e.EventState3 < 800f)
                e.EventState3 += GameState.FpsFactor / 4f;
            else if (e.EventState3 < 900f)
                Update173PaEscort(e, ctx, rs);
            else if (e.EventState3 <= 905f)
                Update173IncidentSpawn(e, ctx, rs);
            else if (e.EventState3 <= 910f)
                Update173DocHandoff(e, ctx, rs);
            else
                Update173Cleanup(e, ctx, rs);

            Update173DeskScientist(e, ctx, rs);
            Update173GuardSoundLoops(ctx);
        }

        private static void Update173CameraPhase(GameEvent e, EventRoomContext ctx, float rs)
        {
            var room = e.Room;
            float prev3 = _introCamPrev3;
            _introCamPrev3 = e.EventState3;

            if (e.EventState3 == 1f)
            {
                GameState.UnableToMove = true;
                var lightSfx = AudioSystem.Load("SFX/Room/Intro/Light2.ogg");
                lightSfx?.Play(GameState.SfxVolume, 0f, 0f);
                GameState.BlurTimer = 500;

                if (_introLight == -1)
                    _introLight = CreateLight(2);
                ShowEntity(_introLight);
                EntityAlpha(_introLight, 0.5f);
            }

            if (e.EventState3 < 3f)
                e.EventState3 += GameState.FpsFactor / 100f;
            else if (e.EventState3 < 15f || e.EventState3 >= 50f)
                e.EventState3 += GameState.FpsFactor / 30f;

            if (e.EventState3 < 15f)
            {
                float x = EntityX(room.obj) - (3224f + 1024f) * rs;
                float y = 136f * rs;
                float z = EntityZ(room.obj) + 8f * rs;

                if (e.EventState3 < 14f)
                {
                    if (prev3 < 12f && e.EventState3 >= 12f)
                    {
                        var step = AudioSystem.Load("SFX/Footstep/Concrete/Step1.ogg");
                        AudioSystem.PlaySound2(step, GameState.Camera, GameState.Collider, 8f, 0.3f);
                    }

                    if (_introLight != -1)
                    {
                        ShowEntity(_introLight);
                        EntityAlpha(_introLight, 0.9f - e.EventState3 / 2f);
                    }

                    x += (EntityX(room.obj) - (3048f + 1024f) * rs - x) * Math.Max((e.EventState3 - 10f) / 4f, 0f);

                    if (e.EventState3 < 10f)
                        y += 0.2f * Math.Min(Math.Max((e.EventState3 - 3f) / 5f, 0f), 1f);
                    else
                        y = (y + 0.2f) + (0.302f + 0.6f - (y + 0.2f)) * Math.Max((e.EventState3 - 10f) / 4f, 0f);

                    z += (EntityZ(room.obj) + 104f * rs - z) * Math.Min(Math.Max((e.EventState3 - 3f) / 5f, 0f), 1f);

                    float pitch = -70f + 70f * Math.Min(Math.Max((e.EventState3 - 3f) / 5f, 0f), 1f)
                        + Sin(e.EventState3 * 12.857f) * 5f;
                    float yaw = -60f * Math.Max((e.EventState3 - 10f) / 4f, 0f);
                    float roll = Sin(e.EventState3 * 25.7f) * 8f;

                    PositionEntity(GameState.Camera, x, y, z);
                    RotateEntity(GameState.Camera, pitch, yaw, roll);
                    HideEntity(GameState.Collider);
                    PositionEntity(GameState.Collider, x, 0.302f, z);
                    GameState.DropSpeed = 0f;
                }
                else
                {
                    if (_introLight != -1) HideEntity(_introLight);
                    PositionEntity(GameState.Collider, EntityX(GameState.Collider), 0.302f, EntityZ(GameState.Collider));
                    ResetEntity(GameState.Collider);
                    ShowEntity(GameState.Collider);
                    GameState.DropSpeed = 0f;
                    GameState.UnableToMove = false;
                    e.EventState3 = 15f;
                    GameState.Msg = "Pick up the paper on the desk.";
                    GameState.MsgTimer = 70f * 7f;
                }

                RotateEntity(GameState.Collider, 0, EntityYaw(GameState.Camera), 0);
            }
            else if (e.EventState3 < 40f)
            {
                if (ItemSystem.Inventory[0] != null)
                {
                    GameState.Msg = "Press Y to open the inventory.";
                    GameState.MsgTimer = 70f * 7f;
                    e.EventState3 = 40f;
                }
            }

            if (ItemSystem.SelectedItem != null)
                e.EventState3 += GameState.FpsFactor / 5f;
        }

        private static void Update173CellPhase(GameEvent e, EventRoomContext ctx, float rs)
        {
            var room = e.Room;
            var ulgrin = ctx.Npc[3];
            if (ulgrin == null) return;

            float prev3 = e.EventState3;

            if (ulgrin.State == 7f)
            {
                if (ulgrin.Sfx2 == null)
                {
                    ulgrin.Sfx2 = AudioSystem.Load("SFX/Room/Intro/Guard/Ulgrin/BeforeDoorOpen.ogg");
                    AudioSystem.PlaySound2(ulgrin.Sfx2, GameState.Camera, ulgrin.Collider);
                }

                ulgrin.State = 9f;
                if (ctx.Npc[4] != null) ctx.Npc[4].State = 9f;
                if (ctx.Npc[5] != null) ctx.Npc[5].State = 9f;

                var door6 = ctx.RoomDoors[6];
                if (door6 != null)
                {
                    door6.Locked = false;
                    DoorSystem.UseDoor(door6, false);
                    door6.Locked = true;
                }

                ulgrin.Sfx = AudioSystem.Load("SFX/Room/Intro/Guard/Ulgrin/ExitCell.ogg");
                AudioSystem.PlaySound2(ulgrin.Sfx, GameState.Camera, ulgrin.Collider);
            }
            else
            {
                float cellX = room.x - (3072f + 1024f) * rs;
                float cellZ = room.z + 192f * rs;
                float px = EntityX(GameState.Collider, true);
                float pz = EntityZ(GameState.Collider, true);

                if (MathUtil.PointDistance(px, pz, cellX, cellZ) > 1.5f)
                {
                    e.EventState3 = Math.Min(e.EventState3 + GameState.FpsFactor / 4f, 699f);

                    if (e.EventState3 > 250f)
                    {
                        ulgrin.Sfx = AudioSystem.Load("SFX/Room/Intro/Guard/Ulgrin/Escort" + Rand(1, 2) + ".ogg");
                        AudioSystem.PlaySound2(ulgrin.Sfx, GameState.Camera, ulgrin.Collider);

                        SetNpcPath(ctx.Npc[3], room.x - 320f * rs, 0.3f, room.z - 704f * rs);
                        SetNpcPath(ctx.Npc[4], room.x - 320f * rs, 0.3f, room.z - 704f * rs);
                        e.EventState3 = 710f;
                    }
                }
                else
                {
                    ulgrin.State = 9f;
                    e.EventState3 = Math.Min(e.EventState3 + GameState.FpsFactor / 4f, 699f);

                    if (prev3 < 350f && e.EventState3 >= 350f)
                    {
                        ulgrin.Sfx = AudioSystem.Load("SFX/Room/Intro/Guard/Ulgrin/ExitCellRefuse" + Rand(1, 2) + ".ogg");
                        AudioSystem.PlaySound2(ulgrin.Sfx, GameState.Camera, ulgrin.Collider);
                    }
                    else if (prev3 < 550f && e.EventState3 >= 550f)
                    {
                        ulgrin.Sfx = AudioSystem.Load("SFX/Room/Intro/Guard/Ulgrin/CellGas" + Rand(1, 2) + ".ogg");
                        AudioSystem.PlaySound2(ulgrin.Sfx, GameState.Camera, ulgrin.Collider);
                    }
                    else if (e.EventState3 > 630f)
                    {
                        float maxZ = EntityZ(room.obj, true) + 490f * rs;
                        PositionEntity(GameState.Collider, px, EntityY(GameState.Collider), Math.Min(pz, maxZ));

                        var door6 = ctx.RoomDoors[6];
                        if (door6 != null && door6.Open)
                        {
                            door6.Locked = false;
                            DoorSystem.UseDoor(door6, false);
                            door6.Locked = true;

                            SpawnGasEmitter(room, rs);
                        }

                        GameState.EyeIrritation = Math.Max(GameState.EyeIrritation + GameState.FpsFactor * 4f, 1f);
                    }
                }
            }

            TickIntroNpcPaths(ctx);
        }

        private static void SpawnGasEmitter(RoomInstance room, float rs)
        {
            var em = ParticleSystem.CreateEmitter(
                room.x - (2976f + 1024f) * rs, 373f * rs, room.z + 204f * rs, 0);
            TurnEntity(em.Obj, 90f, 0f, 0f, true);
            em.RandAngle = 7f;
            em.Speed = 0.03f;
            em.SizeChange = 0.003f;
            em.Room = room;

            em = ParticleSystem.CreateEmitter(
                room.x - (3168f + 1024f) * rs, 373f * rs, room.z + 204f * rs, 0);
            TurnEntity(em.Obj, 90f, 0f, 0f, true);
            em.RandAngle = 7f;
            em.Speed = 0.03f;
            em.SizeChange = 0.003f;
            em.Room = room;
        }

        private static void Update173PaEscort(GameEvent e, EventRoomContext ctx, float rs)
        {
            var room = e.Room;
            var ulgrin = ctx.Npc[3];
            if (ulgrin == null) return;

            e.EventState3 += GameState.FpsFactor / 4f;

            float px = EntityX(GameState.Collider, true);
            if (px < EntityX(room.obj, true) - 5376f * rs && string.IsNullOrEmpty(e.EventStr))
                e.EventStr = BuildIntroPaMessage();

            Update173ScientistDoor(e, ctx, rs);
            Update173LateGuards(e, ctx, rs);
            PlayIntroPaSequence(e, ctx);

            float dist = MathUtil.PointDistance(px, EntityZ(GameState.Collider, true),
                EntityX(ulgrin.Collider, true), EntityZ(ulgrin.Collider, true));

            if (dist < 3f)
                ulgrin.State3 = Math.Min(Math.Max(ulgrin.State3 - GameState.FpsFactor, 0f), 50f);
            else
            {
                ulgrin.State3 = Math.Max(ulgrin.State3 + GameState.FpsFactor, 50f);
                if (ulgrin.State3 >= 70f * 8f && ulgrin.State3 - GameState.FpsFactor < 70f * 8f && ulgrin.State == 7f)
                    HandleUlgrinEscortRefuse(ctx);
            }

            if (ulgrin.State != 11f)
                Update173EscortMovement(e, ctx, dist, rs);

            if (ctx.Npc[5] != null && ctx.Npc[5].State == 11f)
                AudioSystem.UpdateSoundOrigin(ctx.Npc[5].SoundChn2, GameState.Camera, ctx.Npc[5].Collider);

            TrySpawn173Incident(e, ctx, rs);
            TickIntroNpcPaths(ctx);
        }

        private static void Update173IncidentSpawn(GameEvent e, EventRoomContext ctx, float rs)
        {
            // Handled in TrySpawn173Incident when thresholds met; advance timer here.
            e.EventState3 = Math.Max(e.EventState3, 905f);
        }

        private static void TrySpawn173Incident(GameEvent e, EventRoomContext ctx, float rs)
        {
            var room = e.Room;
            var door2 = ctx.RoomDoors[2];
            var ulgrin = ctx.Npc[3];
            if (door2 == null || ulgrin == null || e.EventState3 >= 905f) return;

            float dist = MathUtil.PointDistance(
                EntityX(GameState.Collider, true), EntityZ(GameState.Collider, true),
                EntityX(door2.FrameObj, true), EntityZ(door2.FrameObj, true));

            if (MathUtil.PointDistance(EntityX(ulgrin.Collider, true), EntityZ(ulgrin.Collider, true),
                    EntityX(door2.FrameObj, true), EntityZ(door2.FrameObj, true)) >= 4.5f || dist >= 5f)
                return;

            if (ctx.Npc[0] == null && ctx.Objects[0] != -1)
            {
                ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard,
                    EntityX(ctx.Objects[0], true), EntityY(ctx.Objects[0], true), EntityZ(ctx.Objects[0], true));
                ctx.Npc[0].Angle = 180f;
            }

            if (ctx.Npc[1] == null && ctx.Objects[1] != -1)
            {
                ctx.Npc[1] = NPCSystem.CreateNpc(NPCSystem.NpcTypeD,
                    EntityX(ctx.Objects[1], true), 0.5f, EntityZ(ctx.Objects[1], true));
                if (ctx.Objects[5] != -1)
                    PointEntity(ctx.Npc[1].Collider, ctx.Objects[5]);
            }

            if (ctx.Npc[2] == null && ctx.Objects[2] != -1)
            {
                ctx.Npc[2] = NPCSystem.CreateNpc(NPCSystem.NpcTypeD,
                    EntityX(ctx.Objects[2], true), 0.5f, EntityZ(ctx.Objects[2], true));
                if (ctx.Objects[5] != -1)
                    PointEntity(ctx.Npc[2].Collider, ctx.Objects[5]);
                ctx.Npc[2].TextureId = 6;
            }

            ulgrin.State = 9f;

            if (ctx.Objects[9] != -1) { FreeEntity(ctx.Objects[9]); ctx.Objects[9] = -1; }
            if (ctx.Objects[10] != -1) { FreeEntity(ctx.Objects[10]); ctx.Objects[10] = -1; }

            if (ctx.Npc[5] != null) NPCSystem.Remove(ctx.Npc[5]);
            for (int i = 8; i <= 10; i++)
                if (ctx.Npc[i] != null) NPCSystem.Remove(ctx.Npc[i]);

            ulgrin.Sfx = AudioSystem.Load("SFX/Room/Intro/Guard/Ulgrin/EscortDone" + Rand(1, 5) + ".ogg");
            AudioSystem.PlaySound2(ulgrin.Sfx, GameState.Camera, ulgrin.Collider);

            if (ctx.Npc[6] != null)
            {
                PositionEntity(ctx.Npc[6].Collider,
                    EntityX(room.obj, true) - 1190f * rs, 450f * rs, EntityZ(room.obj, true) + 456f * rs, true);
                ResetEntity(ctx.Npc[6].Collider);
                PointEntity(ctx.Npc[6].Collider, room.obj);
                ctx.Npc[6].CurrSpeed = 0;
                ctx.Npc[6].State = 0f;
            }

            e.EventState3 = 905f;

            var door3 = ctx.RoomDoors[3];
            if (door3 != null)
            {
                door3.Locked = false;
                DoorSystem.UseDoor(door3, false);
                door3.Locked = true;
            }

            if (ctx.Npc[4] != null) ctx.Npc[4].State = 9f;
        }

        private static void Update173DocHandoff(GameEvent e, EventRoomContext ctx, float rs)
        {
            var ulgrin = ctx.Npc[3];
            if (ulgrin == null) return;

            _introDocHandoff = false;

            if (ulgrin.Frame < 358f)
            {
                ulgrin.State = 8f;
                ulgrin.Sfx = AudioSystem.Load("SFX/Room/Intro/Guard/Ulgrin/OhAndByTheWay.ogg");
                AudioSystem.PlaySound2(ulgrin.Sfx, GameState.Camera, ulgrin.Collider);
                ulgrin.Frame = 358f;
            }
            else if (ulgrin.Frame >= 358f)
            {
                PointEntity(ulgrin.Collider, GameState.Collider);
                RotateEntity(ulgrin.Collider, 0, EntityYaw(ulgrin.Collider), 0);

                if (ulgrin.Frame <= 481.5f)
                    ulgrin.Frame = Math.Min(ulgrin.Frame + 0.4f * GameState.FpsFactor, 482f);
                else
                {
                    ulgrin.Frame = Math.Min(ulgrin.Frame + 0.2f * GameState.FpsFactor, 607f);

                    if (EntityDistance(GameState.Collider, ulgrin.Collider) < 1.5f
                        && EntityVisible(ulgrin.Obj, GameState.Camera))
                    {
                        GameState.DrawHandIcon = true;
                        _introDocHandoff = true;
                    }
                }
            }
        }

        public static void CompleteIntroDocHandoff(GameEvent e)
        {
            var ctx = GetContext(e.Room);
            var ulgrin = ctx.Npc[3];
            if (ulgrin == null) return;

            var doc = ItemSystem.CreateItem("Document SCP-173", "paper", 0f, 0f, 0f);
            ItemSystem.PickItem(doc);

            var door2 = ctx.RoomDoors[2];
            if (door2 != null)
            {
                door2.Locked = false;
                DoorSystem.UseDoor(door2, false);
                door2.Locked = true;
            }

            e.EventState3 = 910f;
            ulgrin.Frame = 608f;
            _introDocHandoff = false;
            GameState.DrawHandIcon = false;
        }

        private static void Update173Cleanup(GameEvent e, EventRoomContext ctx, float rs)
        {
            var ulgrin = ctx.Npc[3];
            if (ulgrin == null) return;

            if (ulgrin.Frame <= 620.5f && ulgrin.State == 8f)
                ulgrin.Frame = Math.Min(ulgrin.Frame + 0.4f * GameState.FpsFactor, 621f);
            else
            {
                ulgrin.Angle = EntityYaw(ulgrin.Collider);
                ulgrin.State = 9f;
                if (ctx.Npc[4] != null) ctx.Npc[4].State = 9f;

                var room = e.Room;
                if (MathUtil.PointDistance(EntityX(GameState.Collider, true), EntityZ(GameState.Collider, true),
                        room.x, room.z) < 4f)
                {
                    var door2 = ctx.RoomDoors[2];
                    if (door2 != null)
                    {
                        door2.Locked = false;
                        DoorSystem.UseDoor(door2, false);
                        door2.Locked = true;
                    }

                    e.EventState3 = 0f;
                    ulgrin.State = 0f;
                    if (ctx.Npc[4] != null) ctx.Npc[4].State = 0f;

                    var door1 = ctx.RoomDoors[1];
                    if (door1 != null) DoorSystem.UseDoor(door1, false);

                    MusicSystem.ClearForcedTrack();
                    GameState.UnableToMove = false;
                }
            }
        }

        private static void Update173DeskScientist(GameEvent e, EventRoomContext ctx, float rs)
        {
            var deskSci = ctx.Npc[7];
            if (deskSci == null) return;

            var room = e.Room;
            RotateEntity(deskSci.Collider, 0, 180f + Sin(MilliSecs() / 20f) * 3f, 0, true);
            PositionEntity(deskSci.Collider,
                EntityX(room.obj, true) - 3361f * rs, -315f * rs, EntityZ(room.obj, true) - 2165f * rs);
            ResetEntity(deskSci.Collider);
            deskSci.State = 6f;
            deskSci.Frame = 182f;

            if (ctx.Npc[6] != null && ctx.Npc[6].State == 1f && deskSci.Sfx != null)
                deskSci.SoundChn = AudioSystem.LoopSound2(deskSci.Sfx, deskSci.SoundChn,
                    GameState.Camera, deskSci.Collider, 7f);
        }

        private static void Update173GuardSoundLoops(EventRoomContext ctx)
        {
            for (int i = 3; i <= 4; i++)
            {
                var n = ctx.Npc[i];
                if (n == null || n.Sfx == null) continue;
                n.SoundChn = AudioSystem.LoopSound2(n.Sfx, n.SoundChn, GameState.Camera, n.Collider);
            }
        }

        private static void Update173ScientistDoor(GameEvent e, EventRoomContext ctx, float rs)
        {
            var sci = ctx.Npc[6];
            var door7 = ctx.RoomDoors[7];
            if (sci == null || door7 == null) return;

            var room = e.Room;
            if (sci.State == 0f)
            {
                if (door7.Open)
                {
                    float dx = EntityX(room.obj, true) - 3328f * rs;
                    float dz = EntityZ(room.obj, true) - 1232f * rs;
                    if (MathUtil.PointDistance(EntityX(GameState.Collider, true), EntityZ(GameState.Collider, true), dx, dz) < 5f)
                    {
                        sci.State = 1f;
                        if (e.EventStr == "done")
                        {
                            var sfx = AudioSystem.Load("SFX/Room/Intro/PA/scripted/announcement" + Rand(1, 7) + ".ogg");
                            sfx?.Play(GameState.SfxVolume, 0f, 0f);
                        }
                    }
                }
            }
            else if (EntityZ(sci.Collider, true) > EntityZ(room.obj, true) - 64f * rs)
            {
                RotateEntity(sci.Collider, 0, MathUtil.CurveAngle(90f, EntityYaw(sci.Collider), 15f), 0);
                if (door7.Open) DoorSystem.UseDoor(door7, false);
                if (door7.OpenState < 1f) sci.State = 0f;
            }
        }

        private static void Update173LateGuards(GameEvent e, EventRoomContext ctx, float rs)
        {
            var room = e.Room;
            if (ctx.Npc[8] == null) return;

            float dx = EntityX(room.obj, true) - 6688f * rs;
            float dz = EntityZ(room.obj, true) - 1252f * rs;

            if (ctx.Npc[8].State == 7f)
            {
                if (MathUtil.PointDistance(EntityX(GameState.Collider, true), EntityZ(GameState.Collider, true), dx, dz) < 2.5f)
                {
                    ctx.Npc[8].State = 10f;
                    if (ctx.Npc[9] != null) ctx.Npc[9].State = 1f;
                    if (ctx.Npc[10] != null) ctx.Npc[10].State = 10f;
                }
            }
            else if (EntityX(ctx.Npc[8].Collider, true) < EntityX(room.obj, true) - 7100f * rs)
            {
                for (int i = 8; i <= 10; i++)
                    if (ctx.Npc[i] != null) ctx.Npc[i].State = 0f;
            }
        }

        private static void Update173EscortMovement(GameEvent e, EventRoomContext ctx, float dist, float rs)
        {
            var ulgrin = ctx.Npc[3];
            var guard2 = ctx.Npc[4];
            if (ulgrin == null) return;

            float followDist = Math.Min(Math.Max(4f - ulgrin.State3 * 0.05f, 1.5f), 4f);

            if (dist < followDist)
            {
                if (ulgrin.PathStatus != 1)
                {
                    ulgrin.State = 7f;
                    PointEntity(ulgrin.Obj, GameState.Collider);
                    RotateEntity(ulgrin.Collider, 0,
                        MathUtil.CurveValue(EntityYaw(ulgrin.Obj), EntityYaw(ulgrin.Collider), 20f), 0, true);

                    if (ulgrin.PathStatus == 2)
                    {
                        SetNpcPath(ulgrin, e.Room.x - 320f * rs, 0.3f, e.Room.z - 704f * rs);
                        if (guard2 != null)
                            SetNpcPath(guard2, e.Room.x - 320f * rs, 0.3f, e.Room.z - 704f * rs);
                        ulgrin.State = 3f;
                    }
                }
                else
                {
                    ulgrin.State = 3f;
                }
            }
            else
            {
                ulgrin.State = 7f;
                PointEntity(ulgrin.Obj, GameState.Collider);
                RotateEntity(ulgrin.Collider, 0,
                    MathUtil.CurveValue(EntityYaw(ulgrin.Obj), EntityYaw(ulgrin.Collider), 20f), 0, true);

                if (dist > 5.5f)
                {
                    ulgrin.PathStatus = 2;
                    if (ulgrin.State2 == 0f)
                    {
                        ulgrin.Sfx = AudioSystem.Load("SFX/Room/Intro/Guard/Ulgrin/EscortRun.ogg");
                        AudioSystem.PlaySound2(ulgrin.Sfx, GameState.Camera, ulgrin.Collider);
                        ulgrin.State2 = 1f;
                    }

                    ulgrin.State = 5f;
                    ulgrin.EnemyX = EntityX(GameState.Collider, true);
                    ulgrin.EnemyY = EntityY(GameState.Collider, true);
                    ulgrin.EnemyZ = EntityZ(GameState.Collider, true);
                }
            }

            if (guard2 != null)
            {
                float d4 = EntityDistance(GameState.Collider, guard2.Collider);
                if (d4 > 1.5f && EntityDistance(ulgrin.Collider, GameState.Collider)
                    < EntityDistance(ulgrin.Collider, guard2.Collider))
                    guard2.State = 3f;
                else
                {
                    guard2.State = 5f;
                    guard2.EnemyX = EntityX(GameState.Collider, true);
                    guard2.EnemyY = EntityY(GameState.Collider, true);
                    guard2.EnemyZ = EntityZ(GameState.Collider, true);
                }
            }

            TickIntroChaseNpcs(ctx);
            CheckEscortShootState(ctx);
        }

        private static void CheckEscortShootState(EventRoomContext ctx)
        {
            var guard5 = ctx.Npc[5];
            if (guard5 == null || guard5.State == 11f) return;

            var ulgrin = ctx.Npc[3];
            var guard4 = ctx.Npc[4];
            if (ulgrin == null) return;

            if (EntityDistance(ulgrin.Collider, guard5.Collider) > 5f
                && (guard4 == null || EntityDistance(guard4.Collider, guard5.Collider) > 5f)
                && EntityDistance(guard5.Collider, GameState.Collider) < 3.5f)
            {
                guard5.State = 11f;
                guard5.State3 = 1f;
                if (guard5.Sfx2 != null)
                    guard5.SoundChn2 = AudioSystem.LoopSound2(guard5.Sfx2, guard5.SoundChn2,
                        GameState.Camera, guard5.Collider);
                guard5.Reload = (int)(70f * 3f);
            }
        }

        private static void HandleUlgrinEscortRefuse(EventRoomContext ctx)
        {
            var ulgrin = ctx.Npc[3];
            if (ulgrin == null) return;

            if (ulgrin.State2 < 2f)
            {
                ulgrin.Sfx = AudioSystem.Load("SFX/Room/Intro/Guard/Ulgrin/EscortRefuse" + Rand(1, 2) + ".ogg");
                AudioSystem.PlaySound2(ulgrin.Sfx, GameState.Camera, ulgrin.Collider);
                ulgrin.State3 = 50f;
                ulgrin.State2 = 3f;
            }
            else if (ulgrin.State2 == 3f)
            {
                ulgrin.Sfx = AudioSystem.Load("SFX/Room/Intro/Guard/Ulgrin/EscortPissedOff" + Rand(1, 2) + ".ogg");
                AudioSystem.PlaySound2(ulgrin.Sfx, GameState.Camera, ulgrin.Collider);
                ulgrin.State3 = 50f;
                ulgrin.State2 = 4f;
            }
            else if (ulgrin.State2 == 4f)
            {
                ulgrin.Sfx = AudioSystem.Load("SFX/Room/Intro/Guard/Ulgrin/EscortKill" + Rand(1, 2) + ".ogg");
                AudioSystem.PlaySound2(ulgrin.Sfx, GameState.Camera, ulgrin.Collider);
                ulgrin.State3 = 50f + 70f * 2.5f;
                ulgrin.State2 = 5f;
            }
            else if (ulgrin.State2 == 5f)
            {
                ulgrin.State = 11f;
                if (ctx.Npc[4] != null) ctx.Npc[4].State = 11f;
                if (ctx.Npc[5] != null) ctx.Npc[5].State = 11f;
                ulgrin.State3 = 1f;
                if (ctx.Npc[4] != null) ctx.Npc[4].State3 = 1f;
                if (ctx.Npc[5] != null) ctx.Npc[5].State3 = 1f;
            }
        }

        private static string BuildIntroPaMessage()
        {
            if (Rand(3) == 1)
                return "scripted/scripted" + Rand(1, 5) + ".ogg|off.ogg|";

            string role = Rand(3) switch
            {
                1 => "crew",
                2 => "scientist",
                _ => "security",
            };

            string msg = "1/attention" + Rand(1, 2) + ".ogg|2/" + role + Rand(0, role == "scientist" ? 19 : 5) + ".ogg";

            if (Rand(2) == 1 && role == "scientist")
            {
                msg += "|3/callonline.ogg|numbers/" + Rand(1, 9) + ".ogg";
                if (Rand(2) == 1) msg += "|numbers/" + Rand(1, 9) + ".ogg";
            }
            else
            {
                msg += "|3/report" + Rand(0, 1) + ".ogg|4/" + role + Rand(0, role == "scientist" ? 7 : 6) + ".ogg";
            }

            return msg + "|off.ogg|";
        }

        private static void PlayIntroPaSequence(GameEvent e, EventRoomContext ctx)
        {
            if (string.IsNullOrEmpty(e.EventStr) || e.EventStr == "done") return;

            int pipe = e.EventStr.IndexOf('|');
            if (pipe < 0) return;

            string clip = e.EventStr[..pipe];
            var sfx = AudioSystem.Load("SFX/Room/Intro/PA/" + clip);
            sfx?.Play(GameState.SfxVolume, 0f, 0f);

            e.EventStr = e.EventStr[(pipe + 1)..];
            if (e.EventStr.Length == 0)
            {
                var ulgrin = ctx.Npc[3];
                var guard4 = ctx.Npc[4];
                int temp = Rand(1, 5);
                if (ulgrin != null)
                {
                    ulgrin.Sfx = AudioSystem.Load("SFX/Room/Intro/Guard/Conversation" + temp + "a.ogg");
                    AudioSystem.PlaySound2(ulgrin.Sfx, GameState.Camera, ulgrin.Collider);
                }
                if (guard4 != null)
                {
                    guard4.Sfx = AudioSystem.Load("SFX/Room/Intro/Guard/Conversation" + temp + "b.ogg");
                    AudioSystem.PlaySound2(guard4.Sfx, GameState.Camera, guard4.Collider);
                }
                e.EventStr = "done";
            }
        }

        private static void SetNpcPath(NPC n, float x, float y, float z)
        {
            if (n == null) return;
            n.PathX = x;
            n.PathY = y;
            n.PathZ = z;
            n.PathStatus = 1;
            n.State = 3f;
        }

        private static void TickIntroNpcPaths(EventRoomContext ctx)
        {
            for (int i = 3; i <= 4; i++)
            {
                var n = ctx.Npc[i];
                if (n == null || n.PathStatus != 1 || n.State != 3f) continue;
                MoveNpcToward(n, n.PathX, n.PathY, n.PathZ, 0.02f);
            }
        }

        private static void TickIntroChaseNpcs(EventRoomContext ctx)
        {
            for (int i = 3; i <= 4; i++)
            {
                var n = ctx.Npc[i];
                if (n == null || n.State != 5f) continue;
                MoveNpcToward(n, n.EnemyX, n.EnemyY, n.EnemyZ, 0.025f);
            }
        }

        private static void MoveNpcToward(NPC n, float x, float y, float z, float speed)
        {
            float dx = x - EntityX(n.Collider, true);
            float dy = y - EntityY(n.Collider, true);
            float dz = z - EntityZ(n.Collider, true);
            float dist = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            if (dist < 0.05f)
            {
                n.PathStatus = 0;
                return;
            }

            float step = speed * GameState.FpsFactor;
            float scale = step / dist;
            MoveEntity(n.Collider, dx * scale, dy * scale, dz * scale);
            PointEntity(n.Obj, GameState.Collider);
            RotateEntity(n.Collider, 0, MathUtil.CurveValue(EntityYaw(n.Obj), EntityYaw(n.Collider), 15f), 0);
        }

        private static int MilliSecs() => Environment.TickCount;
    }
}