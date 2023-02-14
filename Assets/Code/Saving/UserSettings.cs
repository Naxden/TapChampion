using System.Collections.Generic;

namespace Saving
{
    [System.Serializable]
    public class UserSettings
    {
        public int difficulty;
        public List<int> keyBinds;
    }

    public enum Difficulty
    {
        EASY, MEDIUM, HARD
    }
}
