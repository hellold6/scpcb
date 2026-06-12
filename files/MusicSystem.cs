// MusicSystem.cs — ports Main.bb UpdateMusic + zone/room track selection

using Microsoft.Xna.Framework.Audio;

namespace SCPCB360.GameLogic
{
    public static class MusicSystem
    {
        private static readonly string[] Tracks =
        {
            "SFX/Music/The Dread",           // 0 LCZ
            "SFX/Music/HeavyContainment",    // 1 HCZ
            "SFX/Music/EntranceZone",        // 2 EZ
            "SFX/Music/PD",                  // 3
            "SFX/Music/079",                 // 4
            "SFX/Music/GateB1",              // 5
            "SFX/Music/GateB2",              // 6
            "SFX/Music/Room3Storage",        // 7
            "SFX/Music/Room049",             // 8
            "SFX/Music/8601",                // 9
            "SFX/Music/106",                 // 10 pocket dimension
            "SFX/Music/Menu",                // 11
            "SFX/Music/8601Cancer",          // 12
            "SFX/Music/Intro",               // 13
            "SFX/Music/178",                 // 14
            "SFX/Music/PDTrench",            // 15
            "SFX/Music/205",                 // 16
            "SFX/Music/GateA",               // 17
            "SFX/Music/1499",                // 18
            "SFX/Music/1499Danger",          // 19
            "SFX/Music/049Chase",            // 20
            "SFX/Music/SaveMeFrom",          // 21
            "SFX/Music/914",                 // 22
            "SFX/Music/Ending",              // 23
            "SFX/Music/Credits",             // 24
            "SFX/Music/420J",                // 25
        };

        private static SoundEffectInstance _instance;
        private static int _nowPlaying = 66;
        private static float _currVolume;
        private static int _forcedTrack = 66;

        public static void ForceTrack(int index) => _forcedTrack = index;
        public static void ClearForcedTrack() => _forcedTrack = 66;

        public static void Update()
        {
            float musicVolume = IniConfig.GetFloat(GameState.OptionFile, "audio", "music volume", 0.5f);
            int shouldPlay = ResolveShouldPlay();

            if (shouldPlay >= Tracks.Length)
                shouldPlay = 11;

            if (_nowPlaying != shouldPlay)
            {
                _currVolume = System.Math.Max(_currVolume - GameState.FpsFactor / 250f, 0f);
                if (_currVolume <= 0f)
                {
                    StopCurrent();
                    _nowPlaying = shouldPlay;
                    StartTrack(_nowPlaying);
                }
            }
            else
            {
                _currVolume += (musicVolume - _currVolume) * (0.1f * GameState.FpsFactor);
            }

            if (_instance != null)
                _instance.Volume = System.Math.Clamp(_currVolume, 0f, 1f);
        }

        private static int ResolveShouldPlay()
        {
            if (_forcedTrack < 66) return _forcedTrack;

            if (GameState.Screen == GameScreen.MainMenu ||
                GameState.Screen == GameScreen.Loading)
                return 11;

            if (GameState.Screen != GameScreen.Playing)
                return _nowPlaying < 66 ? _nowPlaying : 11;

            string room = GameState.PlayerRoom?.RoomName ?? "";
            switch (room)
            {
                case "pocketdimension": return 10;
                case "room079": return 4;
                case "gatea": return 17;
                case "dimension1499": return 18;
                case "room914": return 22;
                case "room049": return 8;
                case "room860": return 9;
                case "room205": return 16;
                case "room2test": return 13;
            }

            if (NPCSystem.Curr096 != null && NPCSystem.Curr096.State >= 1f)
                return 20;

            return System.Math.Min(EventSystem.PlayerZone, 2);
        }

        private static void StartTrack(int index)
        {
            if (index < 0 || index >= Tracks.Length) return;

            var sfx = AudioSystem.Load(Tracks[index]);
            if (sfx == null) return;

            _instance = sfx.CreateInstance();
            _instance.IsLooped = true;
            _instance.Volume = 0f;
            _instance.Play();
        }

        private static void StopCurrent()
        {
            if (_instance == null) return;
            _instance.Stop();
            _instance.Dispose();
            _instance = null;
        }

        public static void StopAll()
        {
            StopCurrent();
            _nowPlaying = 66;
            _currVolume = 0f;
            _forcedTrack = 66;
        }
    }
}