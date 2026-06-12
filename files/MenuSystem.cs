// MenuSystem.cs — ports Menu.bb main menu flow (tabs, new game, load, options)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SCPCB360.Engine;
using SCPCB360.Input;
using static SCPCB360.Engine.B3D;

namespace SCPCB360.GameLogic
{
    /// <summary>Matches MainMenuTab% from Menu.bb (0=root, 1=new game, 2=load, 3/5/6/7=options, 4=map browser).</summary>
    public enum MainMenuTab
    {
        Root = 0,
        NewGame = 1,
        LoadGame = 2,
        Options = 3,
        LoadMap = 4,
        OptionsAudio = 5,
        OptionsControls = 6,
        OptionsAdvanced = 7,
    }

    public class SaveGameEntry
    {
        public string Name;
        public string Time;
        public string Date;
        public string Version;
    }

    public class SavedMapEntry
    {
        public string FileName;
        public string Author;
    }

    public static class MenuSystem
    {
        public static MainMenuTab CurrentTab = MainMenuTab.Root;
        public static int MenuSelection;
        public static int SubSelection;
        public static int DifficultySelection = (int)DifficultyTier.Safe;
        public static int OptionsSubTab;

        public static string RandomSeed = "";
        public static string CurrSave = "";
        public static string SelectedMap = "";

        public static float MenuBlinkTimer = 1f;
        public static float MenuBlinkDuration = 35f;
        public static string MenuStr = "DON'T BLINK";
        public static int MenuStrX = 700;
        public static int MenuStrY = 100;

        public static bool IntroEnabled = true;
        public static float LoadingProgress;
        public static int CurrLoadGamePage;
        public static string SaveMsg = "";

        public static int SavedMapsAmount;
        public static IReadOnlyList<SavedMapEntry> SavedMaps => _savedMaps;
        public static IReadOnlyList<SaveGameEntry> SaveGameEntries => _saveEntries;

        private static readonly string[] RootMenuItems =
        {
            "NEW GAME",
            "LOAD GAME",
            "OPTIONS",
            "QUIT",
        };

        private static readonly List<SaveGameEntry> _saveEntries = new();
        private static readonly List<SavedMapEntry> _savedMaps = new();

        private static readonly string SeedChars = "abcdefghijklmnopqrstuvwxyz0123456789";
        private static readonly string[] EasterEggSeeds =
        {
            "NIL", "NO", "d9341", "5CP_I73", "DONTBLINK", "CRUNCH", "die",
            "HTAED", "rustledjim", "larry", "JORGE", "dirtymetal", "whatpumpkin",
        };

        public static void Initialize()
        {
            IntroEnabled = IniConfig.GetInt(GameState.OptionFile, "options", "intro enabled", 1) != 0;
            RandomSeed = "";
            CurrSave = "";
            CurrentTab = MainMenuTab.Root;
            MenuSelection = 0;
            SaveSystem.LoadSaveGames();
            RefreshSaveEntries();
        }

        public static void Update()
        {
            if (GameState.Screen != GameScreen.MainMenu) return;

            XInputRouter.Update();
            UpdateBlinkText();

            switch (CurrentTab)
            {
                case MainMenuTab.Root:
                    UpdateRootMenu();
                    break;
                case MainMenuTab.NewGame:
                    UpdateNewGameTab();
                    break;
                case MainMenuTab.LoadGame:
                    UpdateLoadGameTab();
                    break;
                case MainMenuTab.LoadMap:
                    UpdateLoadMapTab();
                    break;
                case MainMenuTab.Options:
                case MainMenuTab.OptionsAudio:
                case MainMenuTab.OptionsControls:
                case MainMenuTab.OptionsAdvanced:
                    UpdateOptionsTab();
                    break;
            }
        }

        public static string[] GetMenuItems() => RootMenuItems;

        public static string GetTabTitle()
        {
            return CurrentTab switch
            {
                MainMenuTab.Root => "MAIN MENU",
                MainMenuTab.NewGame => "NEW GAME",
                MainMenuTab.LoadGame => "LOAD GAME",
                MainMenuTab.LoadMap => "LOAD MAP",
                MainMenuTab.Options => "OPTIONS — GRAPHICS",
                MainMenuTab.OptionsAudio => "OPTIONS — AUDIO",
                MainMenuTab.OptionsControls => "OPTIONS — CONTROLS",
                MainMenuTab.OptionsAdvanced => "OPTIONS — ADVANCED",
                _ => "MENU",
            };
        }

        public static IReadOnlyList<string> GetVisibleLines()
        {
            var lines = new List<string> { GetTabTitle(), "" };

            switch (CurrentTab)
            {
                case MainMenuTab.Root:
                    for (int i = 0; i < RootMenuItems.Length; i++)
                        lines.Add((i == MenuSelection ? "> " : "  ") + RootMenuItems[i]);
                    break;

                case MainMenuTab.NewGame:
                    lines.Add($"Save name: {SanitizeSaveName(CurrSave)}");
                    if (string.IsNullOrEmpty(SelectedMap))
                        lines.Add($"Map seed: {Truncate(RandomSeed, 15)}");
                    else
                        lines.Add($"Selected map: {Truncate(SelectedMap, 15)}");
                    lines.Add($"Intro sequence: {(IntroEnabled ? "ON" : "OFF")}");
                    lines.Add("");
                    lines.Add("Difficulty:");
                    for (int i = 0; i < DifficultySystem.Difficulties.Length; i++)
                    {
                        var d = DifficultySystem.Difficulties[i];
                        string mark = i == DifficultySelection ? "[x]" : "[ ]";
                        lines.Add($"  {mark} {d.Name}");
                    }
                    var sel = DifficultySystem.Difficulties[DifficultySelection];
                    if (sel.Customizable)
                    {
                        lines.Add($"  Permadeath: {sel.PermaDeath}");
                        lines.Add($"  Save anywhere: {sel.SaveType == SaveType.SaveAnywhere}");
                        lines.Add($"  Aggressive NPCs: {sel.AggressiveNpcs}");
                        lines.Add($"  Other factors: {sel.OtherFactors}");
                    }
                    else if (!string.IsNullOrEmpty(sel.Description))
                        lines.Add(Wrap(sel.Description, 42));
                    lines.Add("");
                    lines.Add(MenuSelection == 0 ? "> LOAD MAP" : "  LOAD MAP");
                    lines.Add(MenuSelection == 1 ? "> START" : "  START");
                    lines.Add("  BACK");
                    break;

                case MainMenuTab.LoadGame:
                    if (_saveEntries.Count == 0)
                    {
                        lines.Add("No saved games.");
                    }
                    else
                    {
                        int pageCount = Math.Max(1, (int)Math.Ceiling(_saveEntries.Count / 6.0));
                        lines.Add($"Page {CurrLoadGamePage + 1}/{pageCount}");
                        int start = CurrLoadGamePage * 6;
                        for (int i = start; i < Math.Min(start + 6, _saveEntries.Count); i++)
                        {
                            var e = _saveEntries[i];
                            bool compatible = e.Version == GameState.CompatibleNumber || e.Version == "1.3.10";
                            string prefix = i == MenuSelection ? "> " : "  ";
                            lines.Add($"{prefix}{e.Name} ({e.Time}) v{e.Version}{(compatible ? "" : " INCOMPATIBLE")}");
                        }
                    }
                    if (!string.IsNullOrEmpty(SaveMsg))
                        lines.Add($"Delete '{SaveMsg}'? (Y/N)");
                    lines.Add("  BACK");
                    break;

                case MainMenuTab.LoadMap:
                    if (_savedMaps.Count == 0)
                        lines.Add("No saved maps.");
                    else
                    {
                        int pageCount = Math.Max(1, (int)Math.Ceiling(_savedMaps.Count / 6.0));
                        lines.Add($"Page {CurrLoadGamePage + 1}/{pageCount}");
                        int start = CurrLoadGamePage * 6;
                        for (int i = start; i < Math.Min(start + 6, _savedMaps.Count); i++)
                        {
                            var m = _savedMaps[i];
                            lines.Add((i == MenuSelection ? "> " : "  ") + m.FileName);
                            lines.Add($"     by {m.Author}");
                        }
                    }
                    lines.Add("  BACK");
                    break;

                case MainMenuTab.Options:
                case MainMenuTab.OptionsAudio:
                case MainMenuTab.OptionsControls:
                case MainMenuTab.OptionsAdvanced:
                    lines.Add(OptionsSubTab == 0 ? "> GRAPHICS" : "  GRAPHICS");
                    lines.Add(OptionsSubTab == 1 ? "> AUDIO" : "  AUDIO");
                    lines.Add(OptionsSubTab == 2 ? "> CONTROLS" : "  CONTROLS");
                    lines.Add(OptionsSubTab == 3 ? "> ADVANCED" : "  ADVANCED");
                    lines.Add("");
                    lines.AddRange(GetOptionsLines());
                    lines.Add("  BACK (saves options)");
                    break;
            }

            return lines;
        }

        private static void UpdateRootMenu()
        {
            if (XInputRouter.IsPressed(CBAction.Inventory))
                MenuSelection = (MenuSelection + 1) % RootMenuItems.Length;

            if (!XInputRouter.IsPressed(CBAction.Interact)) return;

            switch (MenuSelection)
            {
                case 0:
                    RandomSeed = GenerateRandomSeed();
                    CurrentTab = MainMenuTab.NewGame;
                    MenuSelection = 1;
                    SubSelection = 0;
                    break;
                case 1:
                    SaveSystem.LoadSaveGames();
                    RefreshSaveEntries();
                    CurrLoadGamePage = 0;
                    MenuSelection = 0;
                    CurrentTab = MainMenuTab.LoadGame;
                    break;
                case 2:
                    OptionsSubTab = 0;
                    CurrentTab = MainMenuTab.Options;
                    MenuSelection = 0;
                    break;
                case 3:
                    Environment.Exit(0);
                    break;
            }
        }

        private static void UpdateNewGameTab()
        {
            const int actionCount = 3;

            if (XInputRouter.IsPressed(CBAction.Inventory))
                MenuSelection = (MenuSelection + 1) % actionCount;

            if (XInputRouter.IsPressed(CBAction.PauseMenu))
            {
                MenuSelection = Math.Max(0, MenuSelection - 1);
                return;
            }

            if (XInputRouter.IsPressed(CBAction.Sprint))
                CycleDifficulty(1);
            if (XInputRouter.IsPressed(CBAction.Crouch))
                CycleDifficulty(-1);

            if (XInputRouter.IsPressed(CBAction.Blink))
                IntroEnabled = !IntroEnabled;

            if (XInputRouter.IsPressed(CBAction.Flashlight))
                RandomSeed = AppendSeedChar();

            if (!XInputRouter.IsPressed(CBAction.Interact)) return;

            switch (MenuSelection)
            {
                case 0:
                    LoadSavedMaps();
                    CurrLoadGamePage = 0;
                    MenuSelection = 0;
                    CurrentTab = MainMenuTab.LoadMap;
                    break;
                case 1:
                    StartNewGame();
                    break;
                case 2:
                    IniConfig.PutValue(GameState.OptionFile, "options", "intro enabled", IntroEnabled ? "1" : "0");
                    CurrentTab = MainMenuTab.Root;
                    MenuSelection = 0;
                    break;
            }
        }

        private static void UpdateLoadGameTab()
        {
            if (!string.IsNullOrEmpty(SaveMsg))
            {
                if (XInputRouter.IsPressed(CBAction.Interact))
                {
                    string path = Path.Combine(SaveSystem.SavePath, SaveMsg);
                    try
                    {
                        if (File.Exists(Path.Combine(path, "save.txt")))
                            File.Delete(Path.Combine(path, "save.txt"));
                        if (Directory.Exists(path))
                            Directory.Delete(path);
                    }
                    catch { /* best effort */ }

                    SaveMsg = "";
                    SaveSystem.LoadSaveGames();
                    RefreshSaveEntries();
                }
                else if (XInputRouter.IsPressed(CBAction.PauseMenu))
                {
                    SaveMsg = "";
                }
                return;
            }

            int count = _saveEntries.Count;
            int maxSel = Math.Max(0, Math.Min(count - 1, CurrLoadGamePage * 6 + 5));

            if (XInputRouter.IsPressed(CBAction.Inventory) && count > 0)
                MenuSelection = Math.Min(maxSel, MenuSelection < maxSel ? MenuSelection + 1 : CurrLoadGamePage * 6);

            if (XInputRouter.IsPressed(CBAction.PauseMenu))
            {
                if (MenuSelection > CurrLoadGamePage * 6)
                    MenuSelection--;
                else
                {
                    CurrentTab = MainMenuTab.Root;
                    MenuSelection = 1;
                }
                return;
            }

            int pageCount = Math.Max(1, (int)Math.Ceiling(count / 6.0));
            if (XInputRouter.IsPressed(CBAction.Sprint) && CurrLoadGamePage < pageCount - 1)
            {
                CurrLoadGamePage++;
                MenuSelection = CurrLoadGamePage * 6;
            }
            if (XInputRouter.IsPressed(CBAction.Crouch) && CurrLoadGamePage > 0)
            {
                CurrLoadGamePage--;
                MenuSelection = CurrLoadGamePage * 6;
            }

            if (!XInputRouter.IsPressed(CBAction.Interact)) return;

            if (count == 0 || MenuSelection >= count)
            {
                CurrentTab = MainMenuTab.Root;
                MenuSelection = 1;
                return;
            }

            var entry = _saveEntries[MenuSelection];
            if (entry.Version != GameState.CompatibleNumber && entry.Version != "1.3.10")
                return;

            GameState.CurrSave = entry.Name;
            GameState.Screen = GameScreen.Loading;
            LoadingProgress = 0f;
            GameBootstrap.InitLoadGame(SaveSystem.SavePath + entry.Name + "/");
        }

        private static void UpdateLoadMapTab()
        {
            int count = _savedMaps.Count;
            int maxSel = Math.Max(0, Math.Min(count - 1, CurrLoadGamePage * 6 + 5));

            if (XInputRouter.IsPressed(CBAction.Inventory) && count > 0)
                MenuSelection = Math.Min(maxSel, MenuSelection < maxSel ? MenuSelection + 1 : CurrLoadGamePage * 6);

            if (XInputRouter.IsPressed(CBAction.PauseMenu))
            {
                if (MenuSelection > CurrLoadGamePage * 6)
                    MenuSelection--;
                else
                {
                    CurrentTab = MainMenuTab.NewGame;
                    MenuSelection = 0;
                }
                return;
            }

            int pageCount = Math.Max(1, (int)Math.Ceiling(count / 6.0));
            if (XInputRouter.IsPressed(CBAction.Sprint) && CurrLoadGamePage < pageCount - 1)
            {
                CurrLoadGamePage++;
                MenuSelection = CurrLoadGamePage * 6;
            }
            if (XInputRouter.IsPressed(CBAction.Crouch) && CurrLoadGamePage > 0)
            {
                CurrLoadGamePage--;
                MenuSelection = CurrLoadGamePage * 6;
            }

            if (!XInputRouter.IsPressed(CBAction.Interact)) return;

            if (count > 0 && MenuSelection < count)
            {
                SelectedMap = _savedMaps[MenuSelection].FileName;
                CurrentTab = MainMenuTab.NewGame;
                MenuSelection = 1;
            }
            else
            {
                CurrentTab = MainMenuTab.NewGame;
                MenuSelection = 0;
            }
        }

        private static void UpdateOptionsTab()
        {
            if (XInputRouter.IsPressed(CBAction.Inventory))
                OptionsSubTab = (OptionsSubTab + 1) % 4;

            CurrentTab = OptionsSubTab switch
            {
                0 => MainMenuTab.Options,
                1 => MainMenuTab.OptionsAudio,
                2 => MainMenuTab.OptionsControls,
                3 => MainMenuTab.OptionsAdvanced,
                _ => CurrentTab,
            };

            if (XInputRouter.IsPressed(CBAction.Blink))
                ToggleCurrentOption();

            if (XInputRouter.IsPressed(CBAction.Interact))
            {
                SaveOptionsIni();
                CurrentTab = MainMenuTab.Root;
                MenuSelection = 2;
            }
        }

        private static void StartNewGame()
        {
            if (string.IsNullOrWhiteSpace(CurrSave))
                CurrSave = "untitled";

            CurrSave = SanitizeSaveName(CurrSave);
            CurrSave = ResolveDuplicateSaveName(CurrSave);

            if (string.IsNullOrEmpty(RandomSeed) && string.IsNullOrEmpty(SelectedMap))
                RandomSeed = Math.Abs(Environment.TickCount).ToString();

            GameState.CurrSave = CurrSave;
            GameState.RandomSeed = RandomSeed;
            GameState.SelectedMap = SelectedMap;
            GameState.SelectedDifficulty = DifficultySystem.Difficulties[DifficultySelection];

            SeedRnd(MathUtil.GenerateSeedNumber(RandomSeed));

            GameState.Screen = GameScreen.Loading;
            LoadingProgress = 0f;
            IniConfig.PutValue(GameState.OptionFile, "options", "intro enabled", IntroEnabled ? "1" : "0");
            GameBootstrap.InitNewGame();
        }

        private static void RefreshSaveEntries()
        {
            _saveEntries.Clear();
            foreach (var name in SaveSystem.SaveGames)
            {
                var meta = SaveSystem.ReadSaveMetadata(name);
                _saveEntries.Add(new SaveGameEntry
                {
                    Name = name,
                    Time = meta.time,
                    Date = meta.date,
                    Version = meta.version,
                });
            }
        }

        public static void LoadSavedMaps()
        {
            _savedMaps.Clear();
            SavedMapsAmount = 0;

            string mapDir = Path.Combine("Map Creator", "Maps");
            if (!Directory.Exists(mapDir)) return;

            foreach (var file in Directory.GetFiles(mapDir))
            {
                string name = Path.GetFileName(file);
                if (!name.EndsWith(".cbmap", StringComparison.OrdinalIgnoreCase) &&
                    !name.EndsWith(".cbmap2", StringComparison.OrdinalIgnoreCase))
                    continue;

                string author = "[Unknown]";
                if (name.EndsWith(".cbmap2", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        using var reader = new StreamReader(file);
                        reader.ReadLine();
                        author = reader.ReadLine() ?? author;
                    }
                    catch { /* ignore */ }
                }

                _savedMaps.Add(new SavedMapEntry { FileName = name, Author = author });
                SavedMapsAmount++;
            }
        }

        private static void SaveOptionsIni()
        {
            IniConfig.PutValue(GameState.OptionFile, "options", "intro enabled", IntroEnabled ? "1" : "0");
            IniConfig.PutValue(GameState.OptionFile, "options", "bump mapping", IniConfig.GetInt(GameState.OptionFile, "options", "bump mapping", 0).ToString());
            IniConfig.PutValue(GameState.OptionFile, "options", "vsync", IniConfig.GetInt(GameState.OptionFile, "options", "vsync", 1).ToString());
            IniConfig.PutValue(GameState.OptionFile, "audio", "music volume", IniConfig.GetFloat(GameState.OptionFile, "audio", "music volume", 0.5f).ToString(System.Globalization.CultureInfo.InvariantCulture));
            IniConfig.PutValue(GameState.OptionFile, "audio", "sound volume", IniConfig.GetFloat(GameState.OptionFile, "audio", "sound volume", 1f).ToString(System.Globalization.CultureInfo.InvariantCulture));
            IniConfig.PutValue(GameState.OptionFile, "options", "show FPS", GameState.ShowFps ? "1" : "0");
        }

        private static IEnumerable<string> GetOptionsLines()
        {
            return CurrentTab switch
            {
                MainMenuTab.Options => new[]
                {
                    $"Bump mapping: {IniOn("options", "bump mapping")}",
                    $"VSync: {IniOn("options", "vsync")}",
                    $"Room lights: {IniOn("options", "room lights", 1)}",
                    $"Particle amount: {IniConfig.GetInt(GameState.OptionFile, "options", "particle amount", 2)}",
                },
                MainMenuTab.OptionsAudio => new[]
                {
                    $"Music volume: {IniConfig.GetFloat(GameState.OptionFile, "audio", "music volume", 0.5f):P0}",
                    $"Sound volume: {IniConfig.GetFloat(GameState.OptionFile, "audio", "sound volume", 1f):P0}",
                    $"SFX auto-release: {IniOn("audio", "sfx auto-release")}",
                    $"User tracks: {IniOn("audio", "user tracks")}",
                },
                MainMenuTab.OptionsControls => new[]
                {
                    $"Mouse sensitivity: {IniConfig.GetFloat(GameState.OptionFile, "controls", "mouse sensitivity", 0f):F2}",
                    $"Invert Y: {IniOn("controls", "invert mouse")}",
                    $"Mouse smoothing: {IniConfig.GetFloat(GameState.OptionFile, "controls", "mouse smoothing", 1f):F2}",
                },
                MainMenuTab.OptionsAdvanced => new[]
                {
                    $"Show HUD: {IniOn("options", "show hud", 1)}",
                    $"Enable console: {IniOn("options", "console", 1)}",
                    $"Achievement popups: {IniOn("options", "achievement popups", 1)}",
                    $"Show FPS: {(GameState.ShowFps ? "ON" : "OFF")}",
                },
                _ => Array.Empty<string>(),
            };
        }

        private static void ToggleCurrentOption()
        {
            switch (CurrentTab)
            {
                case MainMenuTab.Options:
                    if (SubSelection == 0) ToggleIni("options", "bump mapping");
                    else if (SubSelection == 1) ToggleIni("options", "vsync");
                    break;
                case MainMenuTab.OptionsAdvanced:
                    if (SubSelection == 3)
                        GameState.ShowFps = !GameState.ShowFps;
                    break;
            }
        }

        private static string IniOn(string section, string key, int defaultVal = 0)
            => IniConfig.GetInt(GameState.OptionFile, section, key, defaultVal) != 0 ? "ON" : "OFF";

        private static void ToggleIni(string section, string key)
        {
            int v = IniConfig.GetInt(GameState.OptionFile, section, key, 0);
            IniConfig.PutValue(GameState.OptionFile, section, key, v == 0 ? "1" : "0");
        }

        private static void CycleDifficulty(int dir)
        {
            DifficultySelection = (DifficultySelection + dir + DifficultySystem.Difficulties.Length) % DifficultySystem.Difficulties.Length;
            var d = DifficultySystem.Difficulties[DifficultySelection];
            if (d.Customizable)
            {
                d.PermaDeath = false;
                d.SaveType = SaveType.SaveAnywhere;
                d.AggressiveNpcs = true;
                d.OtherFactors = OtherFactors.Easy;
            }
        }

        private static string GenerateRandomSeed()
        {
            if (Rand(0, 14) == 1)
                return EasterEggSeeds[Rand(0, EasterEggSeeds.Length - 1)];

            int n = Rand(4, 8);
            var chars = new char[n];
            for (int i = 0; i < n; i++)
                chars[i] = Rand(0, 2) == 1 ? (char)('0' + Rand(0, 9)) : SeedChars[Rand(0, SeedChars.Length - 1)];
            return new string(chars);
        }

        private static string AppendSeedChar()
        {
            if (RandomSeed.Length >= 15) return RandomSeed;
            return RandomSeed + SeedChars[Rand(0, SeedChars.Length - 1)];
        }

        private static string ResolveDuplicateSaveName(string name)
        {
            int same = SaveSystem.SaveGames.Count(s => s == name || s.StartsWith(name + " (", StringComparison.Ordinal));
            return same > 0 ? $"{name} ({same + 1})" : name;
        }

        private static string SanitizeSaveName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            char[] bad = { ':', '.', '/', '\\', '<', '>', '|', '?', '*', '"' };
            foreach (var c in bad) name = name.Replace(c.ToString(), "");
            return name.Length > 15 ? name[..15] : name;
        }

        private static void UpdateBlinkText()
        {
            MenuBlinkTimer -= GameState.FpsFactor;
            if (MenuBlinkTimer >= MenuBlinkDuration) return;

            if (MenuBlinkTimer < 0f)
            {
                MenuBlinkTimer = Rand(700, 800);
                MenuBlinkDuration = Rand(10, 35);
                MenuStrX = Rand(700, 1000);
                MenuStrY = Rand(100, 600);
                MenuStr = PickRandomMenuStr();
            }
        }

        private static string PickRandomMenuStr()
        {
            return Rand(0, 22) switch
            {
                0 or 2 or 3 => "DON'T BLINK",
                4 or 5 => "Secure. Contain. Protect.",
                6 or 7 or 8 => "You want happy endings? Fuck you.",
                9 or 10 or 11 => "Sometimes we would have had time to scream.",
                12 or 19 => "NIL",
                13 => "NO",
                14 => "black white black white black white gray",
                15 => "Stone does not care",
                16 => "9341",
                17 => "It controls the doors",
                18 => "e8m106]af173o+079m895w914",
                20 => "It has taken over everything",
                21 => "The spiral is growing",
                22 => "\"Some kind of gestalt effect due to massive reality damage.\"",
                _ => "DON'T BLINK",
            };
        }

        private static string Truncate(string s, int max)
            => string.IsNullOrEmpty(s) ? "" : (s.Length > max ? s[..max] : s);

        private static string Wrap(string text, int width)
        {
            if (string.IsNullOrEmpty(text)) return "";
            var words = text.Split(' ');
            var line = "";
            var result = "";
            foreach (var w in words)
            {
                if (line.Length + w.Length + 1 > width)
                {
                    result += line.Trim() + "\n";
                    line = w + " ";
                }
                else line += w + " ";
            }
            return result + line.Trim();
        }
    }
}