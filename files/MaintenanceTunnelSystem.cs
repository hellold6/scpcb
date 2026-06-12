// MaintenanceTunnelSystem.cs — ports room2tunnel procedural grid from UpdateEvents.bb

using System;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public class MaintenanceTunnelGrid
    {
        public const int GridSize = 19;
        public int[] Cells = new int[GridSize * GridSize];
        public int[] Angles = new int[GridSize * GridSize];
        public int[] Entities = new int[GridSize * GridSize];
        public bool Generated;
        public int FirstX, FirstY, LastX, LastY;
    }

    public static class MaintenanceTunnelSystem
    {
        public static void EnsureGrid(RoomInstance room)
        {
            if (room == null) return;
            if (room.TunnelGrid != null && room.TunnelGrid.Generated) return;

            room.TunnelGrid ??= new MaintenanceTunnelGrid();
            var g = room.TunnelGrid;
            if (g.Generated) return;

            int oldSeed = Environment.TickCount;
            SeedRnd(MathUtil.GenerateSeedNumber(GameState.RandomSeed));

            const int gs = MaintenanceTunnelGrid.GridSize;
            int dir = Rand(0, 1) << 1;
            int ix = gs / 2 + Rand(-2, 2);
            int iy = gs / 2 + Rand(-2, 2);
            g.Cells[ix + iy * gs] = 1;
            if (dir == 2) g.Cells[(ix + 1) + iy * gs] = 1;
            else g.Cells[(ix - 1) + iy * gs] = 1;

            int count = 2;
            while (count < 100)
            {
                int steps = Rand(1, 5) << Rand(1, 2);
                for (int i = 1; i <= steps; i++)
                {
                    bool ok = true;
                    switch (dir)
                    {
                        case 0: if (ix < gs - 2 - (i % 2)) ix++; else ok = false; break;
                        case 1: if (iy < gs - 2 - (i % 2)) iy++; else ok = false; break;
                        case 2: if (ix > 1 + (i % 2)) ix--; else ok = false; break;
                        case 3: if (iy > 1 + (i % 2)) iy--; else ok = false; break;
                    }
                    if (!ok) break;
                    if (g.Cells[ix + iy * gs] == 0)
                    {
                        g.Cells[ix + iy * gs] = 1;
                        count++;
                    }
                }
                dir = (dir + ((Rand(0, 1) << 1) - 1) + 4) % 4;
            }

            for (int y = 0; y < gs; y++)
            for (int x = 0; x < gs; x++)
            {
                if (g.Cells[x + y * gs] <= 0) continue;
                int n = (g.Cells[x + (y + 1) * gs] > 0 ? 1 : 0) +
                        (g.Cells[x + (y - 1) * gs] > 0 ? 1 : 0) +
                        (g.Cells[(x + 1) + y * gs] > 0 ? 1 : 0) +
                        (g.Cells[(x - 1) + y * gs] > 0 ? 1 : 0);
                g.Cells[x + y * gs] = n;
            }

            FindTunnelEnds(g, out g.FirstX, out g.FirstY, out g.LastX, out g.LastY);

            if (room.Objects[0] != -1)
                PositionEntity(room.Objects[0], room.x + g.FirstX * 2f, 8f, room.z + g.FirstY * 2f, true);
            if (room.Objects[1] != -1)
                PositionEntity(room.Objects[1], room.x + g.LastX * 2f, 8f, room.z + g.LastY * 2f, true);

            for (int y = 0; y < gs; y++)
            for (int x = 0; x < gs; x++)
            {
                if (g.Cells[x + y * gs] <= 0) continue;
                int ent = CreatePivot();
                PositionEntity(ent, room.x + x * 2f, 8f, room.z + y * 2f, true);
                HideEntity(ent);
                g.Entities[x + y * gs] = ent;
            }

            g.Generated = true;
            SeedRnd(oldSeed);
        }

        private static void FindTunnelEnds(MaintenanceTunnelGrid g, out int firstX, out int firstY, out int lastX, out int lastY)
        {
            firstX = firstY = lastX = lastY = -1;
            for (int y = 0; y < MaintenanceTunnelGrid.GridSize; y++)
            for (int x = 0; x < MaintenanceTunnelGrid.GridSize; x++)
            {
                if (g.Cells[x + y * MaintenanceTunnelGrid.GridSize] != 2) continue;
                bool horiz = g.Cells[(x + 1) + y * MaintenanceTunnelGrid.GridSize] > 0 &&
                             g.Cells[(x - 1) + y * MaintenanceTunnelGrid.GridSize] > 0;
                bool vert = g.Cells[x + (y + 1) * MaintenanceTunnelGrid.GridSize] > 0 &&
                            g.Cells[x + (y - 1) * MaintenanceTunnelGrid.GridSize] > 0;
                if (!horiz && !vert) continue;
                if (firstX < 0) { firstX = x; firstY = y; }
                lastX = x; lastY = y;
            }
        }

        public static bool PlayerInTunnelBounds(RoomInstance room)
        {
            if (room == null || GameState.Collider == -1) return false;
            float py = EntityY(GameState.Collider, true);
            if (py < 8f || py > 12f) return false;
            float px = EntityX(GameState.Collider, true);
            float pz = EntityZ(GameState.Collider, true);
            int gs = MaintenanceTunnelGrid.GridSize;
            return px >= room.x - 6f && px <= room.x + 2f * gs + 6f &&
                   pz >= room.z - 6f && pz <= room.z + 2f * gs + 6f;
        }

        public static void SetTunnelVisible(RoomInstance room, bool visible)
        {
            if (room?.TunnelGrid == null) return;
            var g = room.TunnelGrid;
            for (int i = 0; i < g.Entities.Length; i++)
            {
                if (g.Entities[i] == -1) continue;
                if (visible) ShowEntity(g.Entities[i]);
                else HideEntity(g.Entities[i]);
            }
        }
    }
}