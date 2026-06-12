// EventSystem.cs — ports Events type + InitEvents/UpdateEvents from Main.bb + UpdateEvents.bb

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SCPCB360.Engine;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    /// <summary>Ports BlitzBasic Type Events.</summary>
    public class GameEvent
    {
        public string EventName;
        public RoomInstance Room;

        public float EventState;
        public float EventState2;
        public float EventState3;

        public int SoundChn = -1;
        public int SoundChn2 = -1;
        public SoundEffect Sound;
        public SoundEffect Sound2;
        public bool SoundChnIsStream;
        public bool SoundChn2IsStream;

        public string EventStr = "";
        public int Img = -1;
        public Texture2D OverlayImage;
    }

    /// <summary>Per-room runtime data used by event handlers (RoomDoors, NPC, Objects, etc.).</summary>
    public class EventRoomContext
    {
        public Door[] RoomDoors = new Door[10];
        public Door[] AdjDoor = new Door[4];
        public NPC[] Npc = new NPC[20];
        public int[] Objects = new int[32];
        public float Dist;
        public float Angle;
    }

    public static partial class EventSystem
    {
        private static readonly List<GameEvent> _events = new();
        private static readonly Dictionary<RoomInstance, EventRoomContext> _roomCtx = new();
        private static readonly bool[] _commotionState = new bool[27];
        public static IReadOnlyList<GameEvent> All => _events;

        // Globals referenced across events (Main.bb)
        public static bool RemoteDoorOn;
        public static bool Contained106;
        public static int PlayerZone;
        public static bool SoundTransmission;
        public static string SelectedEnding = "";

        public static EventRoomContext GetContext(RoomInstance room)
        {
            if (room == null) return new EventRoomContext();
            if (!_roomCtx.TryGetValue(room, out var ctx))
            {
                ctx = new EventRoomContext();
                if (room.RoomDoors != null)
                    Array.Copy(room.RoomDoors, ctx.RoomDoors, Math.Min(room.RoomDoors.Length, ctx.RoomDoors.Length));
                if (room.AdjDoor != null)
                    Array.Copy(room.AdjDoor, ctx.AdjDoor, Math.Min(room.AdjDoor.Length, ctx.AdjDoor.Length));
                if (room.Objects != null)
                    Array.Copy(room.Objects, ctx.Objects, Math.Min(room.Objects.Length, ctx.Objects.Length));
                _roomCtx[room] = ctx;
            }
            return ctx;
        }

        private static float AggressiveNpcFactor =>
            GameState.SelectedDifficulty?.AggressiveNpcs == true ? 1f : 0f;

        // ── CreateEvent — ports CreateEvent.Events from Main.bb ─────────────────

        public static GameEvent CreateEvent(string eventName, string roomName, int id, float prob = 0f)
        {
            if (prob == 0f)
            {
                int i = 0;
                foreach (var room in MapSystem.All)
                {
                    if (roomName != "" && !room.RoomName.Equals(roomName, StringComparison.OrdinalIgnoreCase)) continue;

                    bool occupied = RoomHasEvent(room);
                    i++;
                    if (i >= id && !occupied)
                    {
                        var e = NewEvent(eventName, room);
                        _events.Add(e);
                        return e;
                    }
                }
                return null;
            }

            foreach (var room in MapSystem.All)
            {
                if (roomName != "" && !room.RoomName.Equals(roomName, StringComparison.OrdinalIgnoreCase)) continue;
                if (RoomHasEvent(room)) continue;
                if (Rnd(0f, 1f) < prob)
                {
                    var e = NewEvent(eventName, room);
                    _events.Add(e);
                }
            }
            return null;
        }

        private static GameEvent NewEvent(string eventName, RoomInstance room) =>
            new GameEvent { EventName = eventName, Room = room };

        private static bool RoomHasEvent(RoomInstance room)
        {
            foreach (var e in _events)
                if (e.Room == room) return true;
            return false;
        }

        // ── InitEvents — ports Main.bb InitEvents() (lines 2499-2686) ───────────

        public static void InitEvents()
        {
            _events.Clear();
            _roomCtx.Clear();
            Array.Clear(_commotionState, 0, _commotionState.Length);

            CreateEvent("173", "173", 0);
            CreateEvent("alarm", "start", 0);
            CreateEvent("pocketdimension", "pocketdimension", 0);

            CreateEvent("tunnel106", "tunnel", 0, 0.07f + 0.1f * AggressiveNpcFactor);

            if (Rand(0, 3) < 3) CreateEvent("lockroom173", "lockroom", 0);
            CreateEvent("lockroom173", "lockroom", 0, 0.3f + 0.5f * AggressiveNpcFactor);

            CreateEvent("room2trick", "room2", 0, 0.15f);
            CreateEvent("1048a", "room2", 0, 1f);
            CreateEvent("room2storage", "room2storage", 0);
            CreateEvent("lockroom096", "lockroom2", 0);
            CreateEvent("endroom106", "endroom", Rand(0, 1));
            CreateEvent("room2poffices2", "room2poffices2", 0);
            CreateEvent("room2fan", "room2_2", 0, 1f);
            CreateEvent("room2elevator2", "room2elevator", 0);
            CreateEvent("room2elevator", "room2elevator", Rand(1, 2));
            CreateEvent("room3storage", "room3storage", 0, 0f);
            CreateEvent("tunnel2smoke", "tunnel2", 0, 0.2f);
            CreateEvent("tunnel2", "tunnel2", Rand(0, 2), 0f);
            CreateEvent("tunnel2", "tunnel2", 0, 0.2f * AggressiveNpcFactor);
            CreateEvent("room2doors173", "room2doors", 0, 0.5f + 0.4f * AggressiveNpcFactor);
            CreateEvent("room2offices2", "room2offices2", 0, 0.7f);
            CreateEvent("room2closets", "room2closets", 0);
            CreateEvent("room2cafeteria", "room2cafeteria", 0);
            CreateEvent("room3pitduck", "room3pit", 0);
            CreateEvent("room3pit1048", "room3pit", 1);
            CreateEvent("room2offices3", "room2offices3", 0, 1f);
            CreateEvent("room2servers", "room2servers", 0);
            CreateEvent("room3servers", "room3servers", 0);
            CreateEvent("room3servers", "room3servers2", 0);
            CreateEvent("room3tunnel", "room3tunnel", 0, 0.08f);
            CreateEvent("room4", "room4", 0);

            if (Rand(0, 5) < 5)
            {
                switch (Rand(0, 3))
                {
                    case 1: CreateEvent("682roar", "tunnel", Rand(0, 2), 0f); break;
                    case 2: CreateEvent("682roar", "room3pit", Rand(0, 2), 0f); break;
                    case 3: CreateEvent("682roar", "room2z3", 0, 0f); break;
                }
            }

            CreateEvent("testroom173", "room2testroom2", 0, 1f);
            CreateEvent("room2tesla", "room2tesla", 0, 0.9f);
            CreateEvent("room2nuke", "room2nuke", 0, 0f);

            if (Rand(0, 5) < 5)
                CreateEvent("coffin106", "coffin", 0, 0f);
            else
                CreateEvent("coffin", "coffin", 0, 0f);

            CreateEvent("checkpoint", "checkpoint1", 0, 1f);
            CreateEvent("checkpoint", "checkpoint2", 0, 1f);
            CreateEvent("room3door", "room3", 0, 0.1f);
            CreateEvent("room3door", "room3tunnel", 0, 0.1f);

            if (Rand(0, 2) == 1)
            {
                CreateEvent("106victim", "room3", Rand(1, 2));
                CreateEvent("106sinkhole", "room3_2", Rand(2, 3));
            }
            else
            {
                CreateEvent("106victim", "room3_2", Rand(1, 2));
                CreateEvent("106sinkhole", "room3", Rand(2, 3));
            }
            CreateEvent("106sinkhole", "room4", Rand(1, 2));

            CreateEvent("room079", "room079", 0, 0f);
            CreateEvent("room049", "room049", 0, 0f);
            CreateEvent("room012", "room012", 0, 0f);
            CreateEvent("room035", "room035", 0, 0f);
            CreateEvent("008", "008", 0, 0f);
            CreateEvent("room106", "room106", 0, 0f);
            CreateEvent("pj", "roompj", 0, 0f);
            CreateEvent("914", "914", 0, 0f);
            CreateEvent("buttghost", "room2toilets", 0, 0f);
            CreateEvent("toiletguard", "room2toilets", 1, 0f);
            CreateEvent("room2pipes106", "room2pipes", Rand(0, 3));
            CreateEvent("room2pit", "room2pit", 0, 0.4f + 0.4f * AggressiveNpcFactor);
            CreateEvent("testroom", "testroom", 0);
            CreateEvent("room2tunnel", "room2tunnel", 0);
            CreateEvent("room2ccont", "room2ccont", 0);
            CreateEvent("gateaentrance", "gateaentrance", 0);
            CreateEvent("gatea", "gatea", 0);
            CreateEvent("exit1", "exit1", 0);
            CreateEvent("room205", "room205", 0);
            CreateEvent("room860", "room860", 0);
            CreateEvent("room966", "room966", 0);
            CreateEvent("room1123", "room1123", 0, 0f);
            CreateEvent("room2tesla", "room2tesla_lcz", 0, 0.9f);
            CreateEvent("room2tesla", "room2tesla_hcz", 0, 0.9f);

            CreateEvent("room4tunnels", "room4tunnels", 0);
            CreateEvent("room_gw", "room2gw", 0, 1f);
            CreateEvent("dimension1499", "dimension1499", 0);
            CreateEvent("room1162", "room1162", 0);
            CreateEvent("room2scps2", "room2scps2", 0);
            CreateEvent("room_gw", "room3gw", 0, 1f);
            CreateEvent("room2sl", "room2sl", 0);
            CreateEvent("medibay", "medibay", 0);
            CreateEvent("room2shaft", "room2shaft", 0);
            CreateEvent("room1lifts", "room1lifts", 0);
            CreateEvent("room2gw_b", "room2gw_b", Rand(0, 1));

            float spawn096 = 0.2f * AggressiveNpcFactor;
            CreateEvent("096spawn", "room4pit", 0, 0.6f + spawn096);
            CreateEvent("096spawn", "room3pit", 0, 0.6f + spawn096);
            CreateEvent("096spawn", "room2pipes", 0, 0.4f + spawn096);
            CreateEvent("096spawn", "room2pit", 0, 0.5f + spawn096);
            CreateEvent("096spawn", "room3tunnel", 0, 0.6f + spawn096);
            CreateEvent("096spawn", "room4tunnels", 0, 0.7f + spawn096);
            CreateEvent("096spawn", "tunnel", 0, 0.6f + spawn096);
            CreateEvent("096spawn", "tunnel2", 0, 0.4f + spawn096);
            CreateEvent("096spawn", "room3z2", 0, 0.7f + spawn096);

            CreateEvent("room2pit", "room2_4", 0, 0.4f + 0.4f * AggressiveNpcFactor);
            CreateEvent("room2offices035", "room2offices", 0);
            CreateEvent("room2pit106", "room2pit", 0, 0.07f + 0.1f * AggressiveNpcFactor);
            CreateEvent("room1archive", "room1archive", 0, 1f);
        }

        // ── UpdateEvents dispatcher ─────────────────────────────────────────────

        public static void Update()
        {
            UpdateRoomDistances();
            MapSystem.UpdateRooms();

            var snapshot = _events.ToArray();
            foreach (var e in snapshot)
            {
                if (!_events.Contains(e)) continue;
                DispatchEvent(e);
            }

            UpdateEndings();
        }

        private static void DispatchEvent(GameEvent e)
        {
            switch (e.EventName)
            {
                case "exit1": UpdateExit1(e); break;
                case "alarm": UpdateAlarm(e); break;
                case "173": Update173(e); break;
                case "buttghost": UpdateButtghost(e); break;
                case "checkpoint": UpdateCheckpoint(e); break;
                case "coffin":
                case "coffin106": UpdateCoffin(e); break;
                case "endroom106": UpdateEndroom106(e); break;
                case "gateaentrance": UpdateGateAEntrance(e); break;
                case "lockroom173": UpdateLockroom173(e); break;
                case "lockroom096": UpdateLockroom096(e); break;
                case "pj": UpdatePj(e); break;
                case "pocketdimension": UpdatePocketDimension(e); break;
                case "room2cafeteria": UpdateRoom2Cafeteria(e); break;
                case "room2ccont": UpdateRoom2Ccont(e); break;
                case "room2closets": UpdateRoom2Closets(e); break;
                case "room2doors173": UpdateRoom2Doors173(e); break;
                case "room2elevator": UpdateRoom2Elevator(e); break;
                case "room2elevator2": UpdateRoom2Elevator2(e); break;
                case "room2fan": UpdateRoom2Fan(e); break;
                case "room2nuke": UpdateRoom2Nuke(e); break;
                case "room2offices2": UpdateRoom2Offices2(e); break;
                case "room2offices3": UpdateRoom2Offices3(e); break;
                case "room2tesla": UpdateRoom2Tesla(e); break;
                case "room2trick": UpdateRoom2Trick(e); break;
                case "room2tunnel": UpdateRoom2Tunnel(e); break;
                case "room2pipes106": UpdateRoom2Pipes106(e); break;
                case "room2pit106": UpdateRoom2Pit106(e); break;
                case "room2pit": UpdateRoom2Pit(e); break;
                case "room3pitduck": UpdateRoom3PitDuck(e); break;
                case "room3pit1048": UpdateRoom3Pit1048(e); break;
                case "room2poffices2": UpdateRoom2Poffices2(e); break;
                case "room2servers": UpdateRoom2Servers(e); break;
                case "room2storage": UpdateRoom2Storage(e); break;
                case "room2test1074": UpdateRoom2Test1074(e); break;
                case "room3door": UpdateRoom3Door(e); break;
                case "room3servers": UpdateRoom3Servers(e); break;
                case "room3storage": UpdateRoom3Storage(e); break;
                case "room3tunnel": UpdateRoom3Tunnel(e); break;
                case "room4": UpdateRoom4(e); break;
                case "room012": UpdateRoom012(e); break;
                case "room035": UpdateRoom035(e); break;
                case "room049": UpdateRoom049(e); break;
                case "room079": UpdateRoom079(e); break;
                case "room106": UpdateRoom106(e); break;
                case "room205": UpdateRoom205(e); break;
                case "room860": UpdateRoom860(e); break;
                case "room966": UpdateRoom966(e); break;
                case "room1123": UpdateRoom1123(e); break;
                case "testroom": UpdateTestroom(e); break;
                case "tunnel2smoke": UpdateTunnel2Smoke(e); break;
                case "tunnel2": UpdateTunnel2(e); break;
                case "tunnel106": UpdateTunnel106(e); break;
                case "testroom173": UpdateTestroom173(e); break;
                case "toiletguard": UpdateToiletGuard(e); break;
                case "008": Update008(e); break;
                case "106victim": Update106Victim(e); break;
                case "106sinkhole": Update106Sinkhole(e); break;
                case "682roar": Update682Roar(e); break;
                case "914": Update914(e); break;
                case "1048a": Update1048a(e); break;
                case "room4tunnels": UpdateRoom4Tunnels(e); break;
                case "room2gw_b": UpdateRoom2GwB(e); break;
                case "room2scps2": UpdateRoom2Scps2(e); break;
                case "room1162": UpdateRoom1162(e); break;
                case "room_gw": UpdateRoomGw(e); break;
                case "room2sl": UpdateRoom2Sl(e); break;
                case "096spawn": Update096Spawn(e); break;
                case "medibay": UpdateMedibay(e); break;
                case "dimension1499": UpdateDimension1499(e); break;
                case "room2offices035": UpdateRoom2Offices035(e); break;
                case "room1archive": UpdateRoom1Archive(e); break;
                case "room2shaft": UpdateRoom2Shaft(e); break;
                case "room1lifts": UpdateRoom1Lifts(e); break;
                case "gatea": UpdateGateA(e); break;
            }
        }

        // ── Shared helpers ──────────────────────────────────────────────────────

        private static void UpdateRoomDistances()
        {
            if (GameState.Collider == -1) return;
            float px = EntityX(GameState.Collider, true);
            float pz = EntityZ(GameState.Collider, true);

            foreach (var room in MapSystem.All)
            {
                float dx = px - room.x;
                float dz = pz - room.z;
                GetContext(room).Dist = MathUtil.PointDistance(px, pz, room.x, room.z);
            }
        }

        private static bool InPlayerRoom(GameEvent e) => GameState.PlayerRoom == e.Room;

        private static float GetRoomDist(RoomInstance room) => GetContext(room).Dist;

        private static void Advance(GameEvent e, float rate = 1f)
        {
            e.EventState += GameState.FpsFactor * rate;
        }

        private static void Advance2(GameEvent e, float rate = 1f)
        {
            e.EventState2 += GameState.FpsFactor * rate;
        }

        private static void Advance3(GameEvent e, float rate = 1f)
        {
            e.EventState3 += GameState.FpsFactor * rate;
        }

        private static bool CommotionState(int i)
        {
            if (i < 0 || i >= _commotionState.Length) return false;
            return _commotionState[i];
        }

        private static void SetCommotionState(int i, bool value)
        {
            if (i >= 0 && i < _commotionState.Length)
                _commotionState[i] = value;
        }

        private static GameEvent FindEvent(string name, RoomInstance room = null)
        {
            foreach (var ev in _events)
            {
                if (ev.EventName != name) continue;
                if (room != null && ev.Room != room) continue;
                return ev;
            }
            return null;
        }

        private static RoomInstance FindRoomByName(string name) =>
            MapSystem.FindRoomByName(name);

        // ── Complex events ──────────────────────────────────────────────────────

        private static void UpdateExit1(GameEvent e)
        {
            if (e.Room == null) return;
            var ctx = GetContext(e.Room);

            if (!RemoteDoorOn)
            {
                if (ctx.RoomDoors[4] != null)
                    ctx.RoomDoors[4].Locked = true;
            }
            else if (RemoteDoorOn && e.EventState3 == 0f)
            {
                if (ctx.RoomDoors[4] != null)
                {
                    ctx.RoomDoors[4].Locked = false;
                    if (ctx.RoomDoors[4].Open && (ctx.RoomDoors[4].OpenState > 50f ||
                        EntityDistance(GameState.Collider, ctx.RoomDoors[4].FrameObj) < 0.5f))
                    {
                        ctx.RoomDoors[4].OpenState = Math.Min(ctx.RoomDoors[4].OpenState, 50f);
                        ctx.RoomDoors[4].Open = false;
                    }
                }
            }
            else
            {
                if (ctx.RoomDoors[4] != null)
                    ctx.RoomDoors[4].Locked = false;

                if (NPCSystem.Curr096 != null &&
                    NPCSystem.Curr096.State != 0f && NPCSystem.Curr096.State != 5f)
                    e.EventState2 = EventElevatorHelper.UpdateElevators(e.EventState2, ctx.RoomDoors[0], ctx.RoomDoors[1], ctx.Objects[8], ctx.Objects[9], e);
                else
                    e.EventState2 = EventElevatorHelper.UpdateElevators(e.EventState2, ctx.RoomDoors[0], ctx.RoomDoors[1], ctx.Objects[8], ctx.Objects[9], e);
            }
        }

        private static void UpdatePocketDimension(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;

            GameState.Injuries += GameState.FpsFactor * 0.00005f;

            if (e.EventState == 0f)
            {
                RenderSystem.FogColor = Microsoft.Xna.Framework.Color.Black;
                e.EventState = 0.1f;
            }

            Advance(e);

            if (e.EventState2 == 0f)
            {
                if (ctxDoor(e, 0) != null) ctxDoor(e, 0).Open = false;
                if (ctxDoor(e, 1) != null) ctxDoor(e, 1).Open = false;

                if (NPCSystem.Curr106 != null && NPCSystem.Curr106.State > 0f)
                {
                    float angle = (e.EventState / 10f) % 360f;
                    PositionEntity(NPCSystem.Curr106.Collider, e.Room.x, 0.55f, e.Room.z, true);
                    RotateEntity(NPCSystem.Curr106.Collider, 0, angle + 90f, 0);
                    NPCSystem.Curr106.Idle = true;

                    if (e.EventState > 65f * 70f && Rand(0, 800) == 1)
                    {
                        NPCSystem.Curr106.State = -0.1f;
                        NPCSystem.Curr106.Idle = false;
                        e.EventState = 601f;
                    }
                }
            }
            else if (e.EventState2 == 1f)
            {
                if (e.EventState3 == 1f || e.EventState3 == 2f)
                {
                    if (e.EventState3 == 1f &&
                        (DoorOpenState(e, 0) > 150f || DoorOpenState(e, 1) > 150f))
                    {
                        GameState.BlurTimer = 800;
                        e.EventState3 = 2f;
                    }
                }
            }
            else if (e.EventState2 >= 12f && e.EventState2 <= 15f)
            {
                if (NPCSystem.Curr106 != null)
                {
                    int pillar = (int)e.EventState2;
                    var ctx = GetContext(e.Room);
                    if (ctx.Objects[pillar] != -1)
                    {
                        PositionEntity(NPCSystem.Curr106.Collider,
                            EntityX(ctx.Objects[pillar], true),
                            EntityY(ctx.Objects[pillar], true),
                            EntityZ(ctx.Objects[pillar], true), true);
                    }
                    e.EventState2 -= GameState.FpsFactor / 140f;
                    if (e.EventState2 < 12f) e.EventState2 = 0f;
                }
            }

            if (NPCSystem.Curr106 != null && GameState.Collider != -1 &&
                EntityDistance(GameState.Collider, NPCSystem.Curr106.Collider) < 0.3f)
            {
                NPCSystem.Curr106.Idle = false;
                NPCSystem.Curr106.State = -10f;
            }
        }

        private static Door ctxDoor(GameEvent e, int idx) => GetContext(e.Room).RoomDoors[idx];
        private static float DoorOpenState(GameEvent e, int idx)
        {
            var d = ctxDoor(e, idx);
            return d?.OpenState ?? 0f;
        }

        private static void UpdateRoom106(GameEvent e)
        {
            if (e.Room == null) return;
            var ctx = GetContext(e.Room);

            if (SoundTransmission && e.EventState == 1f)
                e.EventState3 = Math.Min(e.EventState3 + GameState.FpsFactor, 4000f);

            if (ctx.Npc[0] == null)
            {
                ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard,
                    e.Room.x, e.Room.y + 1.37f, e.Room.z);
            }

            if (!InPlayerRoom(e))
            {
                if (GameState.PlayerRoom?.def?.Name is "pocketdimension" or "dimension1499")
                    StopEventSounds(e);
                return;
            }

            if (ctx.Npc[0] != null)
            {
                ctx.Npc[0].State = 6f;
                if (ctx.Objects[5] != -1)
                {
                    PositionEntity(ctx.Npc[0].Collider,
                        EntityX(ctx.Objects[5], true),
                        EntityY(ctx.Objects[5], true) + 0.1f,
                        EntityZ(ctx.Objects[5], true), true);
                }
            }

            if (e.EventState == 0f)
            {
                if (SoundTransmission && Rand(0, 100) == 1)
                    e.EventState2 = 1f;
            }
            else if (e.EventState == 1f)
            {
                Advance3(e, GameState.FpsFactor);

                if (e.EventState3 >= 2500f)
                {
                    if (e.EventState2 == 1f && e.EventState3 - GameState.FpsFactor < 2500f)
                    {
                        if (NPCSystem.Curr106 != null && ctx.Objects[6] != -1)
                        {
                            PositionEntity(NPCSystem.Curr106.Collider,
                                EntityX(ctx.Objects[6], true),
                                EntityY(ctx.Objects[6], true),
                                EntityZ(ctx.Objects[6], true));
                            Contained106 = false;
                            NPCSystem.Curr106.Idle = false;
                            NPCSystem.Curr106.State = -11f;
                            e.EventState = 2f;
                        }
                    }
                    else if (NPCSystem.Curr106 != null && ctx.Objects[5] != -1)
                    {
                        float rise = Math.Min(e.EventState3 - 2500f, 800f) / 320f;
                        PositionEntity(NPCSystem.Curr106.Collider,
                            EntityX(ctx.Objects[5], true),
                            (700f + 108f * rise) * GameState.RoomScale,
                            EntityZ(ctx.Objects[5], true));
                        NPCSystem.Curr106.State = -11f;
                        NPCSystem.Curr106.Idle = true;
                    }

                    if (e.EventState3 > 3200f)
                    {
                        if (e.EventState2 >= 1f)
                            Contained106 = true;
                        else if (NPCSystem.Curr106 != null && ctx.Objects[6] != -1)
                        {
                            PositionEntity(NPCSystem.Curr106.Collider,
                                EntityX(ctx.Objects[6], true),
                                EntityY(ctx.Objects[6], true),
                                EntityZ(ctx.Objects[6], true));
                            Contained106 = false;
                            NPCSystem.Curr106.Idle = false;
                            NPCSystem.Curr106.State = -11f;
                            e.EventState = 2f;
                        }
                    }
                }
            }
        }

        private static void Update914(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);

            if (ctx.RoomDoors[2] != null && ctx.RoomDoors[2].Open)
                e.EventState2 = 1f;

            if (e.EventState > 0f)
            {
                Advance(e);
                if (ctx.RoomDoors[1] != null) ctx.RoomDoors[1].Open = false;

                if (e.EventState > 70f * 2f && ctx.RoomDoors[0] != null)
                    ctx.RoomDoors[0].Open = false;

                string setting = e.EventStr;
                if (string.IsNullOrEmpty(setting)) setting = "1:1";

                if (e.EventState > 70f * 3f)
                {
                    switch (setting)
                    {
                        case "rough":
                            GameState.KillTimer = Math.Min(-1, GameState.KillTimer);
                            GameState.BlinkTimer = -10f;
                            GameState.DeathMsg = "SCP-914 refined you on the Rough setting.";
                            break;
                        case "coarse":
                        case "1:1":
                        case "fine":
                        case "very fine":
                            GameState.BlinkTimer = -10f;
                            break;
                    }
                }

                if (e.EventState > 12f * 70f)
                {
                    float outX = ctx.Objects[3] != -1 ? EntityX(ctx.Objects[3], true) : 0f;
                    float outY = ctx.Objects[3] != -1 ? EntityY(ctx.Objects[3], true) : 0f;
                    float outZ = ctx.Objects[3] != -1 ? EntityZ(ctx.Objects[3], true) : 0f;

                    if (ctx.Objects[2] != -1)
                    {
                        float boothX = EntityX(ctx.Objects[2], true);
                        float boothZ = EntityZ(ctx.Objects[2], true);
                        float refineRadius = 180f * GameState.RoomScale;

                        foreach (var it in ItemSystem.All)
                        {
                            if (it.Picked || it.Collider == -1) continue;
                            float ix = EntityX(it.Collider, true);
                            float iz = EntityZ(it.Collider, true);
                            if (MathUtil.PointDistance(ix, iz, boothX, boothZ) < refineRadius)
                                ItemUseSystem.Use914(it, setting, outX, outY, outZ);
                        }
                    }

                    if (ctx.Objects[3] != -1 && GameState.Collider != -1)
                    {
                        GameState.BlurTimer = 1000;
                        PositionEntity(GameState.Collider, outX, outY + 1f, outZ);
                        ResetEntity(GameState.Collider);
                        GameState.DropSpeed = 0f;
                    }

                    if (setting == "coarse")
                    {
                        GameState.Injuries = 4f;
                        GameState.Msg = "You notice countless small incisions all around your body.";
                        GameState.MsgTimer = 70f * 8f;
                    }

                    if (ctx.RoomDoors[0] != null) ctx.RoomDoors[0].Open = true;
                    if (ctx.RoomDoors[1] != null) ctx.RoomDoors[1].Open = true;
                    e.EventState = 0f;
                }
            }
            else if (ctx.Objects[0] != -1 && ctx.Objects[1] != -1)
            {
                float roll0 = MathUtil.WrapAngle(EntityRoll(ctx.Objects[0]));
                if (roll0 > 90f && roll0 < 181f)
                {
                    e.EventState = 1f;
                    e.EventStr = Resolve914Setting(ctx.Objects[1]);
                }
            }
        }

        private static string Resolve914Setting(int dialObj)
        {
            float angle = MathUtil.WrapAngle(EntityRoll(dialObj));
            if (angle < 22.5f || angle > 337.5f) return "1:1";
            if (angle < 67.5f) return "coarse";
            if (angle < 180f) return "rough";
            if (angle > 292.5f) return "fine";
            return "very fine";
        }

        private static void UpdateGateAEntrance(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);

            if (e.EventState == 0f)
            {
                if (ctx.Objects[1] != -1 && GameState.Collider != -1 &&
                    EntityDistance(GameState.Collider, ctx.Objects[1]) < 4f)
                {
                    var gatea = FindRoomByName("gatea");
                    if (gatea != null)
                    {
                        if (ctx.RoomDoors[1] != null)
                            ctx.RoomDoors[1].Locked = true;
                        GameState.PlayerRoom = gatea;
                        RemoveEvent(e);
                    }
                }
            }
        }

        private static void UpdateGateA(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;

            if (e.EventState == 0f)
            {
                e.EventState = 1f;
                SecondaryLightOn = true;
                RenderSystem.FogEnabled = false;

                var ctx = GetContext(e.Room);
                for (int i = 2; i <= 4; i++)
                {
                    ctx.Npc[i] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard,
                        e.Room.x, e.Room.y + 11f, e.Room.z);
                    ctx.Npc[i].State = Contained106 ? 0f : 1f;
                }
                return;
            }

            Advance(e);

            if (NPCSystem.Curr106 != null && !Contained106 &&
                EntityDistance(GameState.Collider, NPCSystem.Curr106.Collider) < 12f)
            {
                NPCSystem.Curr106.State = -11f;
                GameState.Msg = "SCP-106 has breached containment.";
                GameState.MsgTimer = 70f * 6f;
            }
        }

        private static void UpdateEndings()
        {
            foreach (var e in _events.ToArray())
            {
                if (e.EventName == "exit1")
                    UpdateExit1Ending(e);
            }
        }

        private static void UpdateExit1Ending(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            if (GameState.Collider == -1) return;
            if (EntityY(GameState.Collider) <= 1040f * GameState.RoomScale) return;

            var ctx = GetContext(e.Room);

            if (e.EventState == 0f)
            {
                NPCSystem.Curr173 = null;
                NPCSystem.Curr106 = null;
                NPCSystem.Curr096 = null;
                RenderSystem.FogEnabled = false;
                e.EventState = 1f;
                return;
            }

            if (e.EventState < 2f && string.IsNullOrEmpty(SelectedEnding))
            {
                Advance2(e);
                if (ctx.Objects[10] != -1 &&
                    EntityDistance(GameState.Collider, ctx.Objects[10]) < 320f * GameState.RoomScale)
                {
                    e.EventState = 2f;
                    if (ctx.RoomDoors[2] != null) { ctx.RoomDoors[2].Open = false; ctx.RoomDoors[2].Locked = true; }
                    if (ctx.RoomDoors[3] != null) { ctx.RoomDoors[3].Open = false; ctx.RoomDoors[3].Locked = true; }
                }
            }
            else if (e.EventState >= 2f)
            {
                Advance(e);
                if (e.EventState > 0.6f * 70f)
                    PlayerSystem.SetCameraShake(0.5f);

                if (e.EventState >= 45f * 70f && string.IsNullOrEmpty(SelectedEnding))
                {
                    var nuke = FindEvent("room2nuke");
                    if (nuke != null && nuke.EventState >= 1f)
                        SelectedEnding = "B2";
                    else
                        SelectedEnding = "B3";
                }
            }
        }

        public static bool SecondaryLightOn;

        private static void StopEventSounds(GameEvent e)
        {
            e.SoundChn = -1;
            e.SoundChn2 = -1;
        }

        // ── Medium complexity events ────────────────────────────────────────────

        private static void UpdateLockroom173(GameEvent e)
        {
            if (e.Room == null || GetRoomDist(e.Room) >= 6f || GetRoomDist(e.Room) <= 0f) return;
            if (NPCSystem.Curr173 == null) return;

            if (NPCSystem.Curr173.Idle)
            {
                RemoveEvent(e);
                return;
            }

            if (!EntityVisible(NPCSystem.Curr173.Collider, GameState.Camera) ||
                EntityDistance(NPCSystem.Curr173.Collider, GameState.Collider) > 15f)
            {
                PositionEntity(NPCSystem.Curr173.Collider,
                    e.Room.x + (float)Math.Cos((225 - 90 + e.Room.ry) * Math.PI / 180) * 2f,
                    0.6f,
                    e.Room.z + (float)Math.Sin((225 - 90 + e.Room.ry) * Math.PI / 180) * 2f);
                ResetEntity(NPCSystem.Curr173.Collider);
                RemoveEvent(e);
            }
        }

        private static void UpdateLockroom096(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            if (NPCSystem.Curr096 == null)
            {
                NPCSystem.Curr096 = NPCSystem.CreateNpc(NPCSystem.NpcType096,
                    EntityX(e.Room.obj, true), 0.3f, EntityZ(e.Room.obj, true));
                RotateEntity(NPCSystem.Curr096.Collider, 0, e.Room.ry + 45f, 0, true);
            }
            RemoveEvent(e);
        }

        private static void UpdateRoom2Doors173(GameEvent e)
        {
            if (!InPlayerRoom(e) || e.EventState != 0f || NPCSystem.Curr173 == null) return;
            if (NPCSystem.Curr173.Idle) return;

            var ctx = GetContext(e.Room);
            if (!EntityVisible(NPCSystem.Curr173.Obj, GameState.Camera)) return;

            e.EventState = 1f;
            if (ctx.Objects[0] != -1)
            {
                PositionEntity(NPCSystem.Curr173.Collider,
                    EntityX(ctx.Objects[0], true), 0.5f, EntityZ(ctx.Objects[0], true));
                ResetEntity(NPCSystem.Curr173.Collider);
            }
            RemoveEvent(e);
        }

        private static void UpdateRoom2Elevator(GameEvent e)
        {
            if (e.Room == null) return;
            var ctx = GetContext(e.Room);

            if (e.EventState == 0f)
            {
                if (GetRoomDist(e.Room) < 8f && GetRoomDist(e.Room) > 0f)
                {
                    ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard,
                        EntityX(e.Room.obj, true), 0.5f, EntityZ(e.Room.obj, true));
                    e.EventState = 1f;
                }
            }
            else if (e.EventState == 1f)
            {
                if (GetRoomDist(e.Room) < 5f || Rand(0, 700) == 1)
                {
                    e.EventState = 2f;
                    if (ctx.Npc[0] != null)
                    {
                        ctx.Npc[0].State = 5f;
                        if (ctx.Objects[1] != -1)
                        {
                            ctx.Npc[0].EnemyX = EntityX(ctx.Objects[1], true);
                            ctx.Npc[0].EnemyY = EntityY(ctx.Objects[1], true);
                            ctx.Npc[0].EnemyZ = EntityZ(ctx.Objects[1], true);
                        }
                    }
                }
            }
            else if (e.EventState < 13f * 70f)
            {
                Advance(e);
                if (e.EventState > 12.6f * 70f)
                {
                    if (ctx.Npc[0] != null) NPCSystem.Remove(ctx.Npc[0]);
                    ctx.Npc[0] = null;
                    if (ctx.RoomDoors[0] != null) ctx.RoomDoors[0].Locked = false;
                }
            }
            else if (ctx.RoomDoors[0] != null && ctx.RoomDoors[0].Open)
            {
                ctx.RoomDoors[0].Locked = true;
                RemoveEvent(e);
            }
        }

        private static void UpdateRoom2Elevator2(GameEvent e)
        {
            if (e.Room == null || GetRoomDist(e.Room) >= 8f || GetRoomDist(e.Room) <= 0f) return;
            var ctx = GetContext(e.Room);
            ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard,
                ctx.Objects[0] != -1 ? EntityX(ctx.Objects[0], true) : e.Room.x,
                0.5f,
                ctx.Objects[0] != -1 ? EntityZ(ctx.Objects[0], true) : e.Room.z);
            ctx.Npc[0].State = 8f;
            RemoveEvent(e);
        }

        private static void UpdateRoom2Nuke(GameEvent e)
        {
            if (InPlayerRoom(e))
            {
                var ctx = GetContext(e.Room);
                e.EventState2 = EventElevatorHelper.UpdateElevators(e.EventState2, ctx.RoomDoors[0], ctx.RoomDoors[1], ctx.Objects[4], ctx.Objects[5], e);
                e.EventState = 1f;
            }

            if (e.EventState3 == 0f)
            {
                var ctx = GetContext(e.Room);
                if (ctx.Objects[6] != -1)
                {
                    var n = NPCSystem.CreateNpc(NPCSystem.NpcTypeGuard,
                        EntityX(ctx.Objects[6], true), 0.5f, EntityZ(ctx.Objects[6], true));
                    n.State = 3f;
                    n.IsDead = true;
                }
                e.EventState3 = 1f;
            }
        }

        private static void Update682Roar(GameEvent e)
        {
            if (e.EventState == 0f)
            {
                if (InPlayerRoom(e))
                    e.EventState = 70f * Rand(300, 1000);
            }
            else if (GameState.PlayerRoom?.def?.Name is not ("pocketdimension" or "room860" or "room1123" or "dimension1499"))
            {
                e.EventState -= GameState.FpsFactor;
                if (e.EventState < 17f * 70f && e.EventState + GameState.FpsFactor >= 17f * 70f)
                    AudioSystem.PlaySound2(AudioSystem.Load("SFX/SCP/682/Roar"), GameState.Camera, e.Room.obj);
                if (e.EventState > 17f * 70f - 3f * 70f)
                    PlayerSystem.SetCameraShake(0.5f);
                if (e.EventState < 70f)
                    RemoveEvent(e);
            }
        }

        private static void Update096Spawn(GameEvent e)
        {
            if (e.Room == null || GetRoomDist(e.Room) >= 35f) return;

            if (e.EventState == 2f) return;

            if (NPCSystem.Curr096 != null)
            {
                if (EntityDistance(NPCSystem.Curr096.Collider, GameState.Collider) < 40f)
                    e.EventState = 2f;
                if (NPCSystem.Curr096.State != 5f)
                    e.EventState = 2f;
            }

            if (InPlayerRoom(e))
                e.EventState = 2f;

            if (e.EventState == 0f && e.EventState != 2f)
            {
                if (NPCSystem.Curr096 == null)
                {
                    NPCSystem.Curr096 = NPCSystem.CreateNpc(NPCSystem.NpcType096,
                        e.Room.x, 0.3f, e.Room.z);
                }
                e.EventState = 1f;
                RemoveEvent(e);
            }
        }

        private static void UpdateRoom2Sl(GameEvent e)
        {
            if (InPlayerRoom(e) && e.EventState == 0f)
                e.EventState = 1f;

            if (e.EventState == 1f)
            {
                var ctx = GetContext(e.Room);
                if (e.EventState2 < 0f)
                    e.EventState2 = Math.Min(e.EventState2 + GameState.FpsFactor, 0f);
                else if (e.EventState2 == 0f && ctx.Npc[0] == null)
                {
                    ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcType049, e.Room.x, 0.5f, e.Room.z);
                    ctx.Npc[0].State = 5f;
                    e.EventState2 = 1f;
                }
                else if (e.EventState2 >= 1f)
                    Advance2(e);
            }
        }

        private static void UpdateTunnel106(GameEvent e)
        {
            if (e.Room == null || GetRoomDist(e.Room) >= 20f) return;
            if (e.EventState == 0f && NPCSystem.Curr106 != null && !NPCSystem.Curr106.Idle)
            {
                e.EventState = 1f;
                NPCSystem.Curr106.State = -11f;
            }
            if (e.EventState == 1f)
                Advance(e);
        }

        private static void UpdateCheckpoint(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);

            if (e.EventState2 == 0f && GameState.Collider != -1 &&
                EntityZ(GameState.Collider) < e.Room.z)
            {
                string sfx = EventSystem.PlayerZone == 1
                    ? "SFX/Ambient/ToZone2"
                    : "SFX/Ambient/ToZone3";
                AudioSystem.Load(sfx)?.Play(GameState.SfxVolume, 0f, 0f);
                e.EventState2 = 1f;
            }

            if (ctx.RoomDoors[0] != null)
                e.EventState = ctx.RoomDoors[0].Open ? 1f : 0f;

            if (e.Room.RoomName == "checkpoint2")
            {
                foreach (var e2 in _events)
                {
                    if (e2.EventName != "008") continue;
                    bool lockDoors = e2.EventState != 2f && GetRoomDist(e.Room) < 12f;
                    if (ctx.RoomDoors[0] != null) ctx.RoomDoors[0].Locked = lockDoors;
                    if (ctx.RoomDoors[1] != null) ctx.RoomDoors[1].Locked = lockDoors;
                    break;
                }
            }
        }

        private static void UpdateCoffin(GameEvent e)
        {
            if (PlayerZone <= 0) return;
            if (e.EventState < Environment.TickCount)
                e.EventState = Environment.TickCount + 3000;
        }

        private static void UpdateButtghost(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);
            if (ctx.Objects[0] == -1 || GameState.Collider == -1) return;

            if (EntityDistance(GameState.Collider, ctx.Objects[0]) < 1.8f)
            {
                if (e.EventState == 0f)
                {
                    e.EventState = 1f;
                    AudioSystem.PlaySound2(AudioSystem.Load("SFX/SCP/ButtGhost"), GameState.Camera, ctx.Objects[0]);
                }
                else
                    RemoveEvent(e);
            }
        }

        private static void UpdatePj(GameEvent e)
        {
            if (!InPlayerRoom(e) || e.EventState != 0f) return;
            if (EntityDistance(GameState.Collider, e.Room.obj) < 2.5f)
            {
                e.EventState = 1f;
                RemoveEvent(e);
            }
        }

        private static void UpdateRoom2Fan(GameEvent e)
        {
            if (InPlayerRoom(e))
                Advance3(e, 0.01f);
            if (GetRoomDist(e.Room) < 16f && e.EventState < 0f)
                e.EventState = Rand(15, 30) * 70f;
            else if (GetRoomDist(e.Room) < 16f)
                e.EventState -= GameState.FpsFactor;
        }
        private static void UpdateRoom2Tesla(GameEvent e)
        {
            if (e.Room == null || GameState.Collider == -1) return;
            var ctx = GetContext(e.Room);

            bool activeWindow = !(e.EventState2 > 70f * 3.5f && e.EventState2 < 70f * 90f);
            float roomY = EntityY(e.Room.obj, true);
            float playerY = EntityY(GameState.Collider, true);
            if (!activeWindow || playerY <= roomY || playerY >= 4f) return;

            float gateRadius = 300f * GameState.RoomScale;
            float killRadius = 250f * GameState.RoomScale;

            if (e.EventState == 0f)
            {
                if (GetRoomDist(e.Room) < 8f)
                {
                    if (ctx.Objects[3] != -1) HideEntity(ctx.Objects[3]);
                    if (ctx.Objects[4] != -1)
                    {
                        if ((Environment.TickCount % 1500) < 800)
                            ShowEntity(ctx.Objects[4]);
                        else
                            HideEntity(ctx.Objects[4]);
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        if (ctx.Objects[i] == -1) continue;
                        float dist = MathUtil.PointDistance(
                            EntityX(GameState.Collider, true), EntityZ(GameState.Collider, true),
                            EntityX(ctx.Objects[i], true), EntityZ(ctx.Objects[i], true));
                        if (dist < gateRadius && GameState.KillTimer >= 0)
                        {
                            e.EventState = 1f;
                            break;
                        }
                    }

                    if (e.EventState == 0f && string.IsNullOrEmpty(e.EventStr) && InPlayerRoom(e))
                    {
                        int spawnObj = 5;
                        if (ctx.Objects[5] != -1 && ctx.Objects[6] != -1 &&
                            EntityDistance(ctx.Objects[5], GameState.Collider) >
                            EntityDistance(ctx.Objects[6], GameState.Collider))
                            spawnObj = 6;

                        if (ctx.Objects[spawnObj] != -1)
                        {
                            ctx.Npc[0] = NPCSystem.CreateNpc(NPCSystem.NpcTypeClerk,
                                EntityX(ctx.Objects[spawnObj], true), 0.5f,
                                EntityZ(ctx.Objects[spawnObj], true));
                            if (ctx.Objects[2] != -1)
                                PointEntity(ctx.Npc[0].Collider, ctx.Objects[2]);
                            ctx.Npc[0].State = 2f;
                            e.EventStr = "step1";
                        }
                    }
                }
                else if (ctx.Objects[4] != -1)
                {
                    HideEntity(ctx.Objects[4]);
                }

                if (NPCSystem.Curr106 != null && NPCSystem.Curr106.State < -10f)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (ctx.Objects[i] == -1) continue;
                        float dist = MathUtil.PointDistance(
                            EntityX(NPCSystem.Curr106.Collider, true), EntityZ(NPCSystem.Curr106.Collider, true),
                            EntityX(ctx.Objects[i], true), EntityZ(ctx.Objects[i], true));
                        if (dist < gateRadius && GameState.KillTimer >= 0)
                        {
                            e.EventState = 1f;
                            NPCSystem.Curr106.State = 70f * 60f * Rand(10, 14);
                            AchievementSystem.Unlock("tesla");
                            if (ctx.Objects[4] != -1) HideEntity(ctx.Objects[4]);
                            break;
                        }
                    }
                }
            }
            else
            {
                e.EventState += GameState.FpsFactor;
                if (e.EventState <= 40f)
                {
                    if (ctx.Objects[3] != -1) HideEntity(ctx.Objects[3]);
                    if (ctx.Objects[4] != -1)
                    {
                        if ((Environment.TickCount % 100) < 50)
                            ShowEntity(ctx.Objects[4]);
                        else
                            HideEntity(ctx.Objects[4]);
                    }
                }
                else if (e.EventState < 70f && GameState.KillTimer >= 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (ctx.Objects[i] == -1) continue;
                        float dist = MathUtil.PointDistance(
                            EntityX(GameState.Collider, true), EntityZ(GameState.Collider, true),
                            EntityX(ctx.Objects[i], true), EntityZ(ctx.Objects[i], true));
                        if (dist < killRadius)
                        {
                            GameState.CameraShakeTimer = 30f;
                            GameState.DeathMsg = "Subject D-9341 killed by the Tesla gate at [REDACTED].";
                            GameState.KillTimer = 0f;
                            break;
                        }
                    }

                    if (e.EventStr == "step1" && ctx.Npc[0] != null)
                        ctx.Npc[0].State = 3f;
                }
            }

            e.EventState2 += GameState.FpsFactor;
        }
        private static void UpdateRoom2Servers(GameEvent e)
        {
            if (InPlayerRoom(e) && e.EventState == 0f) e.EventState = 1f;
            if (e.EventState > 0f) Advance(e);
        }
        private static void UpdateRoom205(GameEvent e)
        {
            if (!InPlayerRoom(e)) return;
            var ctx = GetContext(e.Room);
            if (ctx.RoomDoors[1] != null && ctx.RoomDoors[1].Open)
                e.EventState = 1f;
        }
        public static void Trigger1123Touch()
        {
            foreach (var e in _events)
            {
                if (e.EventName != "room1123") continue;
                e.EventState = Math.Max(1f, e.EventState);
                break;
            }
        }

        private static void UpdateRoom1Archive(GameEvent e)
        {
            if (InPlayerRoom(e) && e.EventState == 0f)
                e.EventState = 1f;
        }
        // ── Cleanup ─────────────────────────────────────────────────────────────

        public static void RemoveEvent(GameEvent e)
        {
            if (e.Sound != null) e.Sound = null;
            if (e.Sound2 != null) e.Sound2 = null;
            if (e.Img != -1) { FreeEntity(e.Img); e.Img = -1; }
            _events.Remove(e);
        }

        public static void FreeAll()
        {
            foreach (var e in _events)
            {
                if (e.Img != -1) FreeEntity(e.Img);
            }
            _events.Clear();
            _roomCtx.Clear();
            RemoteDoorOn = false;
            Contained106 = false;
            PlayerZone = 0;
            SoundTransmission = false;
            SelectedEnding = "";
            Array.Clear(_commotionState, 0, _commotionState.Length);
        }
    }
}