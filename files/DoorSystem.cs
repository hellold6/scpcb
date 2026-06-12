// DoorSystem.cs — ports Doors type + CreateDoor/UpdateDoors/UseDoor from Main.bb

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SCPCB360.Engine;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public class Door
    {
        public int Obj = -1;
        public int Obj2 = -1;
        public int FrameObj = -1;
        public int[] Buttons = { -1, -1 };

        public bool Locked;
        public bool Open;
        public float Angle;
        public float OpenState;
        public bool FastOpen;
        public int Dir;
        public float Timer;
        public float TimerState;
        public int KeyCard;
        public RoomInstance Room;
        public bool DisableWaypoint;
        public float Dist;
        public int SoundChn = -1;
        public string Code = "";
        public int Id;
        public int Level;
        public int LevelDest = 66;
        public bool AutoClose = true;
        public Door LinkedDoor;
        public bool IsElevatorDoor;
        public bool MtfClose = true;
        public bool MTFClose { get => MtfClose; set => MtfClose = value; }
        public bool NpcCalledElevator;
        public int DoorHitObj = -1;
    }

    public static class DoorSystem
    {
        private static readonly List<Door> _doors = new();
        private static int _doorTempId;
        private static int _doorObj = -1;
        private static int _doorFrameObj = -1;

        public static IReadOnlyList<Door> All => _doors;

        public static void Initialize()
        {
            _doorObj = LoadMesh("GFX/map/door01");
            _doorFrameObj = LoadMesh("GFX/map/doorframe");
        }

        public static Door CreateDoor(int lvl, float x, float y, float z, float angle,
            RoomInstance room, bool dopen = false, int big = 0, int keycard = 0,
            string code = "", bool useCollisionMesh = false)
        {
            int parent = room?.mesh ?? -1;

            var d = new Door
            {
                Level = lvl,
                Room = room,
                KeyCard = keycard,
                Code = code,
                Open = dopen,
                Id = _doorTempId++,
            };

            if (big == 1)
            {
                d.Obj = CopyEntity(_doorObj, parent);
                d.Obj2 = CopyEntity(_doorObj, parent);
                ScaleEntity(d.Obj, 55 * GameState.RoomScale, 55 * GameState.RoomScale, 55 * GameState.RoomScale);
                ScaleEntity(d.Obj2, 55 * GameState.RoomScale, 55 * GameState.RoomScale, 55 * GameState.RoomScale);
            }
            else
            {
                d.Obj = CopyEntity(_doorObj, parent);
                d.Obj2 = CopyEntity(_doorObj, parent);
                d.FrameObj = CopyEntity(_doorFrameObj, parent);
            }

            PositionEntity(d.FrameObj != -1 ? d.FrameObj : d.Obj, x, y, z);
            RotateEntity(d.Obj, 0, angle, 0);
            if (d.Obj2 != -1) RotateEntity(d.Obj2, 0, angle, 0);

            EntityType(d.Obj, 2);
            if (d.Obj2 != -1) EntityType(d.Obj2, 2);

            for (int i = 0; i < 2; i++)
                d.Buttons[i] = CreatePivot(d.FrameObj != -1 ? d.FrameObj : d.Obj);

            _doors.Add(d);
            return d;
        }

        public static Door ClosestDoor;
        public static int ClosestButton = -1;

        public static void Update()
        {
            ClosestDoor = null;
            ClosestButton = -1;
            float bestDist = float.MaxValue;

            foreach (var d in _doors)
            {
                float target = d.Open ? 100f : 0f;
                float speed = d.FastOpen ? 8f : 2f;

                if (Math.Abs(d.OpenState - target) > 0.1f)
                {
                    d.OpenState = MathUtil.CurveValue(target, d.OpenState, speed);
                    float slide = d.OpenState * 0.01f;
                    MoveEntity(d.Obj, 0, 0, slide);
                    if (d.Obj2 != -1) MoveEntity(d.Obj2, 0, 0, -slide);
                }

                if (d.AutoClose && d.Open && d.OpenState >= 99f)
                {
                    d.Timer -= GameState.FpsFactor;
                    if (d.Timer <= 0f)
                    {
                        d.Open = false;
                        d.Timer = 0f;
                    }
                }

                if (GameState.Camera == -1) continue;
                for (int i = 0; i < d.Buttons.Length; i++)
                {
                    if (d.Buttons[i] == -1) continue;
                    float dist = EntityDistance(GameState.Camera, d.Buttons[i]);
                    if (dist < 1.5f && dist < bestDist)
                    {
                        bestDist = dist;
                        ClosestDoor = d;
                        ClosestButton = d.Buttons[i];
                    }
                }
            }

            GameState.ClosestDoor = ClosestDoor;
            GameState.ClosestButton = ClosestButton;
        }

        public static bool UseDoor(Door d, bool showMsg = true, bool playSfx = true)
        {
            if (d == null) return false;

            if (d.KeyCard > 0)
            {
                var selected = ItemSystem.SelectedItem;
                if (selected == null)
                {
                    if (showMsg && ShouldShowKeycardMsg())
                    {
                        GameState.Msg = "A keycard is required to operate this door.";
                        GameState.MsgTimer = 70f * 7f;
                    }
                    return false;
                }

                int level = selected.Template.TempName switch
                {
                    "key1" => 1,
                    "key2" => 2,
                    "key3" => 3,
                    "key4" => 4,
                    "key5" => 5,
                    "key6" => 6,
                    _ => -1,
                };

                if (level < 0)
                {
                    if (showMsg && ShouldShowKeycardMsg())
                    {
                        GameState.Msg = "A keycard is required to operate this door.";
                        GameState.MsgTimer = 70f * 7f;
                    }
                    return false;
                }

                if (level < d.KeyCard)
                {
                    ItemSystem.SelectedItem = null;
                    if (showMsg)
                    {
                        AudioSystem.PlayKeyCardSound(false);
                        GameState.Msg = d.Locked
                            ? "The keycard was inserted into the slot but nothing happened."
                            : $"A keycard with security clearance {d.KeyCard} or higher is required to operate this door.";
                        GameState.MsgTimer = 70f * 7f;
                    }
                    return false;
                }

                ItemSystem.SelectedItem = null;
                if (showMsg)
                {
                    if (d.Locked)
                    {
                        AudioSystem.PlayKeyCardSound(false);
                        GameState.Msg = "The keycard was inserted into the slot but nothing happened.";
                        GameState.MsgTimer = 70f * 7f;
                        return false;
                    }

                    AudioSystem.PlayKeyCardSound(true);
                    GameState.Msg = "The keycard was inserted into the slot.";
                    GameState.MsgTimer = 70f * 7f;
                }
            }
            else if (d.KeyCard < 0)
            {
                bool handOk = ItemSystem.SelectedItem != null &&
                    ((ItemSystem.SelectedItem.Template.TempName == "hand" && d.KeyCard == -1) ||
                     (ItemSystem.SelectedItem.Template.TempName == "hand2" && d.KeyCard == -2));
                ItemSystem.SelectedItem = null;

                if (!handOk)
                {
                    if (showMsg)
                    {
                        AudioSystem.PlayInteractSound();
                        GameState.Msg = "You placed your palm onto the scanner. The scanner reads: \"DNA does not match known sample. Access denied.\"";
                        GameState.MsgTimer = 70f * 10f;
                    }
                    return false;
                }

                if (showMsg)
                {
                    AudioSystem.PlayInteractSound();
                    GameState.Msg = "You place the palm of the hand onto the scanner. The scanner reads: \"DNA verified. Access granted.\"";
                    GameState.MsgTimer = 70f * 10f;
                }
            }
            else if (d.Locked)
            {
                if (showMsg)
                {
                    AudioSystem.PlayInteractSound();
                    GameState.Msg = d.Open
                        ? "You pushed the button but nothing happened."
                        : "The door appears to be locked.";
                    GameState.MsgTimer = 70f * 5f;
                }
                return false;
            }

            d.Open = !d.Open;
            d.Timer = 180f;
            if (d.LinkedDoor != null)
                d.LinkedDoor.Open = d.Open;

            if (playSfx)
                AudioSystem.PlayDoorSound(d.Open, d.Dir);

            return true;
        }

        private static bool ShouldShowKeycardMsg()
        {
            if (!GameState.Msg.Contains("keycard", StringComparison.OrdinalIgnoreCase))
                return true;
            return GameState.MsgTimer < 70f * 3f;
        }

        public static void RemoveDoor(Door d)
        {
            if (d == null) return;
            FreeEntity(d.Obj);
            if (d.Obj2 != -1) FreeEntity(d.Obj2);
            if (d.FrameObj != -1) FreeEntity(d.FrameObj);
            foreach (int btn in d.Buttons)
                if (btn != -1) FreeEntity(btn);
            _doors.Remove(d);
        }

        public static void FreeAll()
        {
            foreach (var d in _doors.ToArray())
                RemoveDoor(d);
        }

        public static Door FindByFramePosition(float x, float y, float z, float tolerance = 0.05f)
        {
            foreach (var d in _doors)
            {
                int frame = d.FrameObj != -1 ? d.FrameObj : d.Obj;
                if (frame == -1) continue;
                if (Math.Abs(EntityX(frame, true) - x) < tolerance
                    && Math.Abs(EntityY(frame, true) - y) < tolerance
                    && Math.Abs(EntityZ(frame, true) - z) < tolerance)
                    return d;
            }
            return null;
        }

        public static void RestoreState(Door d, bool open, float openState, bool locked, bool autoClose,
            float objX, float objZ, float obj2X, float obj2Z, float timer, float timerState,
            bool isElevator, bool mtfClose)
        {
            if (d == null) return;
            d.Open = open;
            d.OpenState = openState;
            d.Locked = locked;
            d.AutoClose = autoClose;
            d.Timer = timer;
            d.TimerState = timerState;
            d.IsElevatorDoor = isElevator;
            d.MtfClose = mtfClose;
            PositionEntity(d.Obj, objX, EntityY(d.Obj, true), objZ, true);
            if (d.Obj2 != -1)
                PositionEntity(d.Obj2, obj2X, EntityY(d.Obj2, true), obj2Z, true);
        }
    }
}