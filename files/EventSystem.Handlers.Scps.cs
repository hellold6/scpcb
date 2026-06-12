// EventSystem.Handlers.Scps.cs — SCP room + misc event handlers

using System;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static partial class EventSystem
    {
        // ── room012 ───────────────────────────────────────────────────────────

        private static void UpdateRoom012(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);

            if (e.EventState == 0f)
            {
                if (ctx.RoomDoors[0] != null && RemoteDoorOn &&
                    EntityDistance(GameState.Collider, ctx.RoomDoors[0].Obj) < 2.5f)
                {
                    AchievementSystem.Unlock("012");
                    e.EventState = 1f;
                    if (ctx.RoomDoors[0] != null) ctx.RoomDoors[0].Locked = false;
                }
                return;
            }

            e.EventState = MathUtil.CurveValue(90f, e.EventState, 500f);
            if (ctx.Objects[2] != -1)
                PositionEntity(ctx.Objects[2], EntityX(ctx.Objects[2], true),
                    (-130f - 448f * (float)Math.Sin(e.EventState * Math.PI / 180f)) * GameState.RoomScale,
                    EntityZ(ctx.Objects[2], true), true);

            if (GameState.Wearing714 != 0 || GameState.WearingHazmat >= 3) return;
            if (ctx.Objects[2] == -1 || !EntityVisible(ctx.Objects[2], GameState.Camera)) return;

            float dist = MathUtil.PointDistance(
                EntityX(GameState.Collider, true), EntityZ(GameState.Collider, true),
                EntityX(ctx.Objects[2], true), EntityZ(ctx.Objects[2], true));

            GameState.BlurVolume = MathUtil.Max((2f - dist) * (e.EventState3 / 800f), GameState.BlurVolume);

            if (dist < 0.6f)
            {
                e.EventState3 = MathUtil.Min(e.EventState3 + GameState.FpsFactor, 86f * 70f);
                if (PassedThreshold(e.EventState3, 70f))
                    AudioSystem.Load("SFX/SCP/012/Speech1.ogg")?.Play(GameState.SfxVolume, 0f, 0f);
                if (PassedThreshold(e.EventState3, 13f * 70f))
                {
                    GameState.Msg = "You start pushing your nails into your wrist, drawing blood.";
                    GameState.MsgTimer = 7f * 70f;
                    GameState.Injuries += 0.5f;
                }
                if (PassedThreshold(e.EventState3, 31f * 70f))
                {
                    GameState.Msg = "You tear open your left wrist and start writing on the composition with your blood.";
                    GameState.MsgTimer = 7f * 70f;
                    GameState.Injuries = MathUtil.Max(GameState.Injuries, 1.5f);
                }
                if (PassedThreshold(e.EventState3, 85f * 70f))
                {
                    GameState.DeathMsg = "Subject D-9341 found in a pool of blood next to SCP-012.";
                    GameState.KillTimer = 0f;
                }
            }
            else
            {
                int pvt = CreatePivot();
                PositionEntity(pvt, EntityX(GameState.Camera, true), EntityY(ctx.Objects[2], true) - 0.05f, EntityZ(GameState.Camera, true));
                PointEntity(pvt, ctx.Objects[2]);
                float angle = MathUtil.WrapAngle(EntityYaw(pvt) - EntityYaw(GameState.Collider));
                if (angle < 40f) GameState.ForceMove = (int)((40f - angle) * 2f);
                else if (angle > 310f) GameState.ForceMove = (int)((40f - Math.Abs(360f - angle)) * 2f);
                FreeEntity(pvt);
            }
        }

        // ── room035 ───────────────────────────────────────────────────────────

        private static void UpdateRoom035(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);

            if (e.EventState == 0f)
            {
                if (ctx.Objects[3] != -1 && EntityDistance(GameState.Collider, ctx.Objects[3]) < 2f && ctx.Objects[4] != -1)
                {
                    var n = NPCSystem.CreateNpc(NPCSystem.NpcTypeD,
                        EntityX(ctx.Objects[4], true), 0.5f, EntityZ(ctx.Objects[4], true));
                    n.Texture = "GFX/NPCs/035victim.jpg";
                    n.State = 6f;
                    ctx.Npc[0] = n;
                    e.EventState = 1f;
                }
                return;
            }

            if (e.EventState < 0f)
            {
                UpdateRoom035Chamber(e, ctx);
                return;
            }

            if (ctx.Npc[0] == null) return;

            if (e.EventState == 1f)
            {
                if (EntityDistance(GameState.Collider, ctx.Objects[3]) < 1.2f &&
                    EntityVisible(ctx.Npc[0].Obj, GameState.Camera))
                {
                    AchievementSystem.Unlock("035");
                    e.EventState = 1.5f;
                    AudioSystem.Load("SFX/SCP/035/GetUp.ogg")?.Play(GameState.SfxVolume, 0f, 0f);
                }
                return;
            }

            if (ctx.RoomDoors[3] != null && ctx.RoomDoors[3].Open)
                e.EventState2 = MathUtil.Max(e.EventState2, 1f);

            bool doorClosed = e.Room.Levers[0] == -1 || !EventLeverHelper.UpdateLever(e.Room.Levers[0], e.EventState2 >= 20f);
            if (doorClosed)
            {
                bool gasOn = e.Room.Levers[1] != -1 && EventLeverHelper.UpdateLever(e.Room.Levers[1]);
                if (gasOn || (e.EventState3 > 25f * 70f && e.EventState3 < 50f * 70f))
                {
                    if (e.EventState3 > -30f * 70f)
                    {
                        e.EventState3 = Math.Abs(e.EventState3) + GameState.FpsFactor;
                        if (PassedThreshold(e.EventState3, 1f)) ctx.Npc[0].State = 0f;
                        if (e.EventState3 > 35f * 70f)
                        {
                            GameState.Sanity = -150f * (float)Math.Sin(0.1f) * 9f;
                            if (PassedThreshold(e.EventState3, 35f * 70f))
                                e.EventState = 60f * 70f;
                        }
                    }
                }
                else
                {
                    Advance(e);
                    if (PassedThreshold(e.EventState, 4f * 70f)) e.EventState = 10f * 70f;
                }
            }
            else if (e.EventState2 < 20f)
            {
                e.EventState2 = 20f;
                if (ctx.RoomDoors[2] != null) { ctx.RoomDoors[2].Open = false; ctx.RoomDoors[2].Locked = true; }
            }
            else if (e.EventState2 >= 20f && ctx.Npc[0] != null)
            {
                ctx.Npc[0].State = 1f;
                if (ctx.RoomDoors[0] != null && EntityDistance(ctx.RoomDoors[0].FrameObj, ctx.Npc[0].Collider) < 0.7f)
                {
                    NPCSystem.Remove(ctx.Npc[0]);
                    ctx.Npc[0] = null;
                    e.EventState = -1f;
                    e.EventState2 = 0f;
                    e.EventState3 = 0f;
                    if (ctx.RoomDoors[0] != null) ctx.RoomDoors[0].Locked = false;
                    if (ctx.RoomDoors[1] != null) ctx.RoomDoors[1].Locked = false;
                }
            }
        }

        private static void UpdateRoom035Chamber(GameEvent e, EventRoomContext ctx)
        {
            if (e.Room.Objects[7] == -1 || e.Room.Objects[8] == -1) return;
            float minX = Math.Min(EntityX(e.Room.Objects[7], true), EntityX(e.Room.Objects[8], true));
            float maxX = Math.Max(EntityX(e.Room.Objects[7], true), EntityX(e.Room.Objects[8], true));
            float minZ = Math.Min(EntityZ(e.Room.Objects[7], true), EntityZ(e.Room.Objects[8], true));
            float maxZ = Math.Max(EntityZ(e.Room.Objects[7], true), EntityZ(e.Room.Objects[8], true));
            float px = EntityX(GameState.Collider, true);
            float pz = EntityZ(GameState.Collider, true);

            bool inside = px > minX && px < maxX && pz > minZ && pz < maxZ;
            if (inside)
            {
                if (ctx.Npc[0] == null)
                    ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeTentacle, 0f, 0f, 0f);
                if (ctx.Objects[4] != -1 && ctx.Npc[0] != null)
                    PositionEntity(ctx.Npc[0].Collider, EntityX(ctx.Objects[4], true), 0f, EntityZ(ctx.Objects[4], true));

                GameState.Stamina = MathUtil.CurveValue(MathUtil.Min(60f, GameState.Stamina), GameState.Stamina, 20f);
                e.EventState2 = MathUtil.Min(e.EventState2 + GameState.FpsFactor / 6000f, 1f);
                e.EventState3 = MathUtil.CurveValue(e.EventState2, e.EventState3, 50f);

                if (GameState.Wearing714 == 0 && GameState.WearingHazmat < 3 && !GameState.WearingGasMask)
                {
                    GameState.Sanity -= GameState.FpsFactor * 1.1f;
                    GameState.BlurTimer = (int)((float)Math.Sin(Environment.TickCount / 10.0) * Math.Abs(GameState.Sanity));
                }
                if (GameState.WearingHazmat == 0)
                    GameState.Injuries += GameState.FpsFactor / 5000f;
            }
            else
            {
                e.EventState2 = MathUtil.Max(e.EventState2 - GameState.FpsFactor / 2000f, 0f);
                e.EventState3 = MathUtil.Max(e.EventState3 - GameState.FpsFactor / 100f, 0f);
            }
        }

        // ── room049 ───────────────────────────────────────────────────────────

        private static void UpdateRoom049(GameEvent e)
        {
            if (e.Room == null) return;
            var ctx = GetContext(e.Room);

            if (InPlayerRoom(e) && EntityY(GameState.Collider, true) > -2848f * GameState.RoomScale)
            {
                e.EventState2 = UpdateElevatorsLocal(e.EventState2, ctx.RoomDoors[0], ctx.RoomDoors[1], e.Room.Objects[0], e.Room.Objects[1], e);
                e.EventState3 = UpdateElevatorsLocal(e.EventState3, ctx.RoomDoors[2], ctx.RoomDoors[3], e.Room.Objects[2], e.Room.Objects[3], e);
            }
            else if (InPlayerRoom(e))
            {
                if (e.EventState == 0f)
                {
                    AudioSystem.Load("SFX/Room/Blackout.ogg")?.Play(GameState.SfxVolume, 0f, 0f);
                    int spawnObj = e.Room.Objects[11] != -1 && e.Room.Objects[12] != -1 &&
                        EntityDistance(GameState.Collider, e.Room.Objects[11]) < EntityDistance(GameState.Collider, e.Room.Objects[12])
                        ? 11 : 12;
                    if (e.Room.Objects[spawnObj] != -1)
                        ItemSystem.CreateItem("Research Sector-02 Scheme", "paper",
                            EntityX(e.Room.Objects[spawnObj], true), EntityY(e.Room.Objects[spawnObj], true), EntityZ(e.Room.Objects[spawnObj], true));
                    e.EventState = 1f;
                }
                else if (e.EventState > 0f)
                {
                    bool power = e.Room.Objects[7] == -1 || !EventLeverHelper.UpdateLever(e.Room.Objects[7]);
                    bool gen = e.Room.Objects[9] != -1 && EventLeverHelper.UpdateLever(e.Room.Objects[9]);

                    if (ctx.RoomDoors[1] != null) ctx.RoomDoors[1].Locked = true;
                    if (ctx.RoomDoors[3] != null) ctx.RoomDoors[3].Locked = true;

                    if (e.EventState < 70f) e.EventState = MathUtil.Min(e.EventState + GameState.FpsFactor, 70f);
                    else if (gen)
                    {
                        e.EventState = MathUtil.Max(e.EventState, 70f * 180f);
                        GameState.SecondaryLightOn = MathUtil.CurveValue(1f, GameState.SecondaryLightOn, 10f);
                        for (int i = 4; i <= 6; i++)
                            if (ctx.RoomDoors[i] != null) ctx.RoomDoors[i].Locked = false;
                    }
                    else
                    {
                        GameState.SecondaryLightOn = MathUtil.CurveValue(0f, GameState.SecondaryLightOn, 10f);
                        for (int i = 4; i <= 6; i++)
                            if (ctx.RoomDoors[i] != null) ctx.RoomDoors[i].Locked = true;
                    }

                    if (power && gen)
                    {
                        if (ctx.RoomDoors[1] != null) ctx.RoomDoors[1].Locked = false;
                        if (ctx.RoomDoors[3] != null) ctx.RoomDoors[3].Locked = false;
                        e.EventState2 = UpdateElevatorsLocal(e.EventState2, ctx.RoomDoors[0], ctx.RoomDoors[1], e.Room.Objects[0], e.Room.Objects[1], e);
                        e.EventState3 = UpdateElevatorsLocal(e.EventState3, ctx.RoomDoors[2], ctx.RoomDoors[3], e.Room.Objects[2], e.Room.Objects[3], e);
                    }

                    if (e.EventState >= 70f * 180f && e.EventState < 70f * 240f)
                    {
                        if (ctx.RoomDoors[1] != null) ctx.RoomDoors[1].Open = false;
                        if (ctx.RoomDoors[3] != null) ctx.RoomDoors[3].Open = false;
                        if (ctx.RoomDoors[0] != null) ctx.RoomDoors[0].Open = true;
                        if (ctx.RoomDoors[2] != null) ctx.RoomDoors[2].Open = true;
                        e.EventState = 70f * 241f;
                    }
                    else if (e.EventState >= 70f * 241f)
                    {
                        foreach (var n in NPCSystem.All)
                            if (n.NpcType == NPCSystem.NpcTypeZombie && n.State == 0f) n.State = 1f;
                    }
                }
            }
            else
            {
                e.EventState2 = UpdateElevatorsLocal(e.EventState2, ctx.RoomDoors[0], ctx.RoomDoors[1], e.Room.Objects[0], e.Room.Objects[1], e);
                e.EventState3 = UpdateElevatorsLocal(e.EventState3, ctx.RoomDoors[2], ctx.RoomDoors[3], e.Room.Objects[2], e.Room.Objects[3], e);
            }

            if (e.EventState < 0f)
            {
                GameState.Infect = 0f;
                GameState.BlurTimer = 800;
                GameState.ForceMove = 1;
                GameState.Injuries = MathUtil.Max(2f, GameState.Injuries);
                GameState.Bloodloss = 0f;
                GameState.IsZombie = true;
                if (GameState.KillTimer < 0f) RemoveEvent(e);
            }
        }

        // ── room079 ───────────────────────────────────────────────────────────

        private static void UpdateRoom079(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);

            if (e.EventState == 0f)
            {
                if (ctx.Objects[2] != -1)
                {
                    ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard,
                        EntityX(ctx.Objects[2], true), EntityY(ctx.Objects[2], true) + 0.5f, EntityZ(ctx.Objects[2], true));
                    PointEntity(ctx.Npc[0].Collider, e.Room.obj);
                    ctx.Npc[0].State = 8f;
                }
                e.EventState = 1f;
            }

            if (!RemoteDoorOn && e.EventState < 10000f)
            {
                if (e.EventState == 1f) e.EventState = 2f;
                else if (e.EventState == 2f && ctx.Objects[0] != -1 && EntityDistance(ctx.Objects[0], GameState.Collider) < 3f)
                {
                    AchievementSystem.Unlock("079");
                    e.EventState = 3f;
                    e.EventState2 = 1f;
                }
                else if (e.EventState < 2000f)
                    Advance(e);
                else if (ctx.Objects[0] != -1 && EntityDistance(ctx.Objects[0], GameState.Collider) < 2.5f)
                    e.EventState = 10001f;
            }
            else if (RemoteDoorOn && ctx.RoomDoors[0] != null && ctx.RoomDoors[0].Open)
            {
                if (ctx.RoomDoors[0].OpenState > 50f || EntityDistance(GameState.Collider, ctx.RoomDoors[0].FrameObj) < 0.5f)
                {
                    ctx.RoomDoors[0].OpenState = MathUtil.Min(ctx.RoomDoors[0].OpenState, 50f);
                    ctx.RoomDoors[0].Open = false;
                }
            }

            if (e.EventState2 == 1f && RemoteDoorOn)
            {
                e.EventState2 = 2f;
                foreach (var ev in _events)
                {
                    if (ev.EventName is "exit1" or "gateaentrance")
                        ev.EventState3 = 1f;
                }
            }
        }

        // ── room860 ───────────────────────────────────────────────────────────

        private static void UpdateRoom860(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);

            if (e.EventState == 1f)
            {
                if (!GameState.NoClip && GameState.Collider != -1)
                {
                    if (EntityY(GameState.Collider) <= 28.5f)
                    {
                        GameState.KillTimer = 0f;
                        GameState.BlinkTimer = -2f;
                    }
                }

                if (ctx.Npc[0] == null || EntityDistance(GameState.Collider, ctx.Npc[0].Collider) > 20f)
                {
                    e.EventState3 += (1f + GameState.CurrSpeed) * GameState.FpsFactor;
                    if (e.EventState3 > 3000f - (500f * (GameState.SelectedDifficulty?.AggressiveNpcs == true ? 1f : 0f)) &&
                        Rand(0, 10000) < (int)e.EventState3)
                    {
                        if (ctx.Npc[0] == null)
                            ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcType860, 0f, -110f, 0f);
                        ctx.Npc[0].State = 2f;
                        e.EventState3 -= Rand(1000, 2000);
                    }
                }
            }
            else
            {
                if (!Contained106 && NPCSystem.Curr106 != null)
                    NPCSystem.Curr106.Idle = false;

                if (e.Room.Objects[3] != -1 && EntityYaw(e.Room.Objects[3]) == 0f &&
                    MathUtil.PointDistance(EntityX(e.Room.Objects[3], true), EntityZ(e.Room.Objects[3], true),
                        EntityX(GameState.Collider, true), EntityZ(GameState.Collider, true)) < 1f)
                {
                    GameState.DrawHandIcon = true;
                    e.EventState = 1f;
                    e.EventState3 = 0f;
                    GameState.PrevSecondaryLightOn = GameState.SecondaryLightOn;
                    GameState.SecondaryLightOn = 1f;
                    GameState.BlinkTimer = -10f;
                }
            }
        }

        // ── room966 ───────────────────────────────────────────────────────────

        private static void UpdateRoom966(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            if (e.EventState == 0f) e.EventState = 1f;
            else if (e.EventState == 2f) RemoveEvent(e);
        }

        // ── room1123 ──────────────────────────────────────────────────────────

        private static void UpdateRoom1123(GameEvent e)
        {
            if (!InPlayerRoom(e) || e.EventState <= 0f) return;
            var ctx = GetContext(e.Room);

            if (e.EventState > 0f && e.EventState < 7f) GameState.CanSave = false;

            if (e.EventState == 1f)
            {
                GameState.PrevInjuries = GameState.Injuries;
                GameState.PrevBloodloss = GameState.Bloodloss;
                GameState.PrevSecondaryLightOn = GameState.SecondaryLightOn;
                GameState.SecondaryLightOn = 1f;
                if (e.Room.Objects[6] != -1)
                    ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeD,
                        EntityX(e.Room.Objects[6], true), EntityY(e.Room.Objects[6], true), EntityZ(e.Room.Objects[6], true));
                if (e.Room.Objects[4] != -1)
                    PositionEntity(GameState.Collider, EntityX(e.Room.Objects[4], true), EntityY(e.Room.Objects[4], true), EntityZ(e.Room.Objects[4], true), true);
                PlayerSystem.SetCameraShake(1f);
                GameState.BlurTimer = 1200;
                GameState.Injuries = 1f;
                e.EventState = 2f;
            }
            else if (e.EventState == 2f)
            {
                Advance2(e);
                if (ctx.Npc[0] != null) PointEntity(ctx.Npc[0].Collider, GameState.Collider);
                GameState.BlurTimer = (int)MathUtil.Max(GameState.BlurTimer, 100f);
                if (e.EventState2 > 1000f && e.Room.Objects[4] != -1 &&
                    EntityDistance(GameState.Collider, e.Room.Objects[4]) > 392f * GameState.RoomScale)
                {
                    if (e.Room.Objects[5] != -1)
                        PositionEntity(GameState.Collider, EntityX(e.Room.Objects[5], true), EntityY(e.Room.Objects[5], true), EntityZ(e.Room.Objects[5], true), true);
                    e.EventState = 3f;
                }
            }
            else if (e.EventState == 3f)
            {
                if (ctx.RoomDoors[0] != null && ctx.RoomDoors[0].OpenState > 160f)
                {
                    if (ctx.Npc[0] != null && e.Room.Objects[7] != -1)
                        PositionEntity(ctx.Npc[0].Collider, EntityX(e.Room.Objects[7], true), EntityY(e.Room.Objects[7], true), EntityZ(e.Room.Objects[7], true));
                    e.EventState = 4f;
                }
            }
            else if (e.EventState == 4f)
            {
                if (e.Room.Objects[13] != -1 && EntityYaw(e.Room.Objects[13]) > 30f)
                {
                    if (ctx.Npc[0] != null) ctx.Npc[0].State = 3f;
                    if (ctx.Npc[0] != null && ctx.Npc[0].Frame >= 54f)
                    {
                        e.EventState = 5f;
                        e.EventState2 = 0f;
                        PositionEntity(GameState.Collider, e.Room.x, 0.3f, e.Room.z, true);
                        GameState.BlinkTimer = -10f;
                        GameState.BlurTimer = 500;
                        GameState.Injuries = 1.5f;
                        GameState.Bloodloss = 70f;
                    }
                }
            }
            else if (e.EventState == 5f)
            {
                Advance2(e);
                if (e.EventState2 > 500f) e.EventState = 6f;
            }
            else if (e.EventState == 6f)
            {
                e.EventState = 7f;
            }
            else if (e.EventState == 7f)
            {
                GameState.Injuries = GameState.PrevInjuries;
                GameState.Bloodloss = GameState.PrevBloodloss;
                GameState.SecondaryLightOn = GameState.PrevSecondaryLightOn;
                GameState.PrevInjuries = 0f;
                GameState.PrevBloodloss = 0f;
                GameState.CanSave = true;
                AchievementSystem.Unlock("1123");
                if (ctx.Npc[0] != null) NPCSystem.Remove(ctx.Npc[0]);
                RemoveEvent(e);
            }
        }

        // ── testroom / tunnels / guard ────────────────────────────────────────

        private static void UpdateTestroom(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            if (e.EventState == 0f) e.EventState = 1f;
            if (e.Room.Objects[6] != -1 && EntityDistance(GameState.Collider, e.Room.Objects[6]) < 2.5f && e.EventState > 0f)
            {
                AudioSystem.Load("SFX/SCP/079/TestroomWarning.ogg")?.Play(GameState.SfxVolume, 0f, 0f);
                e.EventState = -e.EventState;
            }
            if (e.EventState == -2f) RemoveEvent(e);
        }

        private static void UpdateTunnel2Smoke(GameEvent e)
        {
            if (!InPlayerRoom(e) || GetRoomDist(e.Room) >= 3.5f) return;
            AudioSystem.PlaySound2(AudioSystem.Load("SFX/General/Burst.ogg"), GameState.Camera, e.Room.obj);
            RemoveEvent(e);
        }

        private static void UpdateTunnel2(GameEvent e)
        {
            if (InPlayerRoom(e) && NPCSystem.Curr173 != null && NPCSystem.Curr173.Idle)
            {
                RemoveEvent(e);
                return;
            }

            if (InPlayerRoom(e) && e.EventState == 0f &&
                MathUtil.PointDistance(EntityX(GameState.Collider, true), EntityZ(GameState.Collider, true),
                    EntityX(e.Room.obj, true), EntityZ(e.Room.obj, true)) < 3.5f)
            {
                e.EventState = 1f;
            }

            if (e.EventState > 0f && e.EventState < 200f)
            {
                GameState.BlinkTimer = -10f;
                if (PassedThreshold(e.EventState, 100f) && NPCSystem.Curr173 != null)
                {
                    PositionEntity(NPCSystem.Curr173.Collider, EntityX(e.Room.obj, true), 0.6f, EntityZ(e.Room.obj, true));
                    ResetEntity(NPCSystem.Curr173.Collider);
                    NPCSystem.Curr173.Idle = true;
                }
                Advance(e);
            }
            else if (e.EventState != 0f)
            {
                if (NPCSystem.Curr173 != null) NPCSystem.Curr173.Idle = false;
                RemoveEvent(e);
            }
        }

        private static void UpdateTestroom173(GameEvent e)
        {
            if (!InPlayerRoom(e) || NPCSystem.Curr173 == null || NPCSystem.Curr173.Idle) return;
            if (e.EventState == 0f && EntityDistance(GameState.Collider, e.Room.obj) < 8f)
            {
                PositionEntity(NPCSystem.Curr173.Collider, EntityX(e.Room.obj, true), 0.5f, EntityZ(e.Room.obj, true));
                ResetEntity(NPCSystem.Curr173.Collider);
                RemoveEvent(e);
            }
        }

        private static void UpdateToiletGuard(GameEvent e)
        {
            if (e.Room == null) return;
            var ctx = GetContext(e.Room);

            if (e.EventState == 0f)
            {
                if (GetRoomDist(e.Room) < 8f && GetRoomDist(e.Room) > 0f) e.EventState = 1f;
            }
            else if (e.EventState == 1f && e.Room.Objects[1] != -1)
            {
                ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard,
                    EntityX(e.Room.Objects[1], true), EntityY(e.Room.Objects[1], true) + 0.5f, EntityZ(e.Room.Objects[1], true));
                PointEntity(ctx.Npc[0].Collider, e.Room.obj);
                ctx.Npc[0].State = 8f;
                e.EventState = 2f;
            }
            else if (GetRoomDist(e.Room) < 4f)
            {
                if (e.EventState2 == 0f && e.Room.Objects[2] != -1)
                {
                    var de = DecalSystem.Create(3, EntityX(e.Room.Objects[2], true), EntityY(e.Room.Objects[2], true), EntityZ(e.Room.Objects[2], true), 0f, e.Room.Angle + 270f, 0f);
                    de.Size = 0.3f;
                    e.EventState2 = 1f;
                }
                RemoveEvent(e);
            }
        }

        // ── 008 ───────────────────────────────────────────────────────────────

        private static void Update008(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);
            AchievementSystem.Unlock("008");

            if (e.EventState == 0f)
            {
                if (NPCSystem.Curr173 != null && NPCSystem.Curr173.Idle &&
                    EntityDistance(NPCSystem.Curr173.Collider, GameState.Collider) > 15f && e.Room.Objects[3] != -1)
                {
                    PositionEntity(NPCSystem.Curr173.Collider, EntityX(e.Room.Objects[3], true), 0.5f, EntityZ(e.Room.Objects[3], true), true);
                    ResetEntity(NPCSystem.Curr173.Collider);
                }
                e.EventState = 1f;
            }
            else if (e.EventState == 1f)
            {
                if (ctx.Objects[0] != -1 && EntityDistance(GameState.Collider, ctx.Objects[0]) < 2f)
                {
                    if (ctx.RoomDoors[0] != null) ctx.RoomDoors[0].Locked = true;
                    if (ctx.RoomDoors[1] != null) ctx.RoomDoors[1].Locked = true;

                    if (e.EventState2 == 0f && NPCSystem.Curr173 != null && e.Room.Objects[4] != -1 &&
                        EntityDistance(NPCSystem.Curr173.Collider, e.Room.Objects[4]) < 3f &&
                        (GameState.BlinkTimer < -10f || !EntityVisible(NPCSystem.Curr173.Obj, GameState.Camera)))
                    {
                        PositionEntity(NPCSystem.Curr173.Collider, EntityX(e.Room.Objects[4], true), 0.5f, EntityZ(e.Room.Objects[4], true), true);
                        ResetEntity(NPCSystem.Curr173.Collider);
                        if (GameState.WearingHazmat == 0)
                        {
                            GameState.Injuries += 0.1f;
                            if (GameState.Infect == 0f) GameState.Infect = 1f;
                            GameState.Msg = "The window shattered and a piece of glass cut your arm.";
                            GameState.MsgTimer = 70f * 8f;
                        }
                        e.EventState2 = 1f;
                    }
                }

                if (e.Room.Levers[0] != -1 && EntityPitch(e.Room.Levers[0]) < 40f)
                    e.EventState = 2f;
            }
            else
            {
                if (ctx.RoomDoors[0] != null) ctx.RoomDoors[0].Locked = false;
                if (ctx.RoomDoors[1] != null) ctx.RoomDoors[1].Locked = false;
                if (ctx.RoomDoors[2] != null) ctx.RoomDoors[2].Locked = false;
                if (e.Room.Levers[0] != -1 && EntityPitch(e.Room.Levers[0]) <= 1f)
                    RemoveEvent(e);
            }
        }

        // ── 106victim / 106sinkhole ───────────────────────────────────────────

        private static void Update106Victim(GameEvent e)
        {
            if (Contained106 || !InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);

            if (e.EventState == 0f)
            {
                DecalSystem.Create(0, EntityX(e.Room.obj, true), 799f * GameState.RoomScale, EntityZ(e.Room.obj, true), -90f, Rand(0, 360), 0f);
                e.EventState = 1f;
            }

            if (e.EventState > 0f)
            {
                if (ctx.Npc[0] == null) Advance(e);
                if (e.EventState > 200f && ctx.Npc[0] == null)
                {
                    ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeD, EntityX(e.Room.obj, true), 900f * GameState.RoomScale, EntityZ(e.Room.obj, true));
                    ctx.Npc[0].State = 6f;
                }
                if (e.EventState > 400f) RemoveEvent(e);
            }
        }

        private static void Update106Sinkhole(GameEvent e)
        {
            if (e.EventState == 0f)
            {
                var de = DecalSystem.Create(0, EntityX(e.Room.obj, true) + Rnd(-0.5f, 0.5f), 0.01f, EntityZ(e.Room.obj, true) + Rnd(-0.5f, 0.5f), 90f, Rand(0, 360), 0f);
                de.Size = 2.5f;
                e.EventState = 1f;
            }
            else if (InPlayerRoom(e))
            {
                float dist = MathUtil.PointDistance(EntityX(GameState.Collider, true), EntityZ(GameState.Collider, true),
                    EntityX(e.Room.obj, true), EntityZ(e.Room.obj, true));
                if (dist < 0.5f)
                {
                    if (e.EventState2 == 0f)
                        AudioSystem.Load("SFX/Room/SinkholeFall.ogg")?.Play(GameState.SfxVolume, 0f, 0f);
                    e.EventState2 = MathUtil.Min(e.EventState2 + GameState.FpsFactor / 200f, 2f);
                    GameState.BlurTimer = (int)(e.EventState2 * 500f);
                    if (e.EventState2 >= 2f) MoveToPocketDimension();
                }
            }
            else e.EventState2 = 0f;
        }

        // ── 1048a ─────────────────────────────────────────────────────────────

        private static void Update1048a(GameEvent e)
        {
            if (e.Room == null) return;
            float dist = MathUtil.PointDistance(EntityX(GameState.Collider, true), EntityZ(GameState.Collider, true),
                EntityX(e.Room.obj, true), EntityZ(e.Room.obj, true));

            if (e.Room.Objects[0] == -1)
            {
                if (InPlayerRoom(e) || dist >= 16f || GameState.BlinkTimer >= -10f) return;
                e.EventState = 1f;
                e.EventState3 = 1f;
            }
            else
            {
                switch ((int)e.EventState)
                {
                    case 1:
                        if (EntityDistance(GameState.Collider, e.Room.Objects[0]) < 2.5f) e.EventState = 2f;
                        break;
                    case 2:
                        GameState.BlurTimer = 1000;
                        PlayerSystem.SetCameraShake(10f);
                        if (InPlayerRoom(e)) { e.EventState = 3f; e.EventState2 = 0f; }
                        else e.EventState3 = 70f * 30f;
                        break;
                    case 3:
                        Advance2(e);
                        GameState.BlurTimer = (int)e.EventState2 * 2;
                        if (e.EventState2 > 70f * 15f)
                        {
                            GameState.DeathMsg = "A dead body covered in ears was found in [REDACTED].";
                            GameState.KillTimer = 0f;
                            RemoveEvent(e);
                        }
                        break;
                }

                if (!InPlayerRoom(e) && e.EventState3 > 0f)
                {
                    e.EventState3 += GameState.FpsFactor;
                    if (e.EventState3 > 70f * 25f) RemoveEvent(e);
                }
            }
        }

        // ── misc room events ──────────────────────────────────────────────────

        private static void UpdateRoom4Tunnels(GameEvent e)
        {
            if (GetRoomDist(e.Room) >= 10f || GetRoomDist(e.Room) <= 0f) return;
            var ctx = GetContext(e.Room);
            ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeD, EntityX(e.Room.obj, true) + 1f, 0.5f, EntityZ(e.Room.obj, true) + 1f);
            ctx.Npc[0].State = 8f;
            RemoveEvent(e);
        }

        private static void UpdateRoom2GwB(GameEvent e)
        {
            if (GetRoomDist(e.Room) >= 8f) return;
            var ctx = GetContext(e.Room);
            if (e.EventState == 0f && e.Room.Objects[2] != -1)
            {
                ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard,
                    EntityX(e.Room.Objects[2], true), EntityY(e.Room.Objects[2], true) + 0.5f, EntityZ(e.Room.Objects[2], true));
                ctx.Npc[0].State = 8f;
                e.EventState = 1f;
            }
        }

        private static void UpdateRoom2Scps2(GameEvent e)
        {
            if (GetRoomDist(e.Room) >= 15f) return;
            var ctx = GetContext(e.Room);
            if (Contained106 || (NPCSystem.Curr106 != null && NPCSystem.Curr106.State < 0f))
                e.EventState = 2f;

            if (e.EventState < 2f)
            {
                if (e.EventState == 0f)
                {
                    if (e.Room.Objects[0] != -1)
                        DecalSystem.Create(0, EntityX(e.Room.Objects[0], true), e.Room.y + 2f * GameState.RoomScale, EntityZ(e.Room.Objects[0], true), 90f, Rand(0, 360), 0f);
                    e.EventState = 1f;
                }
                else e.EventState = 2f;
            }
            else
            {
                if (ctx.RoomDoors[0] != null) ctx.RoomDoors[0].Locked = false;
                RemoveEvent(e);
            }
        }

        private static void UpdateRoom1162(GameEvent e)
        {
            if (!InPlayerRoom(e) || e.Room.Objects[0] == -1) return;
            if (EntityDistance(e.Room.Objects[0], GameState.Collider) < 0.75f)
            {
                GameState.DrawHandIcon = true;
                e.EventState3 = 3f;
                GameState.Msg = "You feel a strange sense of nostalgia.";
                GameState.MsgTimer = 70f * 5f;
            }
        }

        private static void UpdateRoomGw(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            Advance(e);
            if (e.EventState > 90f * 70f) RemoveEvent(e);
        }

        private static void UpdateMedibay(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);
            if (e.EventState == 0f && ctx.RoomDoors[0] != null && ctx.RoomDoors[0].Open)
                e.EventState = 1f;
            if (e.EventState > 0f) Advance(e);
        }

        private static void UpdateDimension1499(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            Advance(e);
            GameState.Wearing1499 = true;
            if (e.EventState > 200f) RemoveEvent(e);
        }

        private static void UpdateRoom2Offices035(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);
            if (e.EventState == 0f)
            {
                foreach (var ev in _events)
                    if (ev.EventName == "room035" && ev.EventState2 >= 1f) e.EventState = 1f;
            }
            if (e.EventState > 0f && ctx.RoomDoors[0] != null)
                ctx.RoomDoors[0].Locked = false;
        }

        private static void UpdateRoom2Shaft(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            Advance(e);
            if (e.EventState > 100f * 70f) RemoveEvent(e);
        }

        private static void UpdateRoom1Lifts(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);
            e.EventState2 = UpdateElevatorsLocal(e.EventState2, ctx.RoomDoors[0], ctx.RoomDoors[1], e.Room.Objects[0], e.Room.Objects[1], e);
            if (e.EventState == 0f) e.EventState = 1f;
        }
    }
}