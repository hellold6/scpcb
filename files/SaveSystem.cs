// SaveSystem.cs — ports Save.bb (SaveGame / LoadGame / LoadSaveGames)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using SCPCB360.Engine;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    public static class SaveSystem
    {
        public const string SavePath = "Saves/";
        public const int MaxAchievements = 32;
        public static string SaveMsg = "";

        private static readonly List<string> _saveGames = new();
        public static IReadOnlyList<string> SaveGames => _saveGames;

        private const int MarkerNpcs = 113;
        private const int MarkerRooms = 632;
        private const int MarkerDoors = 954;
        private const int MarkerDecals = 1845;
        private const int MarkerConsole = 994;

        public static void LoadSaveGames()
        {
            _saveGames.Clear();
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);

            foreach (var dir in Directory.GetDirectories(SavePath))
            {
                if (File.Exists(Path.Combine(dir, "save.txt")))
                    _saveGames.Add(Path.GetFileName(dir));
            }
        }

        public static (string time, string date, string version) ReadSaveMetadata(string folderName)
        {
            string path = Path.Combine(SavePath, folderName, "save.txt");
            if (!File.Exists(path)) return ("", "", "");

            try
            {
                using var reader = new BinaryReader(File.OpenRead(path), Encoding.ASCII);
                string time = ReadBbString(reader);
                string date = ReadBbString(reader);
                reader.ReadInt32(); // play time
                for (int i = 0; i < 6; i++) reader.ReadSingle();
                ReadBbString(reader); // access code
                reader.ReadSingle();
                reader.ReadSingle();
                string version = ReadBbString(reader);
                return (time, date, version);
            }
            catch
            {
                return ("", "", "");
            }
        }

        public static bool SaveGame(string file)
        {
            if (!GameState.Playable) return false;
            if (GameState.DropSpeed > 0.02f * GameState.FpsFactor || GameState.DropSpeed < -0.02f * GameState.FpsFactor)
                return false;
            if (GameState.KillTimer < 0) return false;

            GameState.GameSaved = true;
            Directory.CreateDirectory(file);

            using var stream = File.Create(Path.Combine(file, "save.txt"));
            using var w = new BinaryWriter(stream, Encoding.ASCII);

            WriteBbString(w, DateTime.Now.ToLongTimeString());
            WriteBbString(w, DateTime.Now.ToLongDateString());
            w.Write(GameState.PlayTime);

            w.Write(EntityX(GameState.Collider));
            w.Write(EntityY(GameState.Collider));
            w.Write(EntityZ(GameState.Collider));

            w.Write(EntityX(GameState.Head));
            w.Write(EntityY(GameState.Head));
            w.Write(EntityZ(GameState.Head));

            WriteBbString(w, GameState.AccessCode.ToString(CultureInfo.InvariantCulture));

            w.Write(EntityPitch(GameState.Collider));
            w.Write(EntityYaw(GameState.Collider));

            WriteBbString(w, GameState.CompatibleNumber);

            w.Write(GameState.BlinkTimer);
            w.Write(GameState.BlinkEffect);
            w.Write(GameState.BlinkEffectTimer);

            w.Write(GameState.DeathTimer);
            w.Write(GameState.BlurTimer);
            w.Write(GameState.HealTimer);

            w.Write((byte)(GameState.Crouch ? 1 : 0));

            w.Write(GameState.Stamina);
            w.Write(GameState.StaminaEffect);
            w.Write(GameState.StaminaEffectTimer);

            w.Write(GameState.EyeStuck);
            w.Write(GameState.EyeIrritation);

            w.Write(GameState.Injuries);
            w.Write(GameState.Bloodloss);
            w.Write(GameState.PrevInjuries);
            w.Write(GameState.PrevBloodloss);

            WriteBbString(w, GameState.DeathMsg ?? "");

            for (int i = 0; i < 6; i++)
                w.Write(GameState.Scp1025State[i]);

            w.Write(GameState.VomitTimer);
            w.Write((byte)(GameState.Vomit ? 1 : 0));
            w.Write(GameState.CameraShakeTimer);
            w.Write(GameState.Infect);

            WriteDifficulty(w);

            w.Write(GameState.MonitorTimer);
            w.Write(GameState.Sanity);

            w.Write((byte)(GameState.WearingGasMask ? 1 : 0));
            w.Write((byte)GameState.WearingVest);
            w.Write((byte)GameState.WearingHazmat);
            w.Write((byte)GameState.WearingNightVision);
            w.Write((byte)(GameState.Wearing1499 ? 1 : 0));

            w.Write(0f); w.Write(0f); w.Write(0f); // NTF_1499 prev
            w.Write(0f); w.Write(0f); w.Write(0f); // NTF_1499
            w.Write(0f); w.Write(0f); // prev room

            w.Write((byte)(GameState.SuperMan ? 1 : 0));
            w.Write(GameState.SuperManTimer);
            w.Write((byte)(GameState.LightsOn ? 1 : 0));

            WriteBbString(w, GameState.RandomSeed ?? "");

            w.Write(GameState.SecondaryLightOn);
            w.Write(GameState.PrevSecondaryLightOn);
            w.Write((byte)(GameState.RemoteDoorOn ? 1 : 0));
            w.Write((byte)(GameState.SoundTransmission ? 1 : 0));
            w.Write((byte)(GameState.Contained106 ? 1 : 0));

            for (int i = 0; i < MaxAchievements; i++)
                w.Write((byte)0);

            w.Write(GameState.RefinedItems);

            w.Write(MapSystem.MapWidth);
            w.Write(MapSystem.MapHeight);
            for (int x = 0; x <= MapSystem.MapWidth; x++)
            for (int y = 0; y <= MapSystem.MapHeight; y++)
            {
                w.Write(MapSystem.MapTemp[x, y]);
                w.Write((byte)(MapSystem.MapFoundGrid[x, y] ? 1 : 0));
            }

            w.Write(MarkerNpcs);
            w.Write(NPCSystem.Count);
            foreach (var n in NPCSystem.All)
                WriteNpc(w, n);

            w.Write(GameState.MtfTimer);
            for (int i = 0; i < 7; i++)
            {
                WriteBbString(w, "a");
                w.Write(0);
            }

            w.Write(MarkerRooms);
            w.Write(0); // room2gw_brokendoor
            w.Write(0f); w.Write(0f); // room2gw_x/z

            w.Write((byte)MapSystem.ZoneTransition0);
            w.Write((byte)MapSystem.ZoneTransition1);
            w.Write((byte)0);
            w.Write((byte)0);

            w.Write(MapSystem.RoomCount);
            foreach (var room in MapSystem.All)
                WriteRoom(w, room);

            w.Write(MarkerDoors);
            w.Write(DoorSystem.All.Count);
            foreach (var d in DoorSystem.All)
                WriteDoor(w, d);

            w.Write(MarkerDecals);
            w.Write(DecalSystem.All.Count);
            foreach (var d in DecalSystem.All)
                WriteDecal(w, d);

            w.Write(EventSystem.All.Count);
            foreach (var e in EventSystem.All)
            {
                WriteBbString(w, e.EventName ?? "");
                w.Write(e.EventState);
                w.Write(e.EventState2);
                w.Write(e.EventState3);
                w.Write(e.Room?.x ?? 0f);
                w.Write(e.Room?.z ?? 0f);
                WriteBbString(w, e.EventStr ?? "");
            }

            w.Write(ItemSystem.All.Count);
            foreach (var it in ItemSystem.All)
                WriteItem(w, it);

            w.Write(0); // secondary inventories

            foreach (var t in ItemSystem.AllTemplates)
                w.Write((byte)(t.Found ? 1 : 0));

            w.Write(GameState.UsedConsole ? 100 : MarkerConsole);
            w.Write(GameState.CameraFogFar);
            w.Write(GameState.StoredCameraFogFar);
            w.Write((byte)0); // I_427 using
            w.Write(0f); // I_427 timer
            w.Write((byte)GameState.Wearing714);

            GameState.Msg = "Game progress saved.";
            GameState.MsgTimer = 70f * 4f;
            return true;
        }

        public static bool LoadGame(string file)
        {
            string path = Path.Combine(file, "save.txt");
            if (!File.Exists(path)) return false;

            GameState.DropSpeed = 0f;
            GameState.GameSaved = true;

            using var stream = File.OpenRead(path);
            using var r = new BinaryReader(stream, Encoding.ASCII);

            ReadBbString(r);
            ReadBbString(r);

            GameState.PlayTime = r.ReadInt32();

            float px = r.ReadSingle();
            float py = r.ReadSingle();
            float pz = r.ReadSingle();
            PositionEntity(GameState.Collider, px, py + 0.05f, pz, true);
            ResetEntity(GameState.Collider);

            float hx = r.ReadSingle();
            float hy = r.ReadSingle();
            float hz = r.ReadSingle();
            PositionEntity(GameState.Head, hx, hy + 0.05f, hz, true);
            ResetEntity(GameState.Head);

            GameState.AccessCode = int.Parse(ReadBbString(r), CultureInfo.InvariantCulture);

            float pitch = r.ReadSingle();
            float yaw = r.ReadSingle();
            RotateEntity(GameState.Collider, pitch, yaw, 0f);

            string version = ReadBbString(r);

            GameState.BlinkTimer = r.ReadSingle();
            GameState.BlinkEffect = r.ReadSingle();
            GameState.BlinkEffectTimer = r.ReadSingle();

            GameState.DeathTimer = r.ReadInt32();
            GameState.BlurTimer = r.ReadInt32();
            GameState.HealTimer = r.ReadSingle();

            GameState.Crouch = r.ReadByte() != 0;

            GameState.Stamina = r.ReadSingle();
            GameState.StaminaEffect = r.ReadSingle();
            GameState.StaminaEffectTimer = r.ReadSingle();

            GameState.EyeStuck = r.ReadSingle();
            GameState.EyeIrritation = r.ReadSingle();

            GameState.Injuries = r.ReadSingle();
            GameState.Bloodloss = r.ReadSingle();
            GameState.PrevInjuries = r.ReadSingle();
            GameState.PrevBloodloss = r.ReadSingle();

            GameState.DeathMsg = ReadBbString(r);

            for (int i = 0; i < 6; i++)
                GameState.Scp1025State[i] = r.ReadSingle();

            GameState.VomitTimer = r.ReadSingle();
            GameState.Vomit = r.ReadByte() != 0;
            GameState.CameraShakeTimer = r.ReadSingle();
            GameState.Infect = r.ReadSingle();

            ReadDifficulty(r);

            GameState.MonitorTimer = r.ReadSingle();
            GameState.Sanity = r.ReadSingle();

            GameState.WearingGasMask = r.ReadByte() != 0;
            GameState.WearingVest = r.ReadByte();
            GameState.WearingHazmat = r.ReadByte();
            GameState.WearingNightVision = r.ReadByte();
            GameState.Wearing1499 = r.ReadByte() != 0;

            for (int i = 0; i < 8; i++) r.ReadSingle(); // 1499 coords

            GameState.SuperMan = r.ReadByte() != 0;
            GameState.SuperManTimer = r.ReadSingle();
            GameState.LightsOn = r.ReadByte() != 0;

            GameState.RandomSeed = ReadBbString(r);

            GameState.SecondaryLightOn = r.ReadSingle();
            GameState.PrevSecondaryLightOn = r.ReadSingle();
            GameState.RemoteDoorOn = r.ReadByte() != 0;
            GameState.SoundTransmission = r.ReadByte() != 0;
            GameState.Contained106 = r.ReadByte() != 0;

            for (int i = 0; i < MaxAchievements; i++)
                r.ReadByte();

            GameState.RefinedItems = r.ReadInt32();

            int mapW = r.ReadInt32();
            int mapH = r.ReadInt32();
            for (int x = 0; x <= mapW; x++)
            for (int y = 0; y <= mapH; y++)
            {
                if (x <= MapSystem.MapWidth && y <= MapSystem.MapHeight)
                {
                    MapSystem.MapTemp[x, y] = r.ReadInt32();
                    MapSystem.MapFoundGrid[x, y] = r.ReadByte() != 0;
                }
                else
                {
                    r.ReadInt32();
                    r.ReadByte();
                }
            }

            if (r.ReadInt32() != MarkerNpcs)
                throw new InvalidDataException("Save corrupted (NPC marker)");

            NPCSystem.FreeAll();
            int npcCount = r.ReadInt32();
            var loadedNpcs = new List<NPC>();
            for (int i = 0; i < npcCount; i++)
                loadedNpcs.Add(ReadNpc(r));

            NPCSystem.ResolveTargets(loadedNpcs);

            GameState.MtfTimer = r.ReadSingle();
            for (int i = 0; i < 7; i++)
            {
                ReadBbString(r);
                r.ReadInt32();
            }

            if (r.ReadInt32() != MarkerRooms)
                throw new InvalidDataException("Save corrupted (room marker)");

            r.ReadInt32();
            r.ReadSingle();
            r.ReadSingle();

            if (version == GameState.CompatibleNumber)
            {
                MapSystem.ZoneTransition0 = r.ReadByte();
                MapSystem.ZoneTransition1 = r.ReadByte();
                r.ReadByte();
                r.ReadByte();
            }

            RoomTemplateSystem.LoadRoomTemplates();
            MapSystem.FreeAllRooms();
            DoorSystem.FreeAll();
            ItemSystem.FreeAll();
            EventSystem.FreeAll();
            DecalSystem.FreeAll();

            int roomCount = r.ReadInt32();
            for (int i = 0; i < roomCount; i++)
                ReadRoom(r);

            MapSystem.SpawnGridDoors();
            MapSystem.LinkAdjacentRooms();

            if (r.ReadInt32() != MarkerDoors)
                throw new InvalidDataException("Save corrupted (door marker)");

            int doorCount = r.ReadInt32();
            for (int i = 0; i < doorCount; i++)
                ReadDoor(r);

            MapSystem.InitWayPoints();

            if (r.ReadInt32() != MarkerDecals)
                throw new InvalidDataException("Save corrupted (decal marker)");

            int decalCount = r.ReadInt32();
            for (int i = 0; i < decalCount; i++)
                ReadDecal(r);

            int eventCount = r.ReadInt32();
            for (int i = 0; i < eventCount; i++)
                ReadEvent(r);

            int itemCount = r.ReadInt32();
            for (int i = 0; i < itemCount; i++)
                ReadItem(r);

            int otherInvCount = r.ReadInt32();
            for (int i = 0; i < otherInvCount; i++)
                SkipOtherInv(r);

            foreach (var t in ItemSystem.AllTemplates)
                t.Found = r.ReadByte() != 0;

            int consoleMarker = r.ReadInt32();
            GameState.UsedConsole = consoleMarker != MarkerConsole;

            GameState.CameraFogFar = r.ReadSingle();
            GameState.StoredCameraFogFar = r.ReadSingle();
            if (GameState.CameraFogFar == 0f) GameState.CameraFogFar = 6f;

            r.ReadByte();
            r.ReadSingle();

            if (version == "1.3.10")
            {
                MapSystem.ZoneTransition0 = r.ReadByte();
                MapSystem.ZoneTransition1 = r.ReadByte();
                r.ReadByte();
                r.ReadByte();
            }

            GameState.Wearing714 = r.ReadByte();

            GameState.Playable = true;
            GameState.Screen = GameScreen.Playing;
            return true;
        }

        private static void WriteDifficulty(BinaryWriter w)
        {
            int idx = Array.IndexOf(DifficultySystem.Difficulties, GameState.SelectedDifficulty);
            if (idx < 0) idx = 0;
            w.Write((byte)idx);
            if (idx == (int)DifficultyTier.Custom)
            {
                w.Write((byte)(GameState.SelectedDifficulty.AggressiveNpcs ? 1 : 0));
                w.Write((byte)(GameState.SelectedDifficulty.PermaDeath ? 1 : 0));
                w.Write((byte)GameState.SelectedDifficulty.SaveType);
                w.Write((byte)GameState.SelectedDifficulty.OtherFactors);
            }
        }

        private static void ReadDifficulty(BinaryReader r)
        {
            int idx = r.ReadByte();
            if (idx < 0 || idx >= DifficultySystem.Difficulties.Length)
                idx = 0;
            GameState.SelectedDifficulty = DifficultySystem.Difficulties[idx];
            if (idx == (int)DifficultyTier.Custom)
            {
                GameState.SelectedDifficulty.AggressiveNpcs = r.ReadByte() != 0;
                GameState.SelectedDifficulty.PermaDeath = r.ReadByte() != 0;
                GameState.SelectedDifficulty.SaveType = (SaveType)r.ReadByte();
                GameState.SelectedDifficulty.OtherFactors = (OtherFactors)r.ReadByte();
            }
        }

        private static void WriteNpc(BinaryWriter w, NPC n)
        {
            w.Write((byte)n.NpcType);
            w.Write(EntityX(n.Collider, true));
            w.Write(EntityY(n.Collider, true));
            w.Write(EntityZ(n.Collider, true));
            w.Write(EntityPitch(n.Collider));
            w.Write(EntityYaw(n.Collider));
            w.Write(EntityRoll(n.Collider));
            w.Write(n.State);
            w.Write(n.State2);
            w.Write(n.State3);
            w.Write(n.PrevState);
            w.Write((byte)(n.Idle ? 1 : 0));
            w.Write(n.LastDist);
            w.Write(n.LastSeen);
            w.Write((int)n.CurrSpeed);
            w.Write(n.Angle);
            w.Write(n.Reload);
            w.Write(n.Id);
            w.Write(n.Target?.Id ?? 0);
            w.Write(n.EnemyX);
            w.Write(n.EnemyY);
            w.Write(n.EnemyZ);
            WriteBbString(w, n.Texture ?? "");
            w.Write(n.Frame);
            w.Write(n.IsDead ? 1 : 0);
            w.Write(n.PathX);
            w.Write(n.PathZ);
            w.Write(n.HP);
            WriteBbString(w, n.Model ?? "");
            w.Write(n.ModelScaleX);
            w.Write(n.ModelScaleY);
            w.Write(n.ModelScaleZ);
            w.Write(n.TextureId);
        }

        private static NPC ReadNpc(BinaryReader r)
        {
            int type = r.ReadByte();
            var n = NPCSystem.CreateNpc(type,
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle());

            float rp = r.ReadSingle();
            float ry = r.ReadSingle();
            float rr = r.ReadSingle();
            RotateEntity(n.Collider, rp, ry, rr);

            n.State = r.ReadSingle();
            n.State2 = r.ReadSingle();
            n.State3 = r.ReadSingle();
            n.PrevState = r.ReadInt32();
            n.Idle = r.ReadByte() != 0;
            n.LastDist = r.ReadSingle();
            n.LastSeen = r.ReadInt32();
            n.CurrSpeed = r.ReadInt32();
            n.Angle = r.ReadSingle();
            n.Reload = r.ReadInt32();

            NPCSystem.ForceSetNpcId(n, r.ReadInt32());
            n.TargetId = r.ReadInt32();

            n.EnemyX = r.ReadSingle();
            n.EnemyY = r.ReadSingle();
            n.EnemyZ = r.ReadSingle();
            n.Texture = ReadBbString(r);
            n.Frame = r.ReadSingle();
            n.IsDead = r.ReadInt32() != 0;
            n.PathX = r.ReadSingle();
            n.PathZ = r.ReadSingle();
            n.HP = r.ReadInt32();
            n.Model = ReadBbString(r);
            n.ModelScaleX = r.ReadSingle();
            n.ModelScaleY = r.ReadSingle();
            n.ModelScaleZ = r.ReadSingle();
            n.TextureId = r.ReadInt32();

            switch (type)
            {
                case NPCSystem.NpcType173: NPCSystem.Curr173 = n; break;
                case NPCSystem.NpcTypeOldMan: NPCSystem.Curr106 = n; break;
                case NPCSystem.NpcType096: NPCSystem.Curr096 = n; break;
                case NPCSystem.NpcType5131: NPCSystem.Curr5131 = n; break;
            }

            return n;
        }

        private static void WriteRoom(BinaryWriter w, RoomInstance room)
        {
            w.Write(room.Template?.Id ?? 0);
            w.Write((int)room.Angle);
            w.Write(room.x);
            w.Write(room.y);
            w.Write(room.z);
            w.Write((byte)0); // found
            w.Write(room.zone);
            w.Write((byte)(GameState.PlayerRoom == room ? 1 : 0));

            for (int i = 0; i < 12; i++) w.Write(0);
            for (int i = 0; i < 11; i++) w.Write((byte)0);
            w.Write((byte)2);
            w.Write((byte)0); // no grid
            w.Write((byte)0); // no forest
        }

        private static void ReadRoom(BinaryReader r)
        {
            int templateId = r.ReadInt32();
            int angle = r.ReadInt32();
            float x = r.ReadSingle();
            float y = r.ReadSingle();
            float z = r.ReadSingle();
            bool found = r.ReadByte() != 0;
            int level = r.ReadInt32();
            bool isPlayerRoom = r.ReadByte() == 1;

            var template = RoomTemplateSystem.GetById(templateId);
            var room = MapSystem.CreateRoom(level, template?.Shape ?? MapSystem.ROOM1, x, y, z, template?.Name);
            if (room != null)
            {
                room.Angle = MathUtil.WrapAngle(angle);
                TurnEntity(room.mesh, 0, room.Angle, 0);
                room.Found = found;
                room.zone = level;
                if (isPlayerRoom)
                    GameState.PlayerRoom = room;
            }

            for (int i = 0; i < 12; i++)
                r.ReadInt32(); // NPC ids — resolved after NPC load

            for (int i = 0; i < 11; i++)
            {
                byte leverState = r.ReadByte();
                if (leverState == 2) break;
            }

            if (r.ReadByte() == 1)
            {
                for (int gy = 0; gy < 8; gy++)
                for (int gx = 0; gx < 8; gx++)
                {
                    r.ReadByte();
                    r.ReadByte();
                }
            }

            byte forest = r.ReadByte();
            if (forest > 0)
            {
                for (int gy = 0; gy < 16; gy++)
                for (int gx = 0; gx < 16; gx++)
                    r.ReadByte();
                r.ReadSingle();
                r.ReadSingle();
                r.ReadSingle();
            }
        }

        private static void WriteDoor(BinaryWriter w, Door d)
        {
            int frame = d.FrameObj != -1 ? d.FrameObj : d.Obj;
            w.Write(EntityX(frame, true));
            w.Write(EntityY(frame, true));
            w.Write(EntityZ(frame, true));
            w.Write((byte)(d.Open ? 1 : 0));
            w.Write(d.OpenState);
            w.Write((byte)(d.Locked ? 1 : 0));
            w.Write((byte)(d.AutoClose ? 1 : 0));
            w.Write(EntityX(d.Obj, true));
            w.Write(EntityZ(d.Obj, true));
            if (d.Obj2 != -1)
            {
                w.Write(EntityX(d.Obj2, true));
                w.Write(EntityZ(d.Obj2, true));
            }
            else
            {
                w.Write(0f);
                w.Write(0f);
            }
            w.Write(d.Timer);
            w.Write(d.TimerState);
            w.Write((byte)(d.IsElevatorDoor ? 1 : 0));
            w.Write((byte)(d.MtfClose ? 1 : 0));
        }

        private static void ReadDoor(BinaryReader r)
        {
            float fx = r.ReadSingle();
            float fy = r.ReadSingle();
            float fz = r.ReadSingle();
            bool open = r.ReadByte() != 0;
            float openState = r.ReadSingle();
            bool locked = r.ReadByte() != 0;
            bool autoClose = r.ReadByte() != 0;
            float objX = r.ReadSingle();
            float objZ = r.ReadSingle();
            float obj2X = r.ReadSingle();
            float obj2Z = r.ReadSingle();
            float timer = r.ReadSingle();
            float timerState = r.ReadSingle();
            bool isElev = r.ReadByte() != 0;
            bool mtfClose = r.ReadByte() != 0;

            var door = DoorSystem.FindByFramePosition(fx, fy, fz);
            DoorSystem.RestoreState(door, open, openState, locked, autoClose,
                objX, objZ, obj2X, obj2Z, timer, timerState, isElev, mtfClose);
        }

        private static void WriteDecal(BinaryWriter w, Decal d)
        {
            w.Write(d.Id);
            w.Write(EntityX(d.Obj, true));
            w.Write(EntityY(d.Obj, true));
            w.Write(EntityZ(d.Obj, true));
            w.Write(EntityPitch(d.Obj));
            w.Write(EntityYaw(d.Obj));
            w.Write(EntityRoll(d.Obj));
            w.Write((byte)d.BlendMode);
            w.Write(d.Fx);
            w.Write(d.Size);
            w.Write(d.Alpha);
            w.Write(d.AlphaChange);
            w.Write(d.Timer);
            w.Write(d.Lifetime);
        }

        private static void ReadDecal(BinaryReader r)
        {
            int id = r.ReadInt32();
            float x = r.ReadSingle();
            float y = r.ReadSingle();
            float z = r.ReadSingle();
            float pitch = r.ReadSingle();
            float yaw = r.ReadSingle();
            float roll = r.ReadSingle();
            var d = DecalSystem.Create(id, x, y, z, pitch, yaw, roll);
            d.BlendMode = r.ReadByte();
            d.Fx = r.ReadInt32();
            d.Size = r.ReadSingle();
            d.Alpha = r.ReadSingle();
            d.AlphaChange = r.ReadSingle();
            d.Timer = r.ReadSingle();
            d.Lifetime = r.ReadSingle();
        }

        private static void ReadEvent(BinaryReader r)
        {
            var e = EventSystem.CreateEvent(ReadBbString(r), "", 0);
            e.EventState = r.ReadSingle();
            e.EventState2 = r.ReadSingle();
            e.EventState3 = r.ReadSingle();
            float rx = r.ReadSingle();
            float rz = r.ReadSingle();
            e.EventStr = ReadBbString(r);

            foreach (var room in MapSystem.All)
            {
                if (Math.Abs(room.x - rx) < 0.01f && Math.Abs(room.z - rz) < 0.01f)
                {
                    e.Room = room;
                    break;
                }
            }
        }

        private static void WriteItem(BinaryWriter w, Item it)
        {
            WriteBbString(w, it.Template?.Name ?? "");
            WriteBbString(w, it.Template?.TempName ?? "");
            WriteBbString(w, it.Template?.Name ?? "");
            w.Write(EntityX(it.Collider, true));
            w.Write(EntityY(it.Collider, true));
            w.Write(EntityZ(it.Collider, true));
            w.Write((byte)255);
            w.Write((byte)255);
            w.Write((byte)255);
            w.Write(1f);
            w.Write(EntityPitch(it.Collider));
            w.Write(EntityYaw(it.Collider));
            w.Write(0f); // state
            w.Write((byte)(it.Picked ? 1 : 0));
            w.Write((byte)(ItemSystem.SelectedItem == it ? 1 : 0));
            w.Write((byte)66);
            w.Write((byte)0);
            w.Write(it.Id);
            w.Write((byte)0);
        }

        private static void ReadItem(BinaryReader r)
        {
            string name = ReadBbString(r);
            string tempName = ReadBbString(r);
            ReadBbString(r); // parent name
            float x = r.ReadSingle();
            float y = r.ReadSingle();
            float z = r.ReadSingle();
            int cr = r.ReadByte();
            int cg = r.ReadByte();
            int cb = r.ReadByte();
            float scale = r.ReadSingle();
            float pitch = r.ReadSingle();
            float yaw = r.ReadSingle();
            r.ReadSingle(); // state
            bool picked = r.ReadByte() != 0;
            bool selected = r.ReadByte() != 0;
            r.ReadByte();
            r.ReadByte();
            int id = r.ReadInt32();
            r.ReadByte();

            var it = ItemSystem.CreateItem(name, tempName, x, y, z, cr, cg, cb);
            if (it == null) return;

            ItemSystem.ForceSetItemId(it, id);
            RotateEntity(it.Collider, pitch, yaw, 0f);
            if (picked)
            {
                it.Picked = true;
                HideEntity(it.Collider);
                if (ItemSystem.ItemAmount < ItemSystem.MaxItemAmount)
                    ItemSystem.Inventory[ItemSystem.ItemAmount++] = it;
            }

            if (selected)
                ItemSystem.SelectedItem = it;
        }

        private static void SkipOtherInv(BinaryReader r)
        {
            int id = r.ReadInt32();
            for (int j = 0; j < 10; j++)
                r.ReadInt32();
        }

        private static void WriteBbString(BinaryWriter w, string value)
        {
            var bytes = Encoding.ASCII.GetBytes(value ?? "");
            w.Write(bytes.Length);
            w.Write(bytes);
        }

        private static string ReadBbString(BinaryReader r)
        {
            int len = r.ReadInt32();
            if (len <= 0) return "";
            return Encoding.ASCII.GetString(r.ReadBytes(len));
        }
    }
}