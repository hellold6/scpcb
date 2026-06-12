// TriggerSystem.cs — ports Main.bb CheckTriggers() (position fallbacks until RMESH triggers load)

using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static class TriggerSystem
    {
        public static string CheckTriggers()
        {
            var room = GameState.PlayerRoom;
            if (room == null || GameState.Collider == -1) return "";

            if (room.TriggerNames != null && room.Triggers != null)
            {
                float px = EntityX(GameState.Collider, true);
                float py = EntityY(GameState.Collider, true);
                float pz = EntityZ(GameState.Collider, true);

                for (int i = 0; i < room.TriggerNames.Length && i < room.Triggers.Length; i++)
                {
                    if (room.Triggers[i] == -1 || string.IsNullOrEmpty(room.TriggerNames[i]))
                        continue;

                    float dist = EntityDistance(GameState.Collider, room.Triggers[i]);
                    if (dist < 1.5f)
                        return room.TriggerNames[i];
                }
            }

            return FallbackTrigger(room);
        }

        private static string FallbackTrigger(RoomInstance room)
        {
            float rs = GameState.RoomScale;
            float px = EntityX(GameState.Collider, true);
            float py = EntityY(GameState.Collider, true);
            float pz = EntityZ(GameState.Collider, true);

            switch (room.RoomName)
            {
                case "start":
                    if (py > room.y + 300f * rs && px > room.x + 2800f * rs)
                        return "173scene_timer";
                    if (px > room.x + 3200f * rs && pz > room.z + 900f * rs)
                        return "173scene_activated";
                    if (pz < room.z + 200f * rs && px > room.x + 1000f * rs)
                        return "173scene_end";
                    break;
            }

            return "";
        }
    }
}