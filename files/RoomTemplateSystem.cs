// RoomTemplateSystem.cs — ports RoomTemplates type + LoadRoomTemplates from MapSystem.bb

using System;
using System.Collections.Generic;
using System.IO;

namespace SCPCB360.GameLogic
{
    public class RoomTemplate
    {
        public const int MaxRoomEmitters = 8;
        public const int MaxTriggerboxes = 128;

        public int Obj = -1;
        public int Id;
        public string ObjPath = "";

        public int[] Zone = new int[5];

        public int[] TempSoundEmitter = new int[MaxRoomEmitters];
        public float[] TempSoundEmitterX = new float[MaxRoomEmitters];
        public float[] TempSoundEmitterY = new float[MaxRoomEmitters];
        public float[] TempSoundEmitterZ = new float[MaxRoomEmitters];
        public float[] TempSoundEmitterRange = new float[MaxRoomEmitters];

        public int Shape;
        public string Name = "";
        public int Commonness;
        public int Large;
        public int DisableDecals;

        public int TempTriggerboxAmount;
        public int[] TempTriggerbox = new int[MaxTriggerboxes];
        public string[] TempTriggerboxName = new string[MaxTriggerboxes];

        public int UseLightCones;
        public bool DisableOverlapCheck = true;

        public float MinX, MinY, MinZ;
        public float MaxX, MaxY, MaxZ;

        public string MeshAssetName
        {
            get
            {
                if (!string.IsNullOrEmpty(ObjPath))
                    return Path.ChangeExtension(ObjPath.Replace('\\', '/'), null);
                return string.IsNullOrEmpty(Name) ? "" : $"GFX/map/{Name}";
            }
        }

        public int PrimaryZone
        {
            get
            {
                for (int i = 0; i < Zone.Length; i++)
                    if (Zone[i] != 0) return Zone[i];
                return 0;
            }
        }
    }

    public static class RoomTemplateSystem
    {
        private static readonly List<RoomTemplate> _templates = new();
        private static readonly Dictionary<string, RoomTemplate> _byName =
            new(StringComparer.OrdinalIgnoreCase);
        private static int _nextId;

        public static IReadOnlyList<RoomTemplate> All => _templates;

        public static string DefaultIniPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "rooms.ini");

        public static void Clear()
        {
            _templates.Clear();
            _byName.Clear();
            _nextId = 0;
        }

        public static RoomTemplate CreateTemplate(string meshPath)
        {
            var rt = new RoomTemplate
            {
                ObjPath = meshPath,
                Id = _nextId++,
            };
            _templates.Add(rt);
            return rt;
        }

        public static void LoadRoomTemplates(string file = null)
        {
            file ??= DefaultIniPath;
            Clear();

            if (!File.Exists(file))
            {
                System.Diagnostics.Debug.WriteLine($"[RoomTemplate] Missing {file}");
                return;
            }

            foreach (string raw in File.ReadAllLines(file))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('#')) continue;
                if (!line.StartsWith('[') || !line.EndsWith(']')) continue;

                string section = line[1..^1].Trim();
                if (section.Equals("room ambience", StringComparison.OrdinalIgnoreCase))
                    continue;

                string meshPath = IniConfig.GetString(file, section, "mesh path");
                var rt = CreateTemplate(meshPath);
                rt.Name = section.ToLowerInvariant();

                string shape = IniConfig.GetString(file, section, "shape").ToLowerInvariant();
                rt.Shape = ParseShape(shape);

                for (int i = 0; i < 5; i++)
                    rt.Zone[i] = IniConfig.GetInt(file, section, "zone" + (i + 1));

                rt.Commonness = Math.Clamp(IniConfig.GetInt(file, section, "commonness"), 0, 100);
                rt.Large = IniConfig.GetInt(file, section, "large");
                rt.DisableDecals = IniConfig.GetInt(file, section, "disabledecals");
                rt.UseLightCones = IniConfig.GetInt(file, section, "usevolumelighting");
                rt.DisableOverlapCheck = IniConfig.GetInt(file, section, "disableoverlapcheck", 1) != 0;

                _byName[rt.Name] = rt;
            }

            System.Diagnostics.Debug.WriteLine($"[RoomTemplate] Loaded {_templates.Count} templates from {file}");
        }

        public static int ParseShape(string shape)
        {
            return shape switch
            {
                "room1" or "1" => MapSystem.ROOM1,
                "room2" or "2" => MapSystem.ROOM2,
                "room2c" or "2c" => MapSystem.ROOM2C,
                "room3" or "3" => MapSystem.ROOM3,
                "room4" or "4" => MapSystem.ROOM4,
                _ => 0,
            };
        }

        public static RoomTemplate GetByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            _byName.TryGetValue(name.ToLowerInvariant(), out var rt);
            return rt;
        }

        public static RoomTemplate GetById(int id)
        {
            foreach (var rt in _templates)
                if (rt.Id == id) return rt;
            return null;
        }

        public static RoomTemplate PickForZoneAndShape(int zone, int shape, Random rng)
        {
            int total = 0;
            foreach (var rt in _templates)
            {
                if (rt.Shape != shape) continue;
                for (int i = 0; i < 5; i++)
                {
                    if (rt.Zone[i] == zone)
                    {
                        total += rt.Commonness;
                        break;
                    }
                }
            }

            if (total <= 0) return null;

            int pick = rng.Next(total);
            int running = 0;
            foreach (var rt in _templates)
            {
                if (rt.Shape != shape) continue;
                bool inZone = false;
                for (int i = 0; i < 5; i++)
                {
                    if (rt.Zone[i] == zone) { inZone = true; break; }
                }
                if (!inZone) continue;

                running += rt.Commonness;
                if (pick >= running - rt.Commonness && pick < running)
                    return rt;
            }

            return null;
        }
    }
}