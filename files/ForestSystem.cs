// ForestSystem.cs — stub for SCP-860 forest generation (MapSystem.bb)

namespace SCPCB360.GameLogic
{
    public static class ForestSystem
    {
        public static void PlaceForest(RoomInstance room)
        {
            if (room == null || ZoneInfo.HasCustomForest) return;
            // Procedural forest placement deferred — room860 doors/items still spawn.
        }
    }
}