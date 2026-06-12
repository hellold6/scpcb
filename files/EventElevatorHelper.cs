// EventElevatorHelper.cs — ports UpdateElevators from MapSystem.bb

using System;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static class EventElevatorHelper
    {
        private const float CabinRadius = 280f * GameState.RoomScale;

        public static float UpdateElevators(float state, Door door1, Door door2,
            int room1, int room2, GameEvent ev, bool ignoreRotation = true)
        {
            if (door1 == null || door2 == null) return state;

            door1.IsElevatorDoor = true;
            door2.IsElevatorDoor = true;

            if (door1.Open && !door2.Open && door1.OpenState >= 179f)
            {
                state = -1f;
                door1.Locked = false;
            }
            else if (door2.Open && !door1.Open && door2.OpenState >= 179f)
            {
                state = 1f;
                door2.Locked = false;
            }
            else if (Math.Abs(door1.OpenState - door2.OpenState) < 0.2f)
            {
                door1.IsElevatorDoor = false;
                door2.IsElevatorDoor = false;
            }

            door1.Locked = true;
            door2.Locked = true;

            if (door1.Open && InCabin(room1))
            {
                door1.Locked = false;
            }

            if (door2.Open && InCabin(room2))
            {
                door2.Locked = false;
            }

            if (!door1.Open && !door2.Open &&
                door1.OpenState < 0.2f && door2.OpenState < 0.2f)
            {
                door1.Locked = true;
                door2.Locked = true;

                if (state < 0f)
                {
                    state -= GameState.FpsFactor;
                    if (InCabin(room1))
                        PlayerSystem.SetCameraShake((float)Math.Sin(Math.Abs(state) / 3f) * 0.3f);

                    if (state < -500f)
                    {
                        TeleportCabin(door1, door2, room1, room2, ignoreRotation);
                        state = 0f;
                        AudioSystem.PlaySound2(AudioSystem.Load("SFX/Door/ElevatorBeep.ogg"),
                            GameState.Camera, room1);
                    }
                }
                else if (state > 0f)
                {
                    state += GameState.FpsFactor;
                    if (InCabin(room2))
                        PlayerSystem.SetCameraShake((float)Math.Sin(Math.Abs(state) / 3f) * 0.3f);

                    if (state > 500f)
                    {
                        TeleportCabin(door2, door1, room2, room1, ignoreRotation);
                        state = 0f;
                        AudioSystem.PlaySound2(AudioSystem.Load("SFX/Door/ElevatorBeep.ogg"),
                            GameState.Camera, room2);
                    }
                }
            }

            return state;
        }

        private static bool InCabin(int pivot)
        {
            if (pivot == -1 || GameState.Collider == -1) return false;
            return Math.Abs(EntityX(GameState.Collider, true) - EntityX(pivot, true)) < CabinRadius &&
                   Math.Abs(EntityZ(GameState.Collider, true) - EntityZ(pivot, true)) < CabinRadius &&
                   Math.Abs(EntityY(GameState.Collider, true) - EntityY(pivot, true)) < CabinRadius;
        }

        private static void TeleportCabin(Door fromDoor, Door toDoor, int fromPivot, int toPivot, bool ignoreRotation)
        {
            if (GameState.Collider != -1 && InCabin(fromPivot))
            {
                float ox = MathUtil.Max(-CabinRadius + 0.22f, MathUtil.Min(CabinRadius - 0.22f,
                    EntityX(GameState.Collider, true) - EntityX(fromPivot, true)));
                float oz = MathUtil.Max(-CabinRadius + 0.22f, MathUtil.Min(CabinRadius - 0.22f,
                    EntityZ(GameState.Collider, true) - EntityZ(fromPivot, true)));
                PositionEntity(GameState.Collider,
                    EntityX(toPivot, true) + ox,
                    EntityY(toPivot, true) + (EntityY(GameState.Collider, true) - EntityY(fromPivot, true)) + 0.1f,
                    EntityZ(toPivot, true) + oz, true);
                ResetEntity(GameState.Collider);
                GameState.DropSpeed = 0f;
            }

            foreach (var n in NPCSystem.All)
            {
                if (n.Collider == -1) continue;
                if (Math.Abs(EntityX(n.Collider, true) - EntityX(fromPivot, true)) >= CabinRadius) continue;
                if (Math.Abs(EntityZ(n.Collider, true) - EntityZ(fromPivot, true)) >= CabinRadius) continue;
                if (Math.Abs(EntityY(n.Collider, true) - EntityY(fromPivot, true)) >= CabinRadius) continue;

                float ox = MathUtil.Max(-CabinRadius + 0.22f, MathUtil.Min(CabinRadius - 0.22f,
                    EntityX(n.Collider, true) - EntityX(fromPivot, true)));
                float oz = MathUtil.Max(-CabinRadius + 0.22f, MathUtil.Min(CabinRadius - 0.22f,
                    EntityZ(n.Collider, true) - EntityZ(fromPivot, true)));
                PositionEntity(n.Collider,
                    EntityX(toPivot, true) + ox,
                    EntityY(toPivot, true) + (EntityY(n.Collider, true) - EntityY(fromPivot, true)) + 0.1f,
                    EntityZ(toPivot, true) + oz, true);
            }

            toDoor.Open = true;
            fromDoor.Open = false;
            toDoor.Locked = false;
            fromDoor.Locked = true;
        }
    }
}