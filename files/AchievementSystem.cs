// AchievementSystem.cs — ports Achievements.bb

using System.Collections.Generic;

namespace SCPCB360.GameLogic
{
    public class Achievement
    {
        public string Id;
        public string Name;
        public string Description;
        public bool Unlocked;
    }

    public static class AchievementSystem
    {
        private static readonly List<Achievement> _achievements = new();

        public static void Initialize()
        {
            _achievements.Clear();
            Register("escape", "Escape", "Escape the facility.");
            Register("173", "Don't Blink", "Survive SCP-173.");
            Register("106", "Old Man", "Encounter SCP-106.");
            Register("096", "Shy Guy", "Trigger SCP-096.");
            Register("049", "Plague Doctor", "Encounter SCP-049.");
            Register("914", "The Clockworks", "Use SCP-914.");
            Register("tesla", "You'll Die Anyway", "Get killed by SCP-106 in the Tesla gate.");
            Register("500", "Blue Key", "Use SCP-500.");
            Register("714", "Unlimited Sandwiches", "Put on SCP-714.");
            Register("420", "Anomalous D-Class", "Use SCP-420-J.");
        }

        private static void Register(string id, string name, string description)
        {
            _achievements.Add(new Achievement { Id = id, Name = name, Description = description });
        }

        public static void Unlock(string id)
        {
            var a = _achievements.Find(x => x.Id == id);
            if (a != null && !a.Unlocked)
                a.Unlocked = true;
        }

        public static IReadOnlyList<Achievement> All => _achievements;
    }
}