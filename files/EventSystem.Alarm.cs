// EventSystem.Alarm.cs — ports UpdateEvents.bb Case "alarm" (start room breach)

using Microsoft.Xna.Framework.Audio;
using SCPCB360.Engine;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static partial class EventSystem
    {
        private static SoundEffect _alarmLoopSfx;
        private static bool _alarmCommotionPlayed;

        private static void UpdateAlarm(GameEvent e)
        {
            if (e.Room == null) return;
            var ctx = GetContext(e.Room);
            float rs = GameState.RoomScale;
            var room = e.Room;

            if (ctx.RoomDoors[5] == null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (ctx.AdjDoor[i] != null)
                    {
                        ctx.RoomDoors[5] = ctx.AdjDoor[i];
                        ctx.RoomDoors[5].Open = true;
                        break;
                    }
                }
            }

            if (e.EventState == 0f)
            {
                if (!InPlayerRoom(e)) return;
                ActivateAlarmRoom(e, ctx, rs);
                return;
            }

            string trigger = TriggerSystem.CheckTriggers();

            if (ctx.Npc[0] != null)
                ctx.Npc[0].Frame = MathUtil.Min(ctx.Npc[0].Frame + 0.4f * GameState.FpsFactor, 286f);

            if (trigger == "173scene_timer")
                e.EventState += GameState.FpsFactor;
            else if (trigger == "173scene_activated")
                e.EventState = MathUtil.Max(e.EventState, 500f);

            if (e.EventState < 850f && NPCSystem.Curr173 != null)
            {
                PositionEntity(NPCSystem.Curr173.Collider,
                    room.x + 32f * rs, 0.31f, room.z + 1072f * rs, true);
                HideEntity(NPCSystem.Curr173.Obj);
            }

            if (e.EventState >= 500f)
            {
                e.EventState += GameState.FpsFactor;
                UpdateAlarmBreach(e, ctx, rs, trigger);
            }

            AnimateAlarmProps(ctx, e.EventState);
            UpdateAlarmAudio(e);

            if (e.EventState3 < 11f)
                UpdateAlarmStingers(e);

            UpdateAlarmCommotion(e, ctx);
        }

        private static void ActivateAlarmRoom(GameEvent e, EventRoomContext ctx, float rs)
        {
            var room = e.Room;

            if (ctx.RoomDoors[2] != null) ctx.RoomDoors[2].Open = true;

            RenderSystem.FogEnabled = true;
            RenderSystem.FogEnd = 25f;

            if (GameState.SelectedDifficulty?.SaveType == SaveType.SaveOnScreens)
            {
                GameState.Msg = "Saving is only permitted on clickable monitors scattered throughout the facility.";
                GameState.MsgTimer = 70f * 8f;
            }
            else
            {
                GameState.Msg = "Press SAVE to save.";
                GameState.MsgTimer = 70f * 4f;
            }

            if (NPCSystem.Curr173 != null) NPCSystem.Curr173.Idle = false;

            var door1 = ctx.RoomDoors[1];
            if (door1 != null)
            {
                while (door1.OpenState < 180f)
                {
                    door1.OpenState = MathUtil.Min(180f, door1.OpenState + 0.8f);
                    MoveEntity(door1.Obj, Sin(door1.OpenState) / 180f, 0f, 0f);
                    if (door1.Obj2 != -1)
                        MoveEntity(door1.Obj2, -Sin(door1.OpenState) / 180f, 0f, 0f);
                }
            }

            if (ctx.Npc[0] != null)
            {
                ctx.Npc[0].Frame = 74f;
                ctx.Npc[0].State = 8f;
            }

            if (ctx.Npc[1] == null)
            {
                ctx.Npc[1] = NPCSystem.CreateNpc(NPCSystem.NpcTypeD, 0f, 0f, 0f);
                ctx.Npc[1].TextureId = 3;
            }
            PositionEntity(ctx.Npc[1].Collider, room.x, 0.5f, room.z - 1f, true);
            ResetEntity(ctx.Npc[1].Collider);
            ctx.Npc[1].Frame = 210f;

            if (ctx.Npc[2] == null)
                ctx.Npc[2] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard, 0f, 0f, 0f);
            PositionEntity(ctx.Npc[2].Collider, room.x, 0.5f, room.z + 528f * rs, true);
            ResetEntity(ctx.Npc[2].Collider);
            ctx.Npc[2].State = 7f;
            PointEntity(ctx.Npc[2].Collider, ctx.Npc[1].Collider);

            if (ctx.Npc[0] == null && ctx.Objects[2] != -1)
            {
                SetupAlarmObservationNpcs(e, ctx, rs);
            }

            e.EventState = 1f;
            _alarmCommotionPlayed = false;
        }

        private static void SetupAlarmObservationNpcs(GameEvent e, EventRoomContext ctx, float rs)
        {
            var room = e.Room;

            ctx.Npc[3] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard,
                EntityX(ctx.Objects[2], true), EntityY(ctx.Objects[2], true), EntityZ(ctx.Objects[2], true));
            RotateEntity(ctx.Npc[3].Collider, 0f, 90f, 0f);
            ctx.Npc[3].Frame = 286f;
            ctx.Npc[3].State = 8f;
            MoveEntity(ctx.Npc[3].Collider, 1f, 0f, 0f);

            ctx.Npc[4] = NPCSystem.CreateNpc(NPCSystem.NpcTypeD,
                EntityX(ctx.Objects[3], true), 0.5f, EntityZ(ctx.Objects[3], true));
            ctx.Npc[4].Frame = 19f;
            ctx.Npc[4].State = 3f;
            RotateEntity(ctx.Npc[4].Collider, 0f, 270f, 0f);
            MoveEntity(ctx.Npc[4].Collider, 0f, 0f, 2.65f);

            ctx.Npc[5] = NPCSystem.CreateNpc(NPCSystem.NpcTypeD,
                EntityX(ctx.Objects[4], true), 0.5f, EntityZ(ctx.Objects[4], true));
            ctx.Npc[5].TextureId = 6;
            ctx.Npc[5].Frame = 19f;
            ctx.Npc[5].State = 3f;
            RotateEntity(ctx.Npc[5].Collider, 0f, 270f, 0f);
            MoveEntity(ctx.Npc[5].Collider, 0.25f, 0f, 3f);
            RotateEntity(ctx.Npc[5].Collider, 0f, 0f, 0f);

            float ox = EntityX(room.obj, true) + 3712f * rs;
            float oy = 384f * rs;
            float oz = EntityZ(room.obj, true) + 1312f * rs;

            for (int i = 3; i <= 5; i++)
            {
                var n = ctx.Npc[i];
                if (n == null) continue;
                PositionEntity(n.Collider,
                    ox + (EntityX(n.Collider) - EntityX(room.obj)),
                    oy + EntityY(n.Collider) + 0.4f,
                    oz + (EntityZ(n.Collider) - EntityZ(room.obj)));
                ResetEntity(n.Collider);
            }
        }

        private static void UpdateAlarmBreach(GameEvent e, EventRoomContext ctx, float rs, string trigger)
        {
            var room = e.Room;
            var guard = ctx.Npc[2];
            var scientist = ctx.Npc[1];
            var curr173 = NPCSystem.Curr173;

            if (e.EventState2 != 0f) return;

            if (curr173 != null)
                ShowEntity(curr173.Obj);

            var exitDoor = ctx.RoomDoors[5];
            bool exitOpen = exitDoor != null && exitDoor.Open;

            if (e.EventState > 900f && exitOpen)
            {
                if (e.EventState - GameState.FpsFactor <= 900f && scientist != null)
                {
                    scientist.Sfx = AudioSystem.Load("SFX/Room/Intro/WhatThe.ogg");
                    AudioSystem.PlaySound2(scientist.Sfx, GameState.Camera, scientist.Collider);
                }

                if (scientist != null)
                {
                    scientist.State = 3f;
                    scientist.CurrSpeed = (int)MathUtil.CurveValue(-0.008f, scientist.CurrSpeed, 5f);
                    scientist.Frame = MathUtil.Max(scientist.Frame + scientist.CurrSpeed * 18f * GameState.FpsFactor, 236f);
                    RotateEntity(scientist.Collider, 0f, 0f, 0f);
                }

                if (e.EventState > 900f + 2.5f * 70f && guard != null && guard.State != 1f)
                {
                    guard.CurrSpeed = (int)MathUtil.CurveValue(-0.012f, guard.CurrSpeed, 5f);
                    guard.Frame = MathUtil.Max(guard.Frame + guard.CurrSpeed * 40f * GameState.FpsFactor, 76f);
                    MoveEntity(guard.Collider, 0f, 0f, guard.CurrSpeed * GameState.FpsFactor);
                    guard.State = 8f;

                    if (EntityZ(guard.Collider, true) < room.z)
                    {
                        PointEntity(guard.Obj, scientist?.Collider ?? guard.Collider);
                        RotateEntity(guard.Collider, 0f,
                            MathUtil.CurveAngle(EntityYaw(guard.Obj) - 180f, EntityYaw(guard.Collider), 15f), 0f);
                    }
                    else
                    {
                        RotateEntity(guard.Collider, 0f, 0f, 0f);
                    }
                }

                if (e.EventState < 900f + 4f * 70f && curr173 != null)
                {
                    PositionEntity(curr173.Collider, room.x + 32f * rs, 0.31f, room.z + 1072f * rs, true);
                    RotateEntity(curr173.Collider, 0f, 190f, 0f);

                    if (e.EventState > 900f + 70f && e.EventState < 900f + 2.5f * 70f && guard != null)
                    {
                        guard.Frame = MathUtil.Min(guard.Frame + 0.2f * GameState.FpsFactor, 1553f);
                        PointEntity(guard.Obj, curr173.Collider);
                        RotateEntity(guard.Collider, 0f,
                            MathUtil.CurveAngle(EntityYaw(guard.Obj), EntityYaw(guard.Collider), 15f), 0f);
                    }
                }
                else if (curr173 != null)
                {
                    if (e.EventState - GameState.FpsFactor < 900f + 4f * 70f)
                    {
                        var light = AudioSystem.Load("SFX/Room/Intro/Light2.ogg");
                        light?.Play(GameState.SfxVolume, 0f, 0f);
                        GameState.LightBlink = 3f;
                        AudioSystem.PlayStoneDrag(GameState.Camera, curr173.Collider);
                        PointEntity(curr173.Collider, guard?.Collider ?? curr173.Collider);
                        if (EntityY(GameState.Collider) < 320f * rs)
                            GameState.BlinkTimer = -10f;
                    }

                    PositionEntity(curr173.Collider, room.x - 96f * rs, 0.31f, room.z + 592f * rs, true);
                    RotateEntity(curr173.Collider, 0f, 190f, 0f);

                    if (guard != null && guard.State != 1f && GameState.KillTimer >= 0f)
                    {
                        if (EntityZ(guard.Collider, true) < room.z - 1150f * rs)
                        {
                            if (exitDoor != null) exitDoor.Open = false;
                            GameState.LightBlink = 3f;
                            var light = AudioSystem.Load("SFX/Room/Intro/Light2.ogg");
                            light?.Play(GameState.SfxVolume, 0f, 0f);
                            GameState.BlinkTimer = -10f;
                            AudioSystem.PlayStoneDrag(GameState.Camera, curr173.Collider);

                            if (EntityDistance(curr173.Collider, GameState.Collider) < 2.5f
                                && System.Math.Abs(EntityY(GameState.Collider) - EntityY(curr173.Collider)) < 1f)
                            {
                                PositionEntity(curr173.Collider,
                                    EntityX(GameState.Collider), EntityY(GameState.Collider), EntityZ(GameState.Collider));
                            }
                            else
                            {
                                PositionEntity(curr173.Collider, 0f, 0f, 0f);
                            }
                            ResetEntity(curr173.Collider);
                            GameState.Msg = "Hold SPRINT to run.";
                            GameState.MsgTimer = 70f * 8f;
                        }
                    }
                }

                if (trigger == "173scene_end" && guard != null
                    && EntityVisible(guard.Collider, GameState.Collider) && !GameState.NoTarget)
                {
                    guard.State = 1f;
                    guard.State3 = 1f;
                }
                else if (guard != null && guard.State == 1f
                    && !EntityVisible(guard.Collider, GameState.Collider))
                {
                    guard.State = 0f;
                    guard.State3 = 0f;
                }

                if (guard != null && guard.State == 1f && exitDoor != null)
                    exitDoor.Open = true;
            }
            else
            {
                GameState.CanSave = true;
                if (guard == null || guard.State != 1f)
                {
                    if (EntityX(GameState.Collider, true) < room.x + 1384f * rs)
                        e.EventState = MathUtil.Max(e.EventState, 900f);

                    if (exitDoor != null && exitDoor.OpenState <= 0f)
                    {
                        if (scientist != null) NPCSystem.Remove(scientist);
                        if (guard != null) NPCSystem.Remove(guard);
                        e.EventState2 = 1f;
                    }
                }
            }
        }

        private static void AnimateAlarmProps(EventRoomContext ctx, float eventState)
        {
            if (ctx.Objects[0] != -1)
            {
                float drop = -MathUtil.Max(eventState - 1300f, 0f) / 4500f;
                PositionEntity(ctx.Objects[0], EntityX(ctx.Objects[0], true), drop, EntityZ(ctx.Objects[0], true), true);
                RotateEntity(ctx.Objects[0], -MathUtil.Max(eventState - 1320f, 0f) / 130f, 0f,
                    -MathUtil.Max(eventState - 1300f, 0f) / 40f, true);
            }

            if (ctx.Objects[1] != -1)
            {
                float drop = -MathUtil.Max(eventState - 1800f, 0f) / 5000f;
                PositionEntity(ctx.Objects[1], EntityX(ctx.Objects[1], true), drop, EntityZ(ctx.Objects[1], true), true);
                RotateEntity(ctx.Objects[1], -MathUtil.Max(eventState - 2040f, 0f) / 135f, 0f,
                    -MathUtil.Max(eventState - 2040f, 0f) / 43f, true);
            }

            if (ctx.Objects[0] != -1 && EntityDistance(ctx.Objects[0], GameState.Collider) < 2.5f && Rand(300) == 2)
            {
                var decay = AudioSystem.Load("SFX/Room/Decay" + Rand(1, 3) + ".ogg");
                AudioSystem.PlaySound2(decay, GameState.Camera, ctx.Objects[0], 3f);
            }
        }

        private static void UpdateAlarmAudio(GameEvent e)
        {
            if (e.EventState >= 2000f) return;

            _alarmLoopSfx ??= AudioSystem.Load("SFX/Alarm/Alarm.ogg");
            if (_alarmLoopSfx != null && (e.SoundChn < 0 || e.SoundChn == 0))
                _alarmLoopSfx.Play(GameState.SfxVolume * 0.5f, 0f, 0f);
        }

        private static void UpdateAlarmStingers(GameEvent e)
        {
            float prev = e.EventState3;
            e.EventState3 += GameState.FpsFactor / 70f;

            if ((int)prev < 8 && (int)e.EventState3 == 8)
                PlayerSystem.SetCameraShake(1f);

            if ((int)e.EventState3 > (int)prev && (int)e.EventState3 <= 10)
            {
                var sfx = AudioSystem.Load("SFX/Alarm/Alarm2_" + (int)e.EventState3 + ".ogg");
                sfx?.Play(GameState.SfxVolume, 0f, 0f);
            }
        }

        private static void UpdateAlarmCommotion(GameEvent e, EventRoomContext ctx)
        {
            int prevMod = (int)e.EventState % 600;
            int nextMod = (int)(e.EventState + GameState.FpsFactor) % 600;
            if (!(prevMod > 300 && nextMod < 300)) return;

            int i = (int)((e.EventState - 5000f) / 600f) + 1;

            if (i == 0 && !_alarmCommotionPlayed)
            {
                var pa = AudioSystem.Load("SFX/Room/Intro/PA/scripted/scripted6.ogg");
                pa?.Play(GameState.SfxVolume, 0f, 0f);
                _alarmCommotionPlayed = true;
            }

            if (i > 0 && i < 26 && !CommotionState(i))
            {
                var comm = AudioSystem.Load("SFX/Room/Intro/Commotion/Commotion" + i + ".ogg");
                comm?.Play(GameState.SfxVolume, 0f, 0f);
                SetCommotionState(i, true);
            }

            if (i > 26)
            {
                if (ctx.Npc[0] != null) NPCSystem.Remove(ctx.Npc[0]);
                if (ctx.Objects[0] != -1) { FreeEntity(ctx.Objects[0]); ctx.Objects[0] = -1; }
                if (ctx.Objects[1] != -1) { FreeEntity(ctx.Objects[1]); ctx.Objects[1] = -1; }
                RemoveEvent(e);
            }
        }
    }
}