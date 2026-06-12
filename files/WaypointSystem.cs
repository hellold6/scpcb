// WaypointSystem.cs — stubs for CreateWaypoint from MapSystem.bb

using System.Collections.Generic;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public class WaypointNode
    {
        public int Obj = -1;
        public Door Door;
        public RoomInstance Room;
        public WaypointNode[] Connected = new WaypointNode[4];
        public float[] Dist = new float[4];
    }

    public static class WaypointSystem
    {
        private static readonly List<WaypointNode> _nodes = new();

        public static WaypointNode Create(float x, float y, float z, Door door, RoomInstance room)
        {
            var w = new WaypointNode
            {
                Door = door,
                Room = room,
                Obj = CreatePivot(),
            };
            PositionEntity(w.Obj, x, y, z, true);
            if (room != null)
                EntityParent(w.Obj, room.obj);
            _nodes.Add(w);
            return w;
        }
    }
}