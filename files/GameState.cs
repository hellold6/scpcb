// GameState.cs — central globals ported from Main.bb

using SCPCB360.GameLogic;

namespace SCPCB360
{
    public enum GameScreen
    {
        MainMenu,
        Loading,
        Playing,
        Paused,
        Dead,
        Credits,
    }

    public static class GameState
    {
        public const string VersionNumber = "1.3.11";
        public const string CompatibleNumber = "1.3.11";
        public const string OptionFile = "options.ini";
        public const float RoomScale = 1f / 8f;

        public static GameScreen Screen = GameScreen.MainMenu;
        public static float FpsFactor = 1f;
        public static float FpsFactor2 = 1f;
        public static int PlayTime;
        public static bool Playable;
        public static bool GameSaved;

        // Player entities
        public static int Collider = -1;
        public static int Head = -1;
        public static int Camera = -1;
        public static int CamPivot = -1;

        // Player stats
        public static float Health = 100f;
        public static float Stamina = 100f;
        public static float StaminaEffect = 1f;
        public static float StaminaEffectTimer;
        public static float Sanity = 100f;
        public static float BlinkTimer = -10f;
        public static float BlinkEffect;
        public static float BlinkEffectTimer;
        public static bool Crouch;
        public static float CrouchState;
        public static float CurrSpeed;
        public static float DropSpeed;
        public static int DeathTimer = -1;
        public static int BlurTimer;
        public static float HealTimer;
        public static float Injuries;
        public static float Bloodloss;
        public static float Infect;
        public static float KillTimer = -1f;
        public static string DeathMsg = "";

        // Equipment flags
        public static bool WearingGasMask;
        public static int WearingVest;
        public static int WearingHazmat;
        public static int WearingNightVision;
        public static bool Wearing1499;
        public static int Wearing714;
        public static bool IsZombie;
        public static bool NoClip;
        public static bool UnableToMove;
        public static int ForceMove;
        public static float ForceAngle;
        public static float FallTimer;
        public static int GrabbedEntity = -1;
        public static bool Using294;
        public static bool CanSave = true;
        public static float LightBlink;
        public static bool NoTarget;

        // Map / session
        public static string RandomSeed = "";
        public static string SelectedMap = "";
        public static string CurrSave = "";
        public static int AccessCode;
        public static RoomInstance PlayerRoom;

        // Audio
        public static float SfxVolume = 1f;

        // UI / HUD (DrawGUI)
        public static string Msg = "";
        public static float MsgTimer;
        public static bool ShowFps;
        public static bool HudEnabled = IniConfig.GetInt(OptionFile, "options", "HUD enabled", 1) != 0;
        public static bool DebugHud;
        public static bool MenuOpen;
        public static bool InvOpen;
        public static int InvHoverSlot = 66;
        public static bool DrawHandIcon;
        public static bool[] DrawArrowIcon = new bool[4];
        public static float BlurVolume;
        public static float EndingTimer;
        public static Door SelectedDoor;
        public static int ClosestButton = -1;
        public static Door ClosestDoor;
        public static string KeypadInput = "";
        public static float KeypadTimer;
        public static string KeypadMsg = "";

        // Difficulty
        public static Difficulty SelectedDifficulty;

        // Extended save-state fields (Save.bb)
        public static float EyeStuck;
        public static float EyeIrritation;
        public static float PrevInjuries;
        public static float PrevBloodloss;
        public static float VomitTimer;
        public static bool Vomit;
        public static float CameraShakeTimer;
        public static float MonitorTimer;
        public static bool SuperMan;
        public static float SuperManTimer;
        public static bool LightsOn = true;
        public static float SecondaryLightOn;
        public static float PrevSecondaryLightOn;
        public static bool RemoteDoorOn;
        public static bool SoundTransmission;
        public static bool Contained106;
        public static int RefinedItems;
        public static bool UsedConsole;
        public static float CameraFogFar = 6f;
        public static float StoredCameraFogFar = 6f;
        public static float MtfTimer;
        public static readonly float[] Scp1025State = new float[6];

        public static void ResetForNewGame()
        {
            Health = 100f;
            Stamina = 100f;
            Sanity = 100f;
            BlinkTimer = -10f;
            BlurTimer = 100;
            DeathTimer = -1;
            KillTimer = -1f;
            Playable = false;
            GameSaved = false;
            Crouch = false;
            DropSpeed = 0f;
            Injuries = 0f;
            Bloodloss = 0f;
            Infect = 0f;
            AccessCode = 0;
            PlayerRoom = null;
            DeathMsg = "";
        }
    }
}