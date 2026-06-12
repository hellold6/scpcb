// DifficultySystem.cs — ports Difficulty.bb

namespace SCPCB360.GameLogic
{
    public enum DifficultyTier { Safe = 0, Euclid = 1, Keter = 2, Custom = 3 }
    public enum SaveType { SaveAnywhere = 0, SaveOnQuit = 1, SaveOnScreens = 2 }
    public enum OtherFactors { Easy = 0, Normal = 1, Hard = 2 }

    public class Difficulty
    {
        public string Name;
        public string Description;
        public bool PermaDeath;
        public bool AggressiveNpcs;
        public SaveType SaveType;
        public OtherFactors OtherFactors;
        public int R, G, B;
        public bool Customizable;
    }

    public static class DifficultySystem
    {
        public static readonly Difficulty[] Difficulties = new Difficulty[4];

        public static void Initialize()
        {
            Difficulties[(int)DifficultyTier.Safe] = new Difficulty
            {
                Name = "Safe",
                Description = "The game can be saved any time. However, as in the case of SCP Objects, a Safe classification does not mean that handling it does not pose a threat.",
                PermaDeath = false,
                AggressiveNpcs = false,
                SaveType = SaveType.SaveAnywhere,
                OtherFactors = OtherFactors.Easy,
                R = 120, G = 150, B = 50,
            };

            Difficulties[(int)DifficultyTier.Euclid] = new Difficulty
            {
                Name = "Euclid",
                Description = "In Euclid difficulty, saving is only allowed at specific locations marked by lit up computer screens. Euclid-class objects are inherently unpredictable, so that reliable containment is not always possible.",
                PermaDeath = false,
                AggressiveNpcs = false,
                SaveType = SaveType.SaveOnScreens,
                OtherFactors = OtherFactors.Normal,
                R = 200, G = 200, B = 0,
            };

            Difficulties[(int)DifficultyTier.Keter] = new Difficulty
            {
                Name = "Keter",
                Description = "Keter-class objects are considered the most dangerous ones in Foundation containment. The same can be said for this difficulty level: the SCPs are more aggressive, and you have only one life - when you die, the game is over.",
                PermaDeath = true,
                AggressiveNpcs = true,
                SaveType = SaveType.SaveOnQuit,
                OtherFactors = OtherFactors.Hard,
                R = 200, G = 0, B = 0,
            };

            Difficulties[(int)DifficultyTier.Custom] = new Difficulty
            {
                Name = "Custom",
                PermaDeath = false,
                AggressiveNpcs = true,
                SaveType = SaveType.SaveAnywhere,
                OtherFactors = OtherFactors.Easy,
                Customizable = true,
                R = 255, G = 255, B = 255,
            };

            GameState.SelectedDifficulty = Difficulties[(int)DifficultyTier.Safe];
        }
    }
}