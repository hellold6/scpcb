// IniConfig.cs — ports CB's INI file helpers from Main.bb

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SCPCB360.GameLogic
{
    public static class IniConfig
    {
        public static string GetString(string file, string section, string key, string defaultValue = "")
        {
            var data = Load(file);
            if (data.TryGetValue(section, out var sec) && sec.TryGetValue(key, out var val))
                return val;
            return defaultValue;
        }

        public static int GetInt(string file, string section, string key, int defaultValue = 0)
        {
            string s = GetString(file, section, key, defaultValue.ToString());
            return int.TryParse(s, out int v) ? v : defaultValue;
        }

        public static float GetFloat(string file, string section, string key, float defaultValue = 0f)
        {
            string s = GetString(file, section, key, defaultValue.ToString(CultureInfo.InvariantCulture));
            return float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float v) ? v : defaultValue;
        }

        private static Dictionary<string, Dictionary<string, string>> Load(string file)
        {
            var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(file)) return result;

            string current = "";
            foreach (string raw in File.ReadAllLines(file))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('#')) continue;

                if (line.StartsWith('[') && line.EndsWith(']'))
                {
                    current = line[1..^1].Trim();
                    if (!result.ContainsKey(current))
                        result[current] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    continue;
                }

                int eq = line.IndexOf('=');
                if (eq <= 0 || string.IsNullOrEmpty(current)) continue;

                string key = line[..eq].Trim();
                string val = line[(eq + 1)..].Trim();
                result[current][key] = val;
            }

            return result;
        }

        public static void PutValue(string file, string section, string key, string value)
        {
            var data = Load(file);
            if (!data.ContainsKey(section))
                data[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            data[section][key] = value;

            using var writer = new StreamWriter(file);
            foreach (var sec in data)
            {
                writer.WriteLine($"[{sec.Key}]");
                foreach (var kv in sec.Value)
                    writer.WriteLine($"{kv.Key}={kv.Value}");
                writer.WriteLine();
            }
        }
    }
}