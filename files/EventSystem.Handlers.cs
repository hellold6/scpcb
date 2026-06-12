// EventSystem.Handlers.cs — ports remaining UpdateEvents.bb handlers

using System;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static partial class EventSystem
    {
        private static bool PassedThreshold(float state, float threshold) =>
            state >= threshold && state - GameState.FpsFactor < threshold;

        private static void MoveToPocketDimension()
        {
            var pd = FindRoomByName("pocketdimension");
            if (pd == null || GameState.Collider == -1) return;
            GameState.PlayerRoom = pd;
            PositionEntity(GameState.Collider, pd.x, 0.5f, pd.z, true);
            ResetEntity(GameState.Collider);
            GameState.DropSpeed = 0f;
        }

        private static float UpdateElevatorsLocal(float state, Door d0, Door d1, int r0, int r1, GameEvent e, bool ignoreRot = true) =>
            EventElevatorHelper.UpdateElevators(state, d0, d1, r0, r1, e, ignoreRot);

        // ── endroom106 ──────────────────────────────────────────────────────────

        private static void UpdateEndroom106(GameEvent e)
        {
            if (Contained106 || e.Room == null) return;
            var ctx = GetContext(e.Room);

            if (e.EventState == 0f)
            {
                if (GetRoomDist(e.Room) >= 8f || GetRoomDist(e.Room) <= 0f) return;
                if (NPCSystem.Curr106 != null && NPCSystem.Curr106.State < 0f) { RemoveEvent(e); return; }

                if (ctx.RoomDoors[0] != null) ctx.RoomDoors[0].Open = true;
                ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeD,
                    ctx.RoomDoors[0] != null ? EntityX(ctx.RoomDoors[0].Obj, true) : e.Room.x,
                    0.5f,
                    ctx.RoomDoors[0] != null ? EntityZ(ctx.RoomDoors[0].Obj, true) : e.Room.z);
                if (ctx.RoomDoors[0] != null) ctx.RoomDoors[0].Open = false;
                AudioSystem.PlaySound2(AudioSystem.Load("SFX/Door/EndroomDoor.ogg"), GameState.Camera, e.Room.obj);
                e.EventState = 1f;
            }
            else if (e.EventState == 1f)
            {
                if (InPlayerRoom(e))
                {
                    if (ctx.Npc[0] != null) ctx.Npc[0].State = 1f;
                    e.EventState = 2f;
                    AudioSystem.Load("SFX/Character/Janitor/106Abduct.ogg")?.Play(GameState.SfxVolume, 0f, 0f);
                }
            }
            else if (e.EventState == 2f)
            {
                if (ctx.Npc[0] == null) return;
                if (EntityDistance(ctx.Npc[0].Collider, e.Room.obj) < 1.5f)
                {
                    var de = DecalSystem.Create(0, EntityX(e.Room.obj, true), 0.01f, EntityZ(e.Room.obj, true), 90f, Rand(0, 360), 0f);
                    de.Size = 0.05f; de.SizeChange = 0.008f; de.Timer = 10000f;
                    e.EventState = 3f;
                }
            }
            else
            {
                if (ctx.Npc[0] == null || NPCSystem.Curr106 == null) return;
                float dist = MathUtil.PointDistance(
                    EntityX(ctx.Npc[0].Collider, true), EntityZ(ctx.Npc[0].Collider, true),
                    EntityX(e.Room.obj, true), EntityZ(e.Room.obj, true));

                PositionEntity(NPCSystem.Curr106.Collider, EntityX(e.Room.obj, true), 0f, EntityZ(e.Room.obj, true));
                PointEntity(NPCSystem.Curr106.Collider, ctx.Npc[0].Collider);
                NPCSystem.Curr106.Idle = true;

                if (dist < 0.4f)
                {
                    Advance(e, 0.5f);
                    ctx.Npc[0].State = 6f;
                    PositionEntity(ctx.Npc[0].Collider,
                        MathUtil.CurveValue(EntityX(e.Room.obj, true), EntityX(ctx.Npc[0].Collider, true), 25f),
                        0.3f - e.EventState / 70f,
                        MathUtil.CurveValue(EntityZ(e.Room.obj, true), EntityZ(ctx.Npc[0].Collider, true), 25f));
                }

                if (e.EventState > 100f)
                {
                    PositionEntity(NPCSystem.Curr106.Collider, EntityX(NPCSystem.Curr106.Collider), -100f, EntityZ(NPCSystem.Curr106.Collider), true);
                    NPCSystem.Curr106.Idle = false;
                    if (EntityDistance(GameState.Collider, e.Room.obj) < 2.5f)
                        NPCSystem.Curr106.State = -0.1f;
                    if (ctx.Npc[0] != null) NPCSystem.Remove(ctx.Npc[0]);
                    RemoveEvent(e);
                }
            }
        }

        // ── room2cafeteria ──────────────────────────────────────────────────────

        private static void UpdateRoom2Cafeteria(GameEvent e)
        {
            if (e.Room == null) return;
            var ctx = GetContext(e.Room);

            if (InPlayerRoom(e) && !GameState.Using294 && ctx.Objects[0] != -1 &&
                EntityDistance(ctx.Objects[0], GameState.Collider) < 1.5f &&
                EntityVisible(ctx.Objects[0], GameState.Camera))
            {
                GameState.DrawHandIcon = true;
            }

            if (e.EventState == 0f)
            {
                NPCSystem.CreateNpc(NPCSystem.NpcType066, EntityX(e.Room.obj, true), 0.5f, EntityZ(e.Room.obj, true));
                e.EventState = 1f;
            }
        }

        // ── room2ccont ──────────────────────────────────────────────────────────

        private static void UpdateRoom2Ccont(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);

            if (ctx.Objects[3] != -1 && EntityDistance(GameState.Camera, ctx.Objects[3]) < 1.5f && e.EventState == 0f)
            {
                e.EventState = 1f;
                AudioSystem.Load("SFX/General/Horror7.ogg")?.Play(GameState.SfxVolume, 0f, 0f);
            }

            EventLeverHelper.UpdateLever(ctx.Objects[1]);
            float prev2 = e.EventState2;
            e.EventState2 = EventLeverHelper.UpdateLever(ctx.Objects[3]) ? 1f : 0f;
            if (prev2 != e.EventState2 && e.EventState > 0f)
                AudioSystem.PlaySound2(AudioSystem.Load("SFX/Door/LightSwitch.ogg"), GameState.Camera, ctx.Objects[3]);

            GameState.SecondaryLightOn = e.EventState2 > 0f
                ? MathUtil.CurveValue(1f, GameState.SecondaryLightOn, 10f)
                : MathUtil.CurveValue(0f, GameState.SecondaryLightOn, 10f);

            RemoteDoorOn = EventLeverHelper.UpdateLever(ctx.Objects[5]);

            if (e.EventState > 0f && e.EventState < 200f)
            {
                Advance(e);
                if (ctx.Objects[3] != -1)
                    RotateEntity(ctx.Objects[3], MathUtil.CurveValue(-85f, EntityPitch(ctx.Objects[3]), 5f), EntityYaw(ctx.Objects[3]), 0f);
            }
        }

        // ── room2closets ──────────────────────────────────────────────────────

        private static void UpdateRoom2Closets(GameEvent e)
        {
            if (e.Room == null || NPCSystem.Curr173 == null) return;
            var ctx = GetContext(e.Room);

            if (e.EventState == 0f)
            {
                if (InPlayerRoom(e) && NPCSystem.Curr173.Idle)
                    e.EventState = 0.1f;
                return;
            }

            Advance(e);
            if (ctx.Npc[0] != null) ctx.Npc[0].State = e.EventState < 70f * 3.5f ? 1f : 0f;

            if (e.EventState > 70f * 7.5f && PassedThreshold(e.EventState, 70f * 7.5f))
            {
                GameState.BlinkTimer = -10f;
                AudioSystem.PlaySound2(AudioSystem.Load("SFX/Character/NeckSnap0.ogg"), GameState.Camera, ctx.Npc[0]?.Collider ?? e.Room.obj);
            }

            if (e.EventState > 70f * 8.5f && NPCSystem.Curr173 != null &&
                ctx.Objects[0] != -1 && ctx.Objects[1] != -1)
            {
                PositionEntity(NPCSystem.Curr173.Collider,
                    (EntityX(ctx.Objects[0], true) + EntityX(ctx.Objects[1], true)) / 2f,
                    EntityY(ctx.Objects[0], true),
                    (EntityZ(ctx.Objects[0], true) + EntityZ(ctx.Objects[1], true)) / 2f);
                PointEntity(NPCSystem.Curr173.Collider, GameState.Collider);
                ResetEntity(NPCSystem.Curr173.Collider);
                RemoveEvent(e);
            }
        }

        // ── room2offices2 / room2offices3 ───────────────────────────────────────

        private static void UpdateRoom2Offices2(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);
            if (GameState.BlinkTimer < -8f && GameState.BlinkTimer > -12f && ctx.Objects[0] != -1)
            {
                int t = Rand(1, 4);
                if (ctx.Objects[t] != -1)
                {
                    PositionEntity(ctx.Objects[0],
                        EntityX(ctx.Objects[t], true), EntityY(ctx.Objects[t], true), EntityZ(ctx.Objects[t], true), true);
                    RotateEntity(ctx.Objects[0], 0f, Rand(0, 360), 0f);
                }
            }
        }

        private static void UpdateRoom2Offices3(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);
            Advance(e);
            if (e.EventState > 700f && ctx.RoomDoors[0] != null &&
                EntityDistance(ctx.RoomDoors[0].Obj, GameState.Collider) > 0.5f &&
                !EntityVisible(ctx.RoomDoors[0].Obj, GameState.Camera))
            {
                ctx.RoomDoors[0].Open = false;
                RemoveEvent(e);
            }
        }

        // ── room2trick ────────────────────────────────────────────────────────

        private static void UpdateRoom2Trick(GameEvent e)
        {
            if (!InPlayerRoom(e) || EntityDistance(e.Room.obj, GameState.Collider) >= 2f) return;

            bool nearThreat = (NPCSystem.Curr173 != null && EntityDistance(GameState.Collider, NPCSystem.Curr173.Obj) < 6f) ||
                              (NPCSystem.Curr106 != null && EntityDistance(GameState.Collider, NPCSystem.Curr106.Obj) < 6f);
            if (nearThreat) { RemoveEvent(e); return; }

            int pvt = CreatePivot();
            PositionEntity(pvt, EntityX(GameState.Collider, true), EntityY(GameState.Collider, true), EntityZ(GameState.Collider, true));
            PointEntity(pvt, e.Room.obj);
            MoveEntity(pvt, 0f, 0f, EntityDistance(pvt, e.Room.obj) * 2f);
            GameState.BlinkTimer = -10f;
            AudioSystem.Load("SFX/General/Horror11.ogg")?.Play(GameState.SfxVolume, 0f, 0f);
            PositionEntity(GameState.Collider, EntityX(pvt, true), EntityY(pvt, true) + 0.05f, EntityZ(pvt, true));
            TurnEntity(GameState.Collider, 0f, 180f, 0f);
            FreeEntity(pvt);
            RemoveEvent(e);
        }

        // ── room2tunnel ───────────────────────────────────────────────────────

        private static void UpdateRoom2Tunnel(GameEvent e)
        {
            if (e.Room == null) return;
            MaintenanceTunnelSystem.EnsureGrid(e.Room);

            if (MaintenanceTunnelSystem.PlayerInTunnelBounds(e.Room))
                GameState.PlayerRoom = e.Room;

            if (!InPlayerRoom(e))
            {
                MaintenanceTunnelSystem.SetTunnelVisible(e.Room, false);
                return;
            }

            float py = EntityY(GameState.Collider, true);
            if (py > 4f)
            {
                MaintenanceTunnelSystem.SetTunnelVisible(e.Room, true);

                if (e.EventState == 0f && e.Room.Objects[0] != -1 && e.Room.Objects[1] != -1)
                {
                    int end = EntityDistance(GameState.Collider, e.Room.Objects[0]) <
                              EntityDistance(GameState.Collider, e.Room.Objects[1]) ? 0 : 1;
                    e.EventState = 2f;

                    if (!Contained106 && NPCSystem.Curr106 != null)
                    {
                        int obj = end == 0 ? e.Room.Objects[0] : e.Room.Objects[1];
                        PositionEntity(NPCSystem.Curr106.Collider,
                            EntityX(obj, true), py - 3f, EntityZ(obj, true));
                        NPCSystem.Curr106.State = -0.1f;
                        NPCSystem.Curr106.PrevY = py;
                    }

                    var ctx = GetContext(e.Room);
                    for (int i = 0; i < 2; i++)
                    {
                        if (Rand(0, 2) == 1 && e.Room.TunnelGrid != null)
                        {
                            int idx = Rand(i * 72, e.Room.TunnelGrid.Entities.Length - 1);
                            if (e.Room.TunnelGrid.Entities[idx] != -1)
                            {
                                ctx.Npc[i] = NPCSystem.CreateNpc(NPCSystem.NpcType966,
                                    EntityX(e.Room.TunnelGrid.Entities[idx], true),
                                    EntityY(e.Room.TunnelGrid.Entities[idx], true),
                                    EntityZ(e.Room.TunnelGrid.Entities[idx], true));
                            }
                        }
                    }
                }
            }
            else
            {
                MaintenanceTunnelSystem.SetTunnelVisible(e.Room, false);
            }

            var c = GetContext(e.Room);
            e.EventState2 = UpdateElevatorsLocal(e.EventState2, c.RoomDoors[0], c.RoomDoors[1], e.Room.Objects[2], e.Room.Objects[3], e, false);
            e.EventState3 = UpdateElevatorsLocal(e.EventState3, c.RoomDoors[2], c.RoomDoors[3], e.Room.Objects[4], e.Room.Objects[5], e, false);
        }

        // ── 106 pipe/pit events ───────────────────────────────────────────────

        private static void UpdateRoom2Pipes106(GameEvent e)
        {
            if (Contained106 || NPCSystem.Curr106 == null) return;
            var ctx = GetContext(e.Room);
            if (e.EventState == 0f) { if (InPlayerRoom(e)) e.EventState = 1f; return; }

            Advance(e, 0.7f);
            if (e.EventState < 50f && ctx.Objects[0] != -1 && ctx.Objects[1] != -1)
            {
                NPCSystem.Curr106.Idle = true;
                PositionEntity(NPCSystem.Curr106.Collider,
                    EntityX(ctx.Objects[0], true), EntityY(GameState.Collider) - 0.15f, EntityZ(ctx.Objects[0], true));
                PointEntity(NPCSystem.Curr106.Collider, ctx.Objects[1]);
                MoveEntity(NPCSystem.Curr106.Collider, 0f, 0f,
                    EntityDistance(ctx.Objects[0], ctx.Objects[1]) * 0.5f * (e.EventState / 50f));
            }
            else if (e.EventState < 200f)
            {
                NPCSystem.Curr106.Idle = true;
                if (ctx.Objects[0] != -1 && ctx.Objects[1] != -1)
                    PositionEntity(NPCSystem.Curr106.Collider,
                        (EntityX(ctx.Objects[0], true) + EntityX(ctx.Objects[1], true)) / 2f,
                        EntityY(GameState.Collider) - 0.15f,
                        (EntityZ(ctx.Objects[0], true) + EntityZ(ctx.Objects[1], true)) / 2f);
                if (EntityDistance(NPCSystem.Curr106.Collider, GameState.Collider) < 4f)
                {
                    NPCSystem.Curr106.State = -11f;
                    NPCSystem.Curr106.Idle = false;
                    e.EventState = 260f;
                }
            }
            else if (e.EventState < 250f && ctx.Objects[0] != -1 && ctx.Objects[1] != -1)
            {
                PositionEntity(NPCSystem.Curr106.Collider, EntityX(ctx.Objects[0], true), EntityY(GameState.Collider) - 0.15f, EntityZ(ctx.Objects[0], true));
                PointEntity(NPCSystem.Curr106.Collider, ctx.Objects[1]);
                MoveEntity(NPCSystem.Curr106.Collider, 0f, 0f,
                    EntityDistance(ctx.Objects[0], ctx.Objects[1]) * ((e.EventState - 150f) / 100f));
            }

            if (PassedThreshold(e.EventState / 250f, 0.3f))
            {
                GameState.BlurTimer = 800;
                if (ctx.Objects[2] != -1)
                    DecalSystem.Create(0, EntityX(ctx.Objects[2], true), EntityY(ctx.Objects[2], true), EntityZ(ctx.Objects[2], true), 0f, e.Room.Angle - 90f, Rand(0, 360));
            }

            if (e.EventState > 250f) { NPCSystem.Curr106.Idle = false; RemoveEvent(e); }
        }

        private static void UpdateRoom2Pit106(GameEvent e)
        {
            if (Contained106 || NPCSystem.Curr106 == null || NPCSystem.Curr106.State <= 0f) return;
            var ctx = GetContext(e.Room);
            if (e.EventState == 0f) { if (InPlayerRoom(e)) e.EventState = 1f; return; }
            e.EventState += 1f;
            if (ctx.Objects[7] != -1)
            {
                PositionEntity(NPCSystem.Curr106.Collider,
                    EntityX(ctx.Objects[7], true), EntityY(ctx.Objects[7], true), EntityZ(ctx.Objects[7], true));
                ResetEntity(NPCSystem.Curr106.Collider);
            }
            if (e.EventState > 30f) RemoveEvent(e);
        }

        private static void UpdateRoom2Pit(GameEvent e)
        {
            if (!InPlayerRoom(e) || GetRoomDist(e.Room) >= 8f) return;
            if (e.EventState == 0f && NPCSystem.Curr173 != null && !NPCSystem.Curr173.Idle)
            {
                e.EventState = 1f;
                NPCSystem.Curr173.State = 70f;
            }
            if (e.EventState > 0f) Advance(e);
            if (e.EventState > 40f) RemoveEvent(e);
        }

        private static void UpdateRoom3PitDuck(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);
            if (GameState.CrouchState < 0.5f && ctx.Objects[0] != -1 &&
                EntityDistance(GameState.Collider, ctx.Objects[0]) < 2f)
            {
                GameState.KillTimer = 0f;
                GameState.DeathMsg = "Subject D-9341 was killed by SCP-173.";
            }
        }

        private static void UpdateRoom3Pit1048(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            Advance(e);
            if (e.EventState > 90f * 70f) RemoveEvent(e);
        }

        private static void UpdateRoom2Poffices2(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            Advance(e);
            if (e.EventState > 45f * 70f) RemoveEvent(e);
        }

        // ── room2storage (SCP-970) ────────────────────────────────────────────

        private static void UpdateRoom2Storage(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);

            if (e.EventState2 <= 0f)
            {
                if (ctx.RoomDoors[1] != null) ctx.RoomDoors[1].Locked = false;
                if (ctx.RoomDoors[4] != null) ctx.RoomDoors[4].Locked = false;
                bool lockDoors = (NPCSystem.Curr173 != null && EntityDistance(GameState.Collider, NPCSystem.Curr173.Obj) < 8f) ||
                                 (NPCSystem.Curr106 != null && EntityDistance(GameState.Collider, NPCSystem.Curr106.Obj) < 8f);
                if (lockDoors)
                {
                    if (ctx.RoomDoors[1] != null) ctx.RoomDoors[1].Locked = true;
                    if (ctx.RoomDoors[4] != null) ctx.RoomDoors[4].Locked = true;
                }
                e.EventState2 = 70f * 5f;
            }
            else
            {
                e.EventState2 -= GameState.FpsFactor;
            }

            // Shift detection via local X (approximate TForm)
            if (e.Room.obj != -1 && GameState.Collider != -1)
            {
                float localX = EntityX(GameState.Collider, true) - e.Room.x;
                if (Math.Abs(localX) > 730f * GameState.RoomScale)
                {
                    AchievementSystem.Unlock("970");
                    e.EventState += 1f;
                    switch ((int)e.EventState)
                    {
                        case 5: GameState.Injuries += 0.3f; break;
                        case 25:
                            if (ctx.Npc[0] == null)
                            {
                                ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeD, e.Room.x, 0.35f, e.Room.z);
                                ctx.Npc[0].State = 10f;
                            }
                            break;
                    }
                }
            }

            if (e.EventState > 26f && Math.Abs(EntityX(GameState.Collider, true) - e.Room.x) < 8f &&
                Math.Abs(EntityZ(GameState.Collider, true) - e.Room.z) < 8f && ctx.Npc[0] != null)
            {
                if (e.EventState < 30f) { /* dim light */ }
                else if (e.EventState > 60f)
                {
                    if (ctx.Npc[0] != null)
                        PositionEntity(ctx.Npc[0].Collider, EntityX(ctx.Npc[0].Collider, true),
                            1.5f + (float)Math.Sin(Environment.TickCount / 20.0) * 0.1f,
                            EntityZ(ctx.Npc[0].Collider, true));
                }
            }
        }

        private static void UpdateRoom2Test1074(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            AchievementSystem.Unlock("1074");

            bool protectedView = GameState.Wearing714 != 0 || GameState.WearingNightVision > 0;
            if (protectedView) { GameState.Playable = true; e.EventState = 0f; return; }

            var ctx = GetContext(e.Room);
            if (ctx.Objects[0] == -1) return;

            if (EntityVisible(ctx.Objects[0], GameState.Camera) && GameState.BlinkTimer > 0f && e.EventState == 0f)
                e.EventState = 1f;

            if (e.EventState > 0f)
            {
                Advance(e, MathUtil.Min(GameState.FpsFactor, 1.99f));
                if (e.EventState >= 100f && e.EventState <= 105f)
                {
                    e.EventState2 = MathUtil.Min(e.EventState2 + 0.01f * MathUtil.Min(GameState.FpsFactor, 1.99f), 0.5f);
                    GameState.ForceMove = (int)(e.EventState2 * 100f);
                    if (ctx.Objects[1] != -1 &&
                        EntityDistance(GameState.Collider, ctx.Objects[1]) < 8f * GameState.RoomScale)
                    {
                        e.EventState = 106f;
                        GameState.Playable = false;
                        GameState.ForceMove = 0;
                    }
                }
                if (!EntityVisible(ctx.Objects[0], GameState.Camera))
                {
                    GameState.Playable = true;
                    e.EventState = 0f;
                }
                if (e.EventState >= 1500f)
                {
                    GameState.DeathMsg = "God DAMMIT, Juan. What were you thinking?";
                    GameState.KillTimer = 0f;
                }
                if (GameState.KillTimer < 0f)
                {
                    e.EventState = 2300f;
                    GameState.ForceMove = 0;
                    GameState.Playable = true;
                }
            }
        }

        // ── room3 events ──────────────────────────────────────────────────────

        private static void UpdateRoom3Door(GameEvent e)
        {
            if (!InPlayerRoom(e) || EntityDistance(e.Room.obj, GameState.Collider) >= 2.5f) return;
            foreach (var d in DoorSystem.All)
            {
                if (Math.Abs(EntityX(d.Obj, true) - EntityX(GameState.Collider, true)) < 2f &&
                    Math.Abs(EntityZ(d.Obj, true) - EntityZ(GameState.Collider, true)) < 2f &&
                    !EntityVisible(d.Obj, GameState.Camera) && d.Open)
                {
                    d.Open = false;
                    d.OpenState = 0f;
                    GameState.BlurTimer = 100;
                    PlayerSystem.SetCameraShake(3f);
                    break;
                }
            }
            RemoveEvent(e);
        }

        private static void UpdateRoom3Servers(GameEvent e)
        {
            if (!InPlayerRoom(e) || NPCSystem.Curr173 == null) return;
            var ctx = GetContext(e.Room);

            if (e.EventState3 == 0f && NPCSystem.Curr173.Idle == false && GameState.BlinkTimer < -10f)
            {
                int t = Rand(0, 2);
                if (ctx.Objects[t] != -1)
                {
                    PositionEntity(NPCSystem.Curr173.Collider,
                        EntityX(ctx.Objects[t], true), EntityY(ctx.Objects[t], true), EntityZ(ctx.Objects[t], true));
                    ResetEntity(NPCSystem.Curr173.Collider);
                    e.EventState3 = 1f;
                }
            }

            if (ctx.Objects[3] > 0)
            {
                if (e.EventState2 == 0f)
                {
                    e.EventState = MathUtil.CurveValue(0f, e.EventState, 15f);
                    if (Rand(0, 800) == 1) e.EventState2 = 1f;
                }
                else
                {
                    e.EventState += GameState.FpsFactor * 0.5f;
                    if (e.EventState > 360f) e.EventState = 0f;
                    if (Rand(0, 1200) == 1) e.EventState2 = 0f;
                }
                PositionEntity(ctx.Objects[3],
                    EntityX(ctx.Objects[3], true),
                    (-608f * GameState.RoomScale) + 0.05f + (float)Math.Sin((e.EventState + 270f) * Math.PI / 180f) * 0.05f,
                    EntityZ(ctx.Objects[3], true), true);
            }
        }

        private static void UpdateRoom3Storage(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);
            e.EventState2 = UpdateElevatorsLocal(e.EventState2, ctx.RoomDoors[0], ctx.RoomDoors[1], e.Room.Objects[0], e.Room.Objects[1], e);
            e.EventState3 = UpdateElevatorsLocal(e.EventState3, ctx.RoomDoors[2], ctx.RoomDoors[3], e.Room.Objects[2], e.Room.Objects[3], e);

            if (EntityY(GameState.Collider, true) < -4600f * GameState.RoomScale)
            {
                if (e.EventState == 0f)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (ctx.Npc[i] == null)
                        {
                            int[] spawns = { 4, 9, 13 };
                            if (e.Room.Objects[spawns[i]] != -1)
                            {
                                ctx.Npc[i] = NPCSystem.CreateNpc(NPCSystem.NpcType939,
                                    EntityX(e.Room.Objects[spawns[i]], true),
                                    EntityY(e.Room.Objects[spawns[i]], true) + 0.2f,
                                    EntityZ(e.Room.Objects[spawns[i]], true));
                                ctx.Npc[i].State = 2f;
                            }
                        }
                    }
                    e.EventState = 1f;
                }

                if (ctx.RoomDoors[4] != null && !ctx.RoomDoors[4].Open)
                {
                    if (e.Room.Levers[0] != -1 && EventLeverHelper.UpdateLever(e.Room.Levers[0]))
                        ctx.RoomDoors[4].Open = true;
                    if (e.Room.Levers[1] != -1 && EventLeverHelper.UpdateLever(e.Room.Levers[1]))
                        ctx.RoomDoors[4].Open = true;
                }

                if (EntityY(GameState.Collider, true) < -6400f * GameState.RoomScale && GameState.KillTimer >= 0f)
                {
                    AudioSystem.Load("SFX/Room/PocketDimension/Impact.ogg")?.Play(GameState.SfxVolume, 0f, 0f);
                    GameState.KillTimer = -1f;
                }
            }
            else
            {
                e.EventState = 0f;
                for (int i = 0; i < 3; i++)
                    if (ctx.Npc[i] != null) ctx.Npc[i].State = 66f;
            }
        }

        private static void UpdateRoom3Tunnel(GameEvent e)
        {
            if (e.EventState != 0f || e.Room == null) return;
            var ctx = GetContext(e.Room);
            if (e.Room.Objects[0] == -1) return;
            ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard,
                EntityX(e.Room.Objects[0], true), EntityY(e.Room.Objects[0], true) + 0.5f, EntityZ(e.Room.Objects[0], true));
            PointEntity(ctx.Npc[0].Collider, e.Room.obj);
            ctx.Npc[0].State = 8f;
            e.EventState = 1f;
            RemoveEvent(e);
        }

        private static void UpdateRoom4(GameEvent e)
        {
            if (e.EventState >= Environment.TickCount) return;
            if (InPlayerRoom(e)) { e.EventState = Environment.TickCount + 5000; return; }

            if (MathUtil.PointDistance(EntityX(GameState.Collider, true), EntityZ(GameState.Collider, true),
                    EntityX(e.Room.obj, true), EntityZ(e.Room.obj, true)) < 16f)
            {
                foreach (var n in NPCSystem.All)
                {
                    if (n.NpcType != NPCSystem.NpcType049 || n.State != 2f) continue;
                    if (EntityDistance(GameState.Collider, n.Collider) <= 16f) continue;
                    PositionEntity(n.Collider, e.Room.x + 46f * GameState.RoomScale, 66f * GameState.RoomScale, e.Room.z + 22f * GameState.RoomScale);
                    ResetEntity(n.Collider);
                    n.State = 4f;
                    RemoveEvent(e);
                    break;
                }
            }
            e.EventState = Environment.TickCount + 5000;
        }
    }
}