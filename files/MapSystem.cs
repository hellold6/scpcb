// MapSystem.cs
// Ports CB's MapSystem.bb to C#.
// Handles room loading, zone tracking, procedural map generation, and the room entity graph.

using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    // ── Room prefab descriptor (legacy helper; templates come from rooms.ini) ─────

    public class RoomDef
    {
        public string Name;
        public int Zone;
        public Vector3[] DoorOffsets;
    }

    // ── Runtime room instance — mirrors BlitzBasic Type Rooms ────────────────────

    public class RoomInstance
    {
        public int mesh = -1;
        public int obj = -1;
        public int zone;

        public float x, y, z;
        public float rx, ry, rz;
        public float Angle;

        public int GridX, GridY;

        public RoomTemplate Template;
        public RoomDef def;

        public List<int> doorPivots = new();
        public List<(Vector3 pos, string objType)> spawnPoints = new();
        public SCPCB360.Engine.CollisionMesh collisionMesh;

        public string RoomName => Template?.Name ?? def?.Name ?? "";

        public RoomInstance[] Adjacent = new RoomInstance[4];
        public Door[] RoomDoors = new Door[10];
        public Door[] AdjDoor = new Door[4];
        public int[] Objects = new int[128];
        public int[] Levers = new int[12];
        public int[] Textures = new int[4];
        public int[] NonFreeAble = new int[4];
        public bool Found;

        public float MinX, MinY, MinZ, MaxX, MaxY, MaxZ;

        public MaintenanceTunnelGrid TunnelGrid;

        public int[] Triggers;
        public string[] TriggerNames;
    }

    // ─────────────────────────────────────────────────────────────────────────────

    public static class MapSystem
    {
        public const int ROOM1 = 1;
        public const int ROOM2 = 2;
        public const int ROOM2C = 3;
        public const int ROOM3 = 4;
        public const int ROOM4 = 5;
        public const int ZoneAmount = 3;

        public const int MapWidth = 32;
        public const int MapHeight = 32;
        public const float RoomSpacing = 8.0f;

        public static int ZoneTransition0 = 13;
        public static int ZoneTransition1 = 7;

        private static readonly List<RoomInstance> _rooms = new();
        public static IReadOnlyList<RoomInstance> All => _rooms;

        public static int RoomCount => _rooms.Count;
        public static int Zone0Count => CountByZone(0);
        public static int Zone1Count => CountByZone(1);

        private static int[,] _mapTemp = new int[MapWidth + 1, MapHeight + 1];
        private static bool[,] _mapFound = new bool[MapWidth + 1, MapHeight + 1];
        private static string[,] _mapName = new string[MapWidth, MapHeight];
        private static readonly List<Waypoint> _waypoints = new();

        public static int[,] MapTemp => _mapTemp;
        public static bool[,] MapFoundGrid => _mapFound;

        // ─── Room creation ───────────────────────────────────────────────────────

        public static RoomInstance CreateRoom(int zone, int roomShape, float x, float y, float z,
            string name = "")
        {
            RoomTemplate template = null;

            if (!string.IsNullOrEmpty(name))
            {
                template = RoomTemplateSystem.GetByName(name);
                if (template == null)
                    System.Diagnostics.Debug.WriteLine($"[Map] CreateRoom: unknown template '{name}'");
            }
            else
            {
                template = RoomTemplateSystem.PickForZoneAndShape(zone, roomShape, _createRng);
            }

            if (template == null)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Map] CreateRoom: no template for zone={zone} shape={roomShape} name='{name}'");
                return null;
            }

            var room = new RoomInstance
            {
                Template = template,
                def = new RoomDef
                {
                    Name = template.Name,
                    Zone = zone,
                    DoorOffsets = Array.Empty<Vector3>(),
                },
                zone = zone,
                x = x,
                y = y,
                z = z,
                GridX = (int)(x / RoomSpacing),
                GridY = (int)(z / RoomSpacing),
            };

            EnsureTemplateMesh(template);
            room.mesh = CopyEntity(template.Obj);
            ScaleEntity(room.mesh, GameState.RoomScale, GameState.RoomScale, GameState.RoomScale);
            PositionEntity(room.mesh, x, y, z);

            room.obj = CreatePivot(room.mesh);

            string rmeshPath = ResolveRMeshPath(template);
            if (File.Exists(rmeshPath))
            {
                room.collisionMesh = SCPCB360.Engine.RMeshReader.LoadCollisionMesh(rmeshPath);
                if (TryGet(room.mesh, out var meshEnt) && room.collisionMesh != null)
                {
                    meshEnt.CollisionMesh = room.collisionMesh;
                    meshEnt.CollisionType = 2;
                }
            }

            _rooms.Add(room);
            FillRoomSystem.Fill(room);
            return room;
        }

        public static RoomInstance LoadRoom(string name, float x, float y, float z,
            float rx = 0, float ry = 0, float rz = 0)
        {
            var room = CreateRoom(0, 0, x, y, z, name);
            if (room == null) return null;

            room.rx = rx;
            room.ry = ry;
            room.rz = rz;
            room.Angle = ry;

            if (ry != 0f)
                RotateEntity(room.mesh, rx, ry, rz);

            return room;
        }

        public static RoomInstance FindRoomByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            name = name.ToLowerInvariant();
            foreach (var room in _rooms)
            {
                if (room.RoomName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return room;
            }
            return null;
        }

        // ─── Procedural map generation — ports CreateMap() from MapSystem.bb ─────

        private static Random _createRng;

        public static void CreateMap()
        {
            FreeAllRooms();
            RoomTemplateSystem.LoadRoomTemplates();

            int seed = MathUtil.GenerateSeedNumber(GameState.RandomSeed);
            _createRng = new Random(seed);

            System.Diagnostics.Debug.WriteLine($"[Map] Generating map seed={GameState.RandomSeed} ({seed})");

            ZoneTransition0 = 13;
            ZoneTransition1 = 7;

            Array.Clear(_mapTemp, 0, _mapTemp.Length);
            for (int gx = 0; gx < MapWidth; gx++)
            for (int gy = 0; gy < MapHeight; gy++)
                _mapName[gx, gy] = "";

            GenerateHallwayGrid();
            CountAndAdjustRoomShapes();
            AssignNamedRooms();
            PlaceRoomsFromGrid();
            PlaceSpecialRooms();
            FinalizeGeneratedMap();

            System.Diagnostics.Debug.WriteLine($"[Map] Generated {_rooms.Count} rooms");
        }

        public static void FinalizeGeneratedMap()
        {
            foreach (var room in _rooms)
                PreventRoomOverlap(room);

            for (int gx = 0; gx <= MapWidth; gx++)
            for (int gy = 0; gy <= MapHeight; gy++)
                _mapTemp[gx, gy] = Min(_mapTemp[gx, gy], 1);

            SpawnGridDoors();
            LinkAdjacentRooms();
        }

        public static void GenerateMap(int seed) => CreateMap();

        private static void GenerateHallwayGrid()
        {
            int x = MapWidth / 2;
            int y = MapHeight - 2;

            for (int i = y; i < MapHeight; i++)
                _mapTemp[x, i] = 1;

            while (y >= 2)
            {
                int width = _createRng.Next(10, 16);
                if (x > MapWidth * 0.6f) width = -width;
                else if (x > MapWidth * 0.4f) x -= width / 2;

                if (x + width > MapWidth - 3) width = MapWidth - 3 - x;
                else if (x + width < 2) width = -x + 2;

                x = Math.Min(x, x + width);
                width = Math.Abs(width);
                for (int i = x; i < x + width; i++)
                    _mapTemp[Math.Min(i, MapWidth), y] = 1;

                int height = _createRng.Next(3, 5);
                if (y - height < 1) height = y - 1;

                int yHallways = _createRng.Next(4, 6);
                if (GetZone(y - height) != GetZone(y - height + 1)) height--;

                int branchX = x;
                for (int h = 1; h <= yHallways; h++)
                {
                    int x2 = Math.Clamp(_createRng.Next(x, x + Math.Max(width - 1, 1)), 2, MapWidth - 2);
                    while (_mapTemp[x2, y - 1] != 0 || _mapTemp[x2 - 1, y - 1] != 0 || _mapTemp[x2 + 1, y - 1] != 0)
                        x2++;

                    if (x2 < x + width)
                    {
                        int tempHeight;
                        if (h == 1)
                        {
                            tempHeight = height;
                            x2 = _createRng.Next(2) == 1 ? x : x + width;
                        }
                        else
                        {
                            tempHeight = _createRng.Next(1, height + 1);
                        }

                        for (int y2 = y - tempHeight; y2 <= y; y2++)
                        {
                            if (GetZone(y2) != GetZone(y2 + 1))
                                _mapTemp[x2, y2] = 255;
                            else
                                _mapTemp[x2, y2] = 1;
                        }

                        if (tempHeight == height) branchX = x2;
                    }
                }

                x = branchX;
                y -= height;
            }
        }

        private static void CountAndAdjustRoomShapes()
        {
            int[] room1Amount = new int[ZoneAmount];
            int[] room2Amount = new int[ZoneAmount];
            int[] room2CAmount = new int[ZoneAmount];
            int[] room3Amount = new int[ZoneAmount];
            int[] room4Amount = new int[ZoneAmount];

            for (int gy = 1; gy < MapHeight; gy++)
            {
                int zone = GetZone(gy);
                for (int gx = 1; gx < MapWidth; gx++)
                {
                    if (_mapTemp[gx, gy] <= 0) continue;

                    int neighbors = Min(_mapTemp[gx + 1, gy], 1) + Min(_mapTemp[gx - 1, gy], 1)
                                  + Min(_mapTemp[gx, gy + 1], 1) + Min(_mapTemp[gx, gy - 1], 1);

                    if (_mapTemp[gx, gy] < 255)
                        _mapTemp[gx, gy] = neighbors;

                    switch (_mapTemp[gx, gy])
                    {
                        case ROOM1:
                            room1Amount[zone]++;
                            break;
                        case ROOM2:
                            if (Min(_mapTemp[gx + 1, gy], 1) + Min(_mapTemp[gx - 1, gy], 1) == 2)
                                room2Amount[zone]++;
                            else if (Min(_mapTemp[gx, gy + 1], 1) + Min(_mapTemp[gx, gy - 1], 1) == 2)
                                room2Amount[zone]++;
                            else
                                room2CAmount[zone]++;
                            break;
                        case ROOM3:
                            room3Amount[zone]++;
                            break;
                        case ROOM4:
                            room4Amount[zone]++;
                            break;
                    }
                }
            }

            ForceMoreRoom1s(room1Amount, room2Amount, room3Amount, room4Amount);
            ForceRoom4AndRoom2C(room1Amount, room2Amount, room2CAmount, room3Amount, room4Amount);

            _room1Amount = room1Amount;
            _room2Amount = room2Amount;
            _room2CAmount = room2CAmount;
            _room3Amount = room3Amount;
            _room4Amount = room4Amount;
        }

        private static int[] _room1Amount, _room2Amount, _room2CAmount, _room3Amount, _room4Amount;
        private static string[,] _mapRoom;

        private static void ForceMoreRoom1s(int[] room1, int[] room2, int[] room3, int[] room4)
        {
            for (int zi = 0; zi < ZoneAmount; zi++)
            {
                int need = -room1[zi] + 5;
                if (need <= 0) continue;

                int yStart = (MapHeight / ZoneAmount) * (2 - zi) + 1;
                int yEnd = (int)((MapHeight / (float)ZoneAmount) * ((2 - zi) + 1.0f)) - 2;

                for (int gy = yStart; gy <= yEnd && need > 0; gy++)
                {
                    for (int gx = 2; gx < MapWidth - 2 && need > 0; gx++)
                    {
                        if (_mapTemp[gx, gy] != 0) continue;

                        int n = Min(_mapTemp[gx + 1, gy], 1) + Min(_mapTemp[gx - 1, gy], 1)
                              + Min(_mapTemp[gx, gy + 1], 1) + Min(_mapTemp[gx, gy - 1], 1);
                        if (n != 1) continue;

                        int nx = 0, ny = 0;
                        if (_mapTemp[gx + 1, gy] != 0) { nx = gx + 1; ny = gy; }
                        else if (_mapTemp[gx - 1, gy] != 0) { nx = gx - 1; ny = gy; }
                        else if (_mapTemp[gx, gy + 1] != 0) { nx = gx; ny = gy + 1; }
                        else if (_mapTemp[gx, gy - 1] != 0) { nx = gx; ny = gy - 1; }

                        bool placed = false;
                        int cell = _mapTemp[nx, ny];
                        if (cell > 1 && cell < 4)
                        {
                            switch (cell)
                            {
                                case ROOM2:
                                    if (Min(_mapTemp[nx + 1, ny], 1) + Min(_mapTemp[nx - 1, ny], 1) == 2)
                                    { room2[zi]--; room3[zi]++; placed = true; }
                                    else if (Min(_mapTemp[nx, ny + 1], 1) + Min(_mapTemp[nx, ny - 1], 1) == 2)
                                    { room2[zi]--; room3[zi]++; placed = true; }
                                    break;
                                case ROOM3:
                                    room3[zi]--; room4[zi]++; placed = true;
                                    break;
                            }

                            if (placed)
                            {
                                _mapTemp[nx, ny] = cell + 1;
                                _mapTemp[gx, gy] = ROOM1;
                                room1[zi]++;
                                need--;
                            }
                        }
                    }
                }
            }
        }

        private static void ForceRoom4AndRoom2C(int[] room1, int[] room2, int[] room2C,
            int[] room3, int[] room4)
        {
            for (int zi = 0; zi < ZoneAmount; zi++)
            {
                int zoneY, zoneYEnd;
                switch (zi)
                {
                    case 2:
                        zoneY = MapHeight / 3;
                        zoneYEnd = MapHeight / 3;
                        break;
                    case 1:
                        zoneY = MapHeight / 3 + 1;
                        zoneYEnd = (int)(MapHeight * (2.0 / 3.0)) - 1;
                        break;
                    default:
                        zoneY = (int)(MapHeight * (2.0 / 3.0)) + 1;
                        zoneYEnd = MapHeight - 2;
                        break;
                }

                if (room4[zi] < 1)
                {
                    bool forced = false;
                    for (int gy = zoneY; gy <= zoneYEnd && !forced; gy++)
                    {
                        for (int gx = 2; gx < MapWidth - 2 && !forced; gx++)
                        {
                            if (_mapTemp[gx, gy] != ROOM3) continue;

                            if (_mapTemp[gx + 1, gy] != 0 || _mapTemp[gx + 1, gy + 1] != 0
                                || _mapTemp[gx + 1, gy - 1] != 0 || _mapTemp[gx + 2, gy] != 0)
                            { _mapTemp[gx + 1, gy] = ROOM1; forced = true; }
                            else if (_mapTemp[gx - 1, gy] != 0 || _mapTemp[gx - 1, gy + 1] != 0
                                || _mapTemp[gx - 1, gy - 1] != 0 || _mapTemp[gx - 2, gy] != 0)
                            { _mapTemp[gx - 1, gy] = ROOM1; forced = true; }
                            else if (_mapTemp[gx, gy + 1] != 0 || _mapTemp[gx + 1, gy + 1] != 0
                                || _mapTemp[gx - 1, gy + 1] != 0 || _mapTemp[gx, gy + 2] != 0)
                            { _mapTemp[gx, gy + 1] = ROOM1; forced = true; }
                            else if (_mapTemp[gx, gy - 1] != 0 || _mapTemp[gx + 1, gy - 1] != 0
                                || _mapTemp[gx - 1, gy - 1] != 0 || _mapTemp[gx, gy - 2] != 0)
                            { _mapTemp[gx, gy - 1] = ROOM1; forced = true; }

                            if (forced)
                            {
                                _mapTemp[gx, gy] = ROOM4;
                                room4[zi]++;
                                room3[zi]--;
                                room1[zi]++;
                            }
                        }
                    }
                }

                if (room2C[zi] < 1)
                {
                    zoneY++;
                    zoneYEnd--;

                    bool forced = false;
                    for (int gy = zoneY; gy <= zoneYEnd && !forced; gy++)
                    {
                        for (int gx = 3; gx < MapWidth - 3 && !forced; gx++)
                        {
                            if (_mapTemp[gx, gy] != ROOM1) continue;

                            if (_mapTemp[gx - 1, gy] > 0
                                && _mapTemp[gx, gy - 1] + _mapTemp[gx, gy + 1] + _mapTemp[gx + 2, gy] == 0)
                            {
                                if (_mapTemp[gx + 1, gy - 2] + _mapTemp[gx + 2, gy - 1] + _mapTemp[gx + 1, gy - 1] == 0)
                                {
                                    _mapTemp[gx, gy] = ROOM2;
                                    _mapTemp[gx + 1, gy] = ROOM2;
                                    _mapTemp[gx + 1, gy - 1] = ROOM1;
                                    forced = true;
                                }
                                else if (_mapTemp[gx + 1, gy + 2] + _mapTemp[gx + 2, gy + 1] + _mapTemp[gx + 1, gy + 1] == 0)
                                {
                                    _mapTemp[gx, gy] = ROOM2;
                                    _mapTemp[gx + 1, gy] = ROOM2;
                                    _mapTemp[gx + 1, gy + 1] = ROOM1;
                                    forced = true;
                                }
                            }
                            else if (_mapTemp[gx + 1, gy] > 0
                                && _mapTemp[gx, gy - 1] + _mapTemp[gx, gy + 1] + _mapTemp[gx - 2, gy] == 0)
                            {
                                if (_mapTemp[gx - 1, gy - 2] + _mapTemp[gx - 2, gy - 1] + _mapTemp[gx - 1, gy - 1] == 0)
                                {
                                    _mapTemp[gx, gy] = ROOM2;
                                    _mapTemp[gx - 1, gy] = ROOM2;
                                    _mapTemp[gx - 1, gy - 1] = ROOM1;
                                    forced = true;
                                }
                                else if (_mapTemp[gx - 1, gy + 2] + _mapTemp[gx - 2, gy + 1] + _mapTemp[gx - 1, gy + 1] == 0)
                                {
                                    _mapTemp[gx, gy] = ROOM2;
                                    _mapTemp[gx - 1, gy] = ROOM2;
                                    _mapTemp[gx - 1, gy + 1] = ROOM1;
                                    forced = true;
                                }
                            }
                            else if (_mapTemp[gx, gy - 1] > 0
                                && _mapTemp[gx - 1, gy] + _mapTemp[gx + 1, gy] + _mapTemp[gx, gy + 2] == 0)
                            {
                                if (_mapTemp[gx - 2, gy + 1] + _mapTemp[gx - 1, gy + 2] + _mapTemp[gx - 1, gy + 1] == 0)
                                {
                                    _mapTemp[gx, gy] = ROOM2;
                                    _mapTemp[gx, gy + 1] = ROOM2;
                                    _mapTemp[gx - 1, gy + 1] = ROOM1;
                                    forced = true;
                                }
                                else if (_mapTemp[gx + 2, gy + 1] + _mapTemp[gx + 1, gy + 2] + _mapTemp[gx + 1, gy + 1] == 0)
                                {
                                    _mapTemp[gx, gy] = ROOM2;
                                    _mapTemp[gx, gy + 1] = ROOM2;
                                    _mapTemp[gx + 1, gy + 1] = ROOM1;
                                    forced = true;
                                }
                            }
                            else if (_mapTemp[gx, gy + 1] > 0
                                && _mapTemp[gx - 1, gy] + _mapTemp[gx + 1, gy] + _mapTemp[gx, gy - 2] == 0)
                            {
                                if (_mapTemp[gx - 2, gy - 1] + _mapTemp[gx - 1, gy - 2] + _mapTemp[gx - 1, gy - 1] == 0)
                                {
                                    _mapTemp[gx, gy] = ROOM2;
                                    _mapTemp[gx, gy - 1] = ROOM2;
                                    _mapTemp[gx - 1, gy - 1] = ROOM1;
                                    forced = true;
                                }
                                else if (_mapTemp[gx + 2, gy - 1] + _mapTemp[gx + 1, gy - 2] + _mapTemp[gx + 1, gy - 1] == 0)
                                {
                                    _mapTemp[gx, gy] = ROOM2;
                                    _mapTemp[gx, gy - 1] = ROOM2;
                                    _mapTemp[gx + 1, gy - 1] = ROOM1;
                                    forced = true;
                                }
                            }

                            if (forced)
                            {
                                room2C[zi]++;
                                room2[zi]++;
                            }
                        }
                    }
                }
            }
        }

        private static void AssignNamedRooms()
        {
            int maxRooms = Math.Max(55 * MapWidth / 20,
                _room1Amount[0] + _room1Amount[1] + _room1Amount[2] + 1);
            maxRooms = Math.Max(maxRooms, _room2Amount[0] + _room2Amount[1] + _room2Amount[2] + 1);
            maxRooms = Math.Max(maxRooms, _room2CAmount[0] + _room2CAmount[1] + _room2CAmount[2] + 1);
            maxRooms = Math.Max(maxRooms, _room3Amount[0] + _room3Amount[1] + _room3Amount[2] + 1);
            maxRooms = Math.Max(maxRooms, _room4Amount[0] + _room4Amount[1] + _room4Amount[2] + 1);

            _mapRoom = new string[ROOM4 + 1, maxRooms];

            int minPos = 1, maxPos = _room1Amount[0] - 1;
            MapRoomSlot(ROOM1, 0, "start");
            SetRoomSlot("roompj", ROOM1, (int)(0.1f * _room1Amount[0]), minPos, maxPos);
            SetRoomSlot("914", ROOM1, (int)(0.3f * _room1Amount[0]), minPos, maxPos);
            SetRoomSlot("room1archive", ROOM1, (int)(0.5f * _room1Amount[0]), minPos, maxPos);
            SetRoomSlot("room205", ROOM1, (int)(0.6f * _room1Amount[0]), minPos, maxPos);
            MapRoomSlot(ROOM2C, 0, "lockroom");

            minPos = 1;
            maxPos = _room2Amount[0] - 1;
            MapRoomSlot(ROOM2, 0, "room2closets");
            SetRoomSlot("room2testroom2", ROOM2, (int)(0.1f * _room2Amount[0]), minPos, maxPos);
            SetRoomSlot("room2scps", ROOM2, (int)(0.2f * _room2Amount[0]), minPos, maxPos);
            SetRoomSlot("room2storage", ROOM2, (int)(0.3f * _room2Amount[0]), minPos, maxPos);
            SetRoomSlot("room2gw_b", ROOM2, (int)(0.4f * _room2Amount[0]), minPos, maxPos);
            SetRoomSlot("room2sl", ROOM2, (int)(0.5f * _room2Amount[0]), minPos, maxPos);
            SetRoomSlot("room012", ROOM2, (int)(0.55f * _room2Amount[0]), minPos, maxPos);
            SetRoomSlot("room2scps2", ROOM2, (int)(0.6f * _room2Amount[0]), minPos, maxPos);
            SetRoomSlot("room1123", ROOM2, (int)(0.7f * _room2Amount[0]), minPos, maxPos);
            SetRoomSlot("room2elevator", ROOM2, (int)(0.85f * _room2Amount[0]), minPos, maxPos);

            MapRoomSlot(ROOM3, (int)Math.Floor((_createRng.NextDouble() * 0.6 + 0.2) * _room3Amount[0]), "room3storage");
            MapRoomSlot(ROOM2C, (int)(0.5f * _room2CAmount[0]), "room1162");
            MapRoomSlot(ROOM4, (int)(0.3f * _room4Amount[0]), "room4info");

            minPos = _room1Amount[0];
            maxPos = _room1Amount[0] + _room1Amount[1] - 1;
            SetRoomSlot("room079", ROOM1, _room1Amount[0] + (int)(0.15f * _room1Amount[1]), minPos, maxPos);
            SetRoomSlot("room106", ROOM1, _room1Amount[0] + (int)(0.3f * _room1Amount[1]), minPos, maxPos);
            SetRoomSlot("008", ROOM1, _room1Amount[0] + (int)(0.4f * _room1Amount[1]), minPos, maxPos);
            SetRoomSlot("room035", ROOM1, _room1Amount[0] + (int)(0.5f * _room1Amount[1]), minPos, maxPos);
            SetRoomSlot("coffin", ROOM1, _room1Amount[0] + (int)(0.7f * _room1Amount[1]), minPos, maxPos);

            minPos = _room2Amount[0];
            maxPos = _room2Amount[0] + _room2Amount[1] - 1;
            MapRoomSlot(ROOM2, _room2Amount[0] + (int)(0.1f * _room2Amount[1]), "room2nuke");
            SetRoomSlot("room2tunnel", ROOM2, _room2Amount[0] + (int)(0.25f * _room2Amount[1]), minPos, maxPos);
            SetRoomSlot("room049", ROOM2, _room2Amount[0] + (int)(0.4f * _room2Amount[1]), minPos, maxPos);
            SetRoomSlot("room2shaft", ROOM2, _room2Amount[0] + (int)(0.6f * _room2Amount[1]), minPos, maxPos);
            SetRoomSlot("testroom", ROOM2, _room2Amount[0] + (int)(0.7f * _room2Amount[1]), minPos, maxPos);
            SetRoomSlot("room2servers", ROOM2, _room2Amount[0] + (int)(0.9f * _room2Amount[1]), minPos, maxPos);

            MapRoomSlot(ROOM3, _room3Amount[0] + (int)(0.3f * _room3Amount[1]), "room513");
            MapRoomSlot(ROOM3, _room3Amount[0] + (int)(0.6f * _room3Amount[1]), "room966");
            MapRoomSlot(ROOM2C, _room2CAmount[0] + (int)(0.5f * _room2CAmount[1]), "room2cpit");

            MapRoomSlot(ROOM1, _room1Amount[0] + _room1Amount[1] + _room1Amount[2] - 2, "exit1");
            MapRoomSlot(ROOM1, _room1Amount[0] + _room1Amount[1] + _room1Amount[2] - 1, "gateaentrance");
            MapRoomSlot(ROOM1, _room1Amount[0] + _room1Amount[1], "room1lifts");

            minPos = _room2Amount[0] + _room2Amount[1];
            maxPos = _room2Amount[0] + _room2Amount[1] + _room2Amount[2] - 1;
            MapRoomSlot(ROOM2, minPos + (int)(0.1f * _room2Amount[2]), "room2poffices");
            SetRoomSlot("room2cafeteria", ROOM2, minPos + (int)(0.2f * _room2Amount[2]), minPos, maxPos);
            SetRoomSlot("room2sroom", ROOM2, minPos + (int)(0.3f * _room2Amount[2]), minPos, maxPos);
            SetRoomSlot("room2servers2", ROOM2, minPos + (int)(0.4f * _room2Amount[2]), minPos, maxPos);
            SetRoomSlot("room2offices", ROOM2, minPos + (int)(0.45f * _room2Amount[2]), minPos, maxPos);
            SetRoomSlot("room2offices4", ROOM2, minPos + (int)(0.5f * _room2Amount[2]), minPos, maxPos);
            SetRoomSlot("room860", ROOM2, minPos + (int)(0.6f * _room2Amount[2]), minPos, maxPos);
            SetRoomSlot("medibay", ROOM2, minPos + (int)(0.7f * _room2Amount[2]), minPos, maxPos);
            SetRoomSlot("room2poffices2", ROOM2, minPos + (int)(0.8f * _room2Amount[2]), minPos, maxPos);
            SetRoomSlot("room2offices2", ROOM2, minPos + (int)(0.9f * _room2Amount[2]), minPos, maxPos);

            MapRoomSlot(ROOM2C, _room2CAmount[0] + _room2CAmount[1], "room2ccont");
            MapRoomSlot(ROOM2C, _room2CAmount[0] + _room2CAmount[1] + 1, "lockroom2");
            MapRoomSlot(ROOM3, _room3Amount[0] + _room3Amount[1] + (int)(0.3f * _room3Amount[2]), "room3servers");
            MapRoomSlot(ROOM3, _room3Amount[0] + _room3Amount[1] + (int)(0.7f * _room3Amount[2]), "room3servers2");
            MapRoomSlot(ROOM3, _room3Amount[0] + _room3Amount[1] + (int)(0.5f * _room3Amount[2]), "room3offices");
        }

        private static int[] _mapRoomId = new int[ROOM4 + 1];

        private static void PlaceRoomsFromGrid()
        {
            Array.Clear(_mapRoomId, 0, _mapRoomId.Length);

            for (int gy = MapHeight - 1; gy >= 1; gy--)
            {
                int zone;
                if (gy < MapHeight / 3 + 1) zone = 3;
                else if (gy < MapHeight * (2.0 / 3.0)) zone = 2;
                else zone = 1;

                for (int gx = 1; gx < MapWidth - 1; gx++)
                {
                    if (_mapTemp[gx, gy] == 255)
                    {
                        string checkpoint = gy > MapHeight / 2 ? "checkpoint1" : "checkpoint2";
                        CreateRoom(zone, ROOM2, gx * RoomSpacing, 0, gy * RoomSpacing, checkpoint);
                        continue;
                    }

                    if (_mapTemp[gx, gy] <= 0) continue;

                    int neighbors = Min(_mapTemp[gx + 1, gy], 1) + Min(_mapTemp[gx - 1, gy], 1)
                                  + Min(_mapTemp[gx, gy + 1], 1) + Min(_mapTemp[gx, gy - 1], 1);

                    RoomInstance room = null;
                    string roomName = _mapName[gx, gy];

                    switch (neighbors)
                    {
                        case 1:
                            if (_mapRoomId[ROOM1] < _mapRoom.GetLength(1) && roomName == "")
                            {
                                string slot = GetMapRoomSlot(ROOM1, _mapRoomId[ROOM1]);
                                if (slot != "") roomName = slot;
                            }
                            room = CreateRoom(zone, ROOM1, gx * RoomSpacing, 0, gy * RoomSpacing, roomName);
                            if (room != null)
                            {
                                if (_mapTemp[gx, gy + 1] != 0) room.Angle = 180;
                                else if (_mapTemp[gx - 1, gy] != 0) room.Angle = 270;
                                else if (_mapTemp[gx + 1, gy] != 0) room.Angle = 90;
                                else room.Angle = 0;
                                ApplyRoomAngle(room);
                                _mapName[gx, gy] = room.RoomName;
                                _mapRoomId[ROOM1]++;
                            }
                            break;

                        case 2:
                            if (_mapTemp[gx - 1, gy] > 0 && _mapTemp[gx + 1, gy] > 0)
                            {
                                if (_mapRoomId[ROOM2] < _mapRoom.GetLength(1) && roomName == "")
                                {
                                    string slot = GetMapRoomSlot(ROOM2, _mapRoomId[ROOM2]);
                                    if (slot != "") roomName = slot;
                                }
                                room = CreateRoom(zone, ROOM2, gx * RoomSpacing, 0, gy * RoomSpacing, roomName);
                                if (room != null)
                                {
                                    room.Angle = _createRng.Next(2) == 1 ? 90f : 270f;
                                    ApplyRoomAngle(room);
                                    _mapName[gx, gy] = room.RoomName;
                                    _mapRoomId[ROOM2]++;
                                }
                            }
                            else if (_mapTemp[gx, gy - 1] > 0 && _mapTemp[gx, gy + 1] > 0)
                            {
                                if (_mapRoomId[ROOM2] < _mapRoom.GetLength(1) && roomName == "")
                                {
                                    string slot = GetMapRoomSlot(ROOM2, _mapRoomId[ROOM2]);
                                    if (slot != "") roomName = slot;
                                }
                                room = CreateRoom(zone, ROOM2, gx * RoomSpacing, 0, gy * RoomSpacing, roomName);
                                if (room != null)
                                {
                                    room.Angle = _createRng.Next(2) == 1 ? 180f : 0f;
                                    ApplyRoomAngle(room);
                                    _mapName[gx, gy] = room.RoomName;
                                    _mapRoomId[ROOM2]++;
                                }
                            }
                            else
                            {
                                if (_mapRoomId[ROOM2C] < _mapRoom.GetLength(1) && roomName == "")
                                {
                                    string slot = GetMapRoomSlot(ROOM2C, _mapRoomId[ROOM2C]);
                                    if (slot != "") roomName = slot;
                                }

                                if (_mapTemp[gx - 1, gy] > 0 && _mapTemp[gx, gy + 1] > 0)
                                {
                                    room = CreateRoom(zone, ROOM2C, gx * RoomSpacing, 0, gy * RoomSpacing, roomName);
                                    if (room != null) { room.Angle = 180; ApplyRoomAngle(room); }
                                }
                                else if (_mapTemp[gx + 1, gy] > 0 && _mapTemp[gx, gy + 1] > 0)
                                {
                                    room = CreateRoom(zone, ROOM2C, gx * RoomSpacing, 0, gy * RoomSpacing, roomName);
                                    if (room != null) { room.Angle = 90; ApplyRoomAngle(room); }
                                }
                                else if (_mapTemp[gx - 1, gy] > 0 && _mapTemp[gx, gy - 1] > 0)
                                {
                                    room = CreateRoom(zone, ROOM2C, gx * RoomSpacing, 0, gy * RoomSpacing, roomName);
                                    if (room != null) { room.Angle = 270; ApplyRoomAngle(room); }
                                }
                                else
                                {
                                    room = CreateRoom(zone, ROOM2C, gx * RoomSpacing, 0, gy * RoomSpacing, roomName);
                                }

                                if (room != null)
                                {
                                    _mapName[gx, gy] = room.RoomName;
                                    _mapRoomId[ROOM2C]++;
                                }
                            }
                            break;

                        case 3:
                            if (_mapRoomId[ROOM3] < _mapRoom.GetLength(1) && roomName == "")
                            {
                                string slot = GetMapRoomSlot(ROOM3, _mapRoomId[ROOM3]);
                                if (slot != "") roomName = slot;
                            }
                            room = CreateRoom(zone, ROOM3, gx * RoomSpacing, 0, gy * RoomSpacing, roomName);
                            if (room != null)
                            {
                                if (_mapTemp[gx, gy - 1] == 0) room.Angle = 180;
                                else if (_mapTemp[gx - 1, gy] == 0) room.Angle = 90;
                                else if (_mapTemp[gx + 1, gy] == 0) room.Angle = 270;
                                ApplyRoomAngle(room);
                                _mapName[gx, gy] = room.RoomName;
                                _mapRoomId[ROOM3]++;
                            }
                            break;

                        case 4:
                            if (_mapRoomId[ROOM4] < _mapRoom.GetLength(1) && roomName == "")
                            {
                                string slot = GetMapRoomSlot(ROOM4, _mapRoomId[ROOM4]);
                                if (slot != "") roomName = slot;
                            }
                            room = CreateRoom(zone, ROOM4, gx * RoomSpacing, 0, gy * RoomSpacing, roomName);
                            if (room != null)
                            {
                                _mapName[gx, gy] = room.RoomName;
                                _mapRoomId[ROOM4]++;
                            }
                            break;
                    }
                }
            }
        }

        private static void PlaceSpecialRooms()
        {
            CreateRoom(0, ROOM1, (MapWidth - 1) * RoomSpacing, 500, RoomSpacing, "gatea");
            CreateRoom(0, ROOM1, (MapWidth - 1) * RoomSpacing, 0, (MapHeight - 1) * RoomSpacing, "pocketdimension");

            if (MenuSystem.IntroEnabled)
                CreateRoom(0, ROOM1, RoomSpacing, 0, (MapHeight - 1) * RoomSpacing, "173");

            CreateRoom(0, ROOM1, RoomSpacing, 800, 0, "dimension1499");
        }

        private static void ApplyRoomAngle(RoomInstance room)
        {
            room.ry = room.Angle;
            TurnEntity(room.mesh, 0, room.Angle, 0);
        }

        private static void MapRoomSlot(int shape, int index, string name)
        {
            if (_mapRoom == null || shape < 0 || shape >= _mapRoom.GetLength(0)) return;
            if (index < 0 || index >= _mapRoom.GetLength(1)) return;
            _mapRoom[shape, index] = name;
        }

        private static string GetMapRoomSlot(int shape, int index)
        {
            if (_mapRoom == null || shape < 0 || shape >= _mapRoom.GetLength(0)) return "";
            if (index < 0 || index >= _mapRoom.GetLength(1)) return "";
            return _mapRoom[shape, index] ?? "";
        }

        private static bool SetRoomSlot(string roomName, int roomType, int pos, int minPos, int maxPos)
        {
            if (maxPos < minPos)
            {
                System.Diagnostics.Debug.WriteLine($"[Map] Can't place {roomName}");
                return false;
            }

            bool looped = false;
            while (GetMapRoomSlot(roomType, pos) != "")
            {
                pos++;
                if (pos > maxPos)
                {
                    if (!looped) { pos = minPos + 1; looped = true; }
                    else return false;
                }
            }

            MapRoomSlot(roomType, pos, roomName);
            return true;
        }

        public static int GetZone(int y)
        {
            return Math.Min((int)Math.Floor((float)(MapHeight - y) / MapHeight * ZoneAmount), ZoneAmount - 1);
        }

        // ─── Waypoints / room tracking ─────────────────────────────────────────────

        public static void LoadMap(string path)
        {
            FreeAllRooms();
            DoorSystem.FreeAll();
            EventSystem.FreeAll();
            RoomTemplateSystem.LoadRoomTemplates();

            if (!File.Exists(path))
            {
                System.Diagnostics.Debug.WriteLine($"[Map] Missing map file: {path}");
                LoadRoom("start", 0, 0, 0);
                return;
            }

            if (path.EndsWith("cbmap2", StringComparison.OrdinalIgnoreCase))
                LoadCbMap2(path);
            else
            {
                System.Diagnostics.Debug.WriteLine($"[Map] Unsupported map format: {path}");
                LoadRoom("start", 0, 0, 0);
            }

            FinalizeGeneratedMap();
            System.Diagnostics.Debug.WriteLine($"[Map] Loaded custom map: {path} ({_rooms.Count} rooms)");
        }

        private static void LoadCbMap2(string path)
        {
            using var stream = File.OpenRead(path);
            using var reader = new BinaryReader(stream);

            ReadLineBytes(reader);
            ReadLineBytes(reader);

            ZoneTransition0 = reader.ReadByte();
            ZoneTransition1 = reader.ReadByte();
            int roomAmount = reader.ReadInt32();
            int forestPieceAmount = reader.ReadInt32();
            int mtPieceAmount = reader.ReadInt32();

            Array.Clear(_mapTemp, 0, _mapTemp.Length);

            for (int i = 0; i < roomAmount; i++)
            {
                int gx = reader.ReadByte();
                int gy = reader.ReadByte();
                string name = ReadMapString(reader).ToLowerInvariant();
                float angle = reader.ReadByte() * 90f;

                var template = RoomTemplateSystem.GetByName(name);
                if (template != null)
                {
                    float wx = (MapWidth - gx) * RoomSpacing;
                    float wz = gy * RoomSpacing;
                    var room = CreateRoom(0, template.Shape, wx, 0, wz, name);
                    if (room != null)
                    {
                        if (angle != 90f && angle != 270f)
                            angle += 180f;
                        room.Angle = MathUtil.WrapAngle(angle);
                        ApplyRoomAngle(room);
                        int mx = MapWidth - gx;
                        if (mx >= 0 && mx <= MapWidth && gy >= 0 && gy <= MapHeight)
                            _mapTemp[mx, gy] = 1;
                    }
                }

                ReadMapString(reader);
                reader.ReadSingle(); // event probability
            }

            // Forest / MT pieces skipped — room860 forest rebuilt by FillRoom when present
            for (int i = 0; i < forestPieceAmount; i++)
            {
                reader.ReadByte();
                reader.ReadByte();
                ReadMapString(reader);
                reader.ReadByte();
            }

            for (int i = 0; i < mtPieceAmount; i++)
            {
                reader.ReadByte();
                reader.ReadByte();
                ReadMapString(reader);
                reader.ReadByte();
            }
        }

        private static byte[] ReadLineBytes(BinaryReader reader)
        {
            var bytes = new List<byte>();
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                byte b = reader.ReadByte();
                if (b == '\n') break;
                if (b != '\r') bytes.Add(b);
            }
            return bytes.ToArray();
        }

        private static string ReadMapString(BinaryReader reader)
        {
            int len = reader.ReadInt32();
            if (len <= 0) return "";
            return System.Text.Encoding.ASCII.GetString(reader.ReadBytes(len));
        }

        public static void SpawnGridDoors()
        {
            for (int gy = MapHeight; gy >= 0; gy--)
            {
                int zone;
                if (gy < ZoneTransition1 - 1) zone = 3;
                else if (gy >= ZoneTransition1 - 1 && gy < ZoneTransition0 - 1) zone = 2;
                else zone = 1;

                for (int gx = MapWidth; gx >= 0; gx--)
                {
                    if (_mapTemp[gx, gy] <= 0) continue;

                    int big = zone == 2 ? 2 : 0;
                    foreach (var room in _rooms)
                    {
                        room.Angle = MathUtil.WrapAngle(room.Angle);
                        if ((int)(room.x / RoomSpacing) != gx || (int)(room.z / RoomSpacing) != gy)
                            continue;

                        if (ShouldSpawnEastDoor(room))
                        {
                            if (gx + 1 <= MapWidth && _mapTemp[gx + 1, gy] > 0)
                            {
                                bool open = Math.Max(Rand(-3, 1), 0) != 0;
                                room.AdjDoor[0] = DoorSystem.CreateDoor(room.zone,
                                    gx * RoomSpacing + RoomSpacing / 2f, 0, gy * RoomSpacing,
                                    90, room, open, big);
                            }
                        }

                        if (ShouldSpawnNorthDoor(room))
                        {
                            if (gy + 1 <= MapHeight && _mapTemp[gx, gy + 1] > 0)
                            {
                                bool open = Math.Max(Rand(-3, 1), 0) != 0;
                                room.AdjDoor[3] = DoorSystem.CreateDoor(room.zone,
                                    gx * RoomSpacing, 0, gy * RoomSpacing + RoomSpacing / 2f,
                                    0, room, open, big);
                            }
                        }

                        break;
                    }
                }
            }
        }

        private static bool ShouldSpawnEastDoor(RoomInstance room)
        {
            int shape = room.Template?.Shape ?? 0;
            float a = room.Angle;
            return shape switch
            {
                ROOM1 => a == 90f,
                ROOM2 => a == 90f || a == 270f,
                ROOM2C => a == 0f || a == 90f,
                ROOM3 => a == 0f || a == 180f || a == 90f,
                _ => true,
            };
        }

        private static bool ShouldSpawnNorthDoor(RoomInstance room)
        {
            int shape = room.Template?.Shape ?? 0;
            float a = room.Angle;
            return shape switch
            {
                ROOM1 => a == 180f,
                ROOM2 => a == 0f || a == 180f,
                ROOM2C => a == 180f || a == 90f,
                ROOM3 => a == 180f || a == 90f || a == 270f,
                _ => true,
            };
        }

        public static void LinkAdjacentRooms()
        {
            foreach (var room in _rooms)
            {
                room.Angle = MathUtil.WrapAngle(room.Angle);
                room.Adjacent[0] = null;
                room.Adjacent[1] = null;
                room.Adjacent[2] = null;
                room.Adjacent[3] = null;

                foreach (var other in _rooms)
                {
                    if (other == room) continue;

                    if (Math.Abs(other.z - room.z) < 0.01f)
                    {
                        if (Math.Abs(other.x - (room.x + RoomSpacing)) < 0.01f)
                        {
                            room.Adjacent[0] = other;
                            if (room.AdjDoor[0] == null) room.AdjDoor[0] = other.AdjDoor[2];
                        }
                        else if (Math.Abs(other.x - (room.x - RoomSpacing)) < 0.01f)
                        {
                            room.Adjacent[2] = other;
                            if (room.AdjDoor[2] == null) room.AdjDoor[2] = other.AdjDoor[0];
                        }
                    }
                    else if (Math.Abs(other.x - room.x) < 0.01f)
                    {
                        if (Math.Abs(other.z - (room.z - RoomSpacing)) < 0.01f)
                        {
                            room.Adjacent[1] = other;
                            if (room.AdjDoor[1] == null) room.AdjDoor[1] = other.AdjDoor[3];
                        }
                        else if (Math.Abs(other.z - (room.z + RoomSpacing)) < 0.01f)
                        {
                            room.Adjacent[3] = other;
                            if (room.AdjDoor[3] == null) room.AdjDoor[3] = other.AdjDoor[1];
                        }
                    }
                }
            }
        }

        public static void CalculateRoomExtents(RoomInstance room)
        {
            if (room?.Template == null || room.Template.DisableOverlapCheck) return;

            float shrink = 0.05f;
            float rs = GameState.RoomScale;
            float half = 4f * rs;

            room.MinX = room.x - half + shrink;
            room.MinY = room.y + shrink;
            room.MinZ = room.z - half + shrink;
            room.MaxX = room.x + half - shrink;
            room.MaxY = room.y + 4f * rs - shrink;
            room.MaxZ = room.z + half - shrink;

            if (room.Template.Shape == ROOM2)
            {
                if (room.Angle == 90f || room.Angle == 270f)
                    room.MaxX = room.x + 12f * rs - shrink;
                else
                    room.MaxZ = room.z + 12f * rs - shrink;
            }
        }

        private static bool CheckRoomOverlap(RoomInstance a, RoomInstance b)
        {
            if (a.MaxX <= b.MinX || a.MaxY <= b.MinY || a.MaxZ <= b.MinZ) return false;
            if (a.MinX >= b.MaxX || a.MinY >= b.MaxY || a.MinZ >= b.MaxZ) return false;
            return true;
        }

        public static bool PreventRoomOverlap(RoomInstance room)
        {
            if (room.Template?.DisableOverlapCheck != false) return true;

            string name = room.RoomName;
            if (name is "checkpoint1" or "checkpoint2" or "start") return true;

            CalculateRoomExtents(room);

            bool intersecting = false;
            foreach (var other in _rooms)
            {
                if (other == room || other.Template?.DisableOverlapCheck != false) continue;
                CalculateRoomExtents(other);
                if (CheckRoomOverlap(room, other))
                {
                    intersecting = true;
                    break;
                }
            }

            if (!intersecting) return true;

            if (room.Template.Shape == ROOM2)
            {
                room.Angle = MathUtil.WrapAngle(room.Angle + 180f);
                ApplyRoomAngle(room);
                CalculateRoomExtents(room);
                intersecting = false;
                foreach (var other in _rooms)
                {
                    if (other == room || other.Template?.DisableOverlapCheck != false) continue;
                    if (CheckRoomOverlap(room, other)) { intersecting = true; break; }
                }
                if (!intersecting) return true;

                room.Angle = MathUtil.WrapAngle(room.Angle - 180f);
                ApplyRoomAngle(room);
                CalculateRoomExtents(room);
            }

            foreach (var candidate in _rooms)
            {
                if (candidate == room || candidate.Template?.DisableOverlapCheck != false) continue;
                if (candidate.Template?.Shape != room.Template?.Shape) continue;
                if (candidate.zone != room.zone) continue;
                if (candidate.RoomName is "checkpoint1" or "checkpoint2" or "start") continue;

                float rx = room.x, rz = room.z, ra = room.Angle;
                float cx = candidate.x, cz = candidate.z, ca = candidate.Angle;

                room.x = cx;
                room.z = cz;
                room.Angle = ca;
                PositionEntity(room.mesh, room.x, room.y, room.z);
                ApplyRoomAngle(room);
                CalculateRoomExtents(room);

                candidate.x = rx;
                candidate.z = rz;
                candidate.Angle = ra;
                PositionEntity(candidate.mesh, candidate.x, candidate.y, candidate.z);
                ApplyRoomAngle(candidate);
                CalculateRoomExtents(candidate);

                intersecting = false;
                foreach (var other in _rooms)
                {
                    if (other.Template?.DisableOverlapCheck != false) continue;
                    if (other != room && CheckRoomOverlap(room, other)) { intersecting = true; break; }
                    if (other != candidate && CheckRoomOverlap(candidate, other)) { intersecting = true; break; }
                }

                if (!intersecting) return true;

                room.x = rx;
                room.z = rz;
                room.Angle = ra;
                PositionEntity(room.mesh, room.x, room.y, room.z);
                ApplyRoomAngle(room);
                CalculateRoomExtents(room);

                candidate.x = cx;
                candidate.z = cz;
                candidate.Angle = ca;
                PositionEntity(candidate.mesh, candidate.x, candidate.y, candidate.z);
                ApplyRoomAngle(candidate);
                CalculateRoomExtents(candidate);
            }

            return false;
        }

        public static void InitWayPoints(int loadingStart = 45)
        {
            _waypoints.Clear();
            foreach (var room in _rooms)
            {
                _waypoints.Add(new Waypoint
                {
                    Room = room,
                    X = room.x,
                    Y = room.y,
                    Z = room.z,
                });
            }
        }

        public static void UpdateRooms()
        {
            if (GameState.Collider == -1) return;

            float px = EntityX(GameState.Collider, true);
            float py = EntityY(GameState.Collider, true);
            float pz = EntityZ(GameState.Collider, true);

            RoomInstance closest = null;
            float closestDist = float.MaxValue;

            foreach (var room in _rooms)
            {
                float dx = px - room.x;
                float dz = pz - room.z;
                float dist = dx * dx + dz * dz;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = room;
                }
            }

            GameState.PlayerRoom = closest;
            UpdatePlayerZone(pz);
        }

        private static void UpdatePlayerZone(float playerZ)
        {
            float z = playerZ / 8f;
            int offset = string.IsNullOrEmpty(GameState.SelectedMap) ? 0 : 1;

            if (z < ZoneTransition1 - offset)
                EventSystem.PlayerZone = 2;
            else if (z >= ZoneTransition1 - offset && z < ZoneTransition0 - offset)
                EventSystem.PlayerZone = 1;
            else
                EventSystem.PlayerZone = 0;
        }

        public class Waypoint
        {
            public RoomInstance Room;
            public float X, Y, Z;
        }

        // ─── Mesh helpers ──────────────────────────────────────────────────────────

        private static void EnsureTemplateMesh(RoomTemplate template)
        {
            if (template.Obj != -1) return;

            string rmeshPath = ResolveRMeshPath(template);
            if (File.Exists(rmeshPath))
            {
                template.Obj = LoadRMesh(rmeshPath);
                if (template.Obj != -1)
                    return;
            }

            if (!string.IsNullOrEmpty(template.MeshAssetName))
            {
                template.Obj = LoadMesh(template.MeshAssetName);
                if (template.Obj != -1)
                    return;
            }

            System.Diagnostics.Debug.WriteLine($"[Map] Failed to load template mesh: {template.Name}");
            template.Obj = CreatePivot();
        }

        private static string ResolveRMeshPath(RoomTemplate template)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrEmpty(template.ObjPath))
            {
                string normalized = template.ObjPath.Replace('\\', Path.DirectorySeparatorChar);
                string full = Path.Combine(baseDir, normalized);
                if (File.Exists(full)) return full;
            }

            string opt = Path.Combine(baseDir, "GFX", "map", $"{template.Name}_opt.rmesh");
            if (File.Exists(opt)) return opt;
            return Path.Combine(baseDir, "GFX", "map", $"{template.Name}.rmesh");
        }

        // ─── Cleanup ───────────────────────────────────────────────────────────────

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

        private static int Min(int a, int b) => a < b ? a : b;
    }
}