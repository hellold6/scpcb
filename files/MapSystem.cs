// MapSystem.cs
// Ports CB's MapSystem.bb to C#.
// Handles room loading, zone tracking, and the room entity graph.
//
// CB's map is procedurally assembled from discrete room prefabs.
// Each room is a .rmesh (→ cooked .xnb mesh) with an attached list of
// spawn pivots, object slots, and door connection points.
//
// Original pattern:
//   Type rooms
//     Field mesh, obj, zone
//     Field x#, y#, z#, rx#, ry#, rz#
//   End Type
//
//   LoadRoom("room2", x#, y#, z#, rx#, ry#, rz#)

using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    // ── Room prefab descriptor ────────────────────────────────────────────────────

    public class RoomDef
    {
        public string Name;       // e.g. "room2"
        public int    Zone;       // 0=light, 1=heavy, 2=entrance
        // Door connection offsets in local space (used for procedural assembly)
        public Vector3[] DoorOffsets;
    }

    // ── Runtime room instance — mirrors BlitzBasic Type rooms ────────────────────

    public class RoomInstance
    {
        public int mesh   = -1;   // entity handle for the room mesh
        public int obj    = -1;   // entity handle for attached objects pivot
        public int zone   =  0;   // zone identifier

        // World-space transform (stored separately for quick queries)
        public float x, y, z;
        public float rx, ry, rz;

        // Door connection handles (child pivots parented to mesh)
        public List<int> doorPivots = new();

        // Object spawn points loaded from the .rmesh spawn data
        public List<(Vector3 pos, string objType)> spawnPoints = new();

        public RoomDef def;
    }

    // ─────────────────────────────────────────────────────────────────────────────

    public static class MapSystem
    {
        // The equivalent of Blitz3D's "Each rooms" list
        private static readonly List<RoomInstance> _rooms = new();
        public static IReadOnlyList<RoomInstance> All => _rooms;

        // Zone totals used by CB's procedural generation seed checks
        public static int RoomCount     => _rooms.Count;
        public static int Zone0Count    => CountByZone(0);
        public static int Zone1Count    => CountByZone(1);

        // ─── Room catalogue (matches GFX\map\ folder contents) ───────────────────

        private static readonly Dictionary<string, RoomDef> _catalogue = new()
        {
            // Entrance zone (zone 2)
            ["checkpoint_entrance_1"] = new() { Name = "checkpoint_entrance_1", Zone = 2,
                DoorOffsets = new[] { new Vector3(0, 0, 7.5f), new Vector3(0, 0, -7.5f) } },
            ["office"] = new() { Name = "office", Zone = 2,
                DoorOffsets = new[] { new Vector3(0, 0, 7.5f) } },

            // Light containment (zone 0)
            ["room2"] = new() { Name = "room2", Zone = 0,
                DoorOffsets = new[] { new Vector3(0,0,7.5f), new Vector3(0,0,-7.5f) } },
            ["room2_2doors"] = new() { Name = "room2_2doors", Zone = 0,
                DoorOffsets = new[] { new Vector3(7.5f,0,0), new Vector3(-7.5f,0,0),
                                      new Vector3(0,0,-7.5f) } },
            ["room3"] = new() { Name = "room3", Zone = 0,
                DoorOffsets = new[] { new Vector3(0,0,7.5f), new Vector3(7.5f,0,0),
                                      new Vector3(-7.5f,0,0) } },
            ["room4"] = new() { Name = "room4", Zone = 0,
                DoorOffsets = new[] { new Vector3(0,0,7.5f), new Vector3(0,0,-7.5f),
                                      new Vector3(7.5f,0,0), new Vector3(-7.5f,0,0) } },

            // Heavy containment (zone 1)
            ["room_hcz_173"] = new() { Name = "room_hcz_173", Zone = 1,
                DoorOffsets = new[] { new Vector3(0,0,7.5f), new Vector3(0,0,-7.5f) } },
        };

        // ─────────────────────────────────────────────────────────────────────────
        // LoadRoom — mirrors CB's LoadRoom(name$, x#, y#, z#, rx#, ry#, rz#)
        // ─────────────────────────────────────────────────────────────────────────

        public static RoomInstance LoadRoom(string name, float x, float y, float z,
                                             float rx = 0, float ry = 0, float rz = 0)
        {
            if (!_catalogue.TryGetValue(name, out var def))
            {
                System.Diagnostics.Debug.WriteLine($"[Map] Unknown room: {name}");
                def = new RoomDef { Name = name, Zone = 0, DoorOffsets = System.Array.Empty<Vector3>() };
            }

            var room = new RoomInstance { def = def, x = x, y = y, z = z,
                                          rx = rx, ry = ry, rz = rz, zone = def.Zone };

            // Load the cooked mesh
            room.mesh = LoadMesh($"GFX/map/{name}");
            PositionEntity(room.mesh, x, y, z);
            RotateEntity(room.mesh, rx, ry, rz);

            // Create an objects pivot parented to the mesh so items rotate with the room
            room.obj = CreatePivot(room.mesh);

            // Create door pivots from definition
            foreach (var offset in def.DoorOffsets)
            {
                int dp = CreatePivot(room.mesh);
                var rotated = Vector3.Transform(offset,
                    Microsoft.Xna.Framework.Matrix.CreateRotationY(
                        MathHelper.ToRadians(ry)));
                PositionEntity(dp, x + rotated.X, y + rotated.Y, z + rotated.Z);
                room.doorPivots.Add(dp);
            }

            _rooms.Add(room);
            return room;
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Procedural map generation stub
        // Mirrors CB's main map generation loop.
        // Full implementation needs CB's seed system; this is the skeleton.
        // ─────────────────────────────────────────────────────────────────────────

        public static void GenerateMap(int seed)
        {
            var rng = new System.Random(seed);

            // Entrance zone — always fixed layout
            LoadRoom("checkpoint_entrance_1", 0,   0, 0,  0, 0, 0);
            LoadRoom("office",                0,   0, 15, 0, 0, 0);

            // Light containment — procedural chain
            float curX = 0, curZ = 30;
            float curRY = 0;
            for (int i = 0; i < 20; i++)
            {
                string[] lcz = { "room2", "room2_2doors", "room3", "room4" };
                string pick  = lcz[rng.Next(lcz.Length)];

                LoadRoom(pick, curX, 0, curZ, 0, curRY, 0);

                // Advance along the current facing direction
                float rad = MathHelper.ToRadians(curRY);
                curX += (float)Math.Sin(rad) * 15f;
                curZ += (float)Math.Cos(rad) * 15f;

                // Randomly turn at junctions
                if (rng.NextDouble() < 0.3f)
                    curRY += rng.NextDouble() < 0.5f ? 90f : -90f;
            }

            // Heavy containment — place key rooms
            LoadRoom("room_hcz_173", curX, 0, curZ, 0, curRY, 0);

            System.Diagnostics.Debug.WriteLine($"[Map] Generated {_rooms.Count} rooms (seed={seed})");
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Cleanup
        // ─────────────────────────────────────────────────────────────────────────

        public static void FreeAllRooms()
        {
            foreach (var r in _rooms)
            {
                foreach (var dp in r.doorPivots) FreeEntity(dp);
                FreeEntity(r.obj);
                FreeEntity(r.mesh);
            }
            _rooms.Clear();
        }

        private static int CountByZone(int zone)
        {
            int c = 0;
            foreach (var r in _rooms) if (r.zone == zone) c++;
            return c;
        }
    }
}
