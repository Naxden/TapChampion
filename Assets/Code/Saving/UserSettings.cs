using System.Collections.Generic;

namespace Saving
{
    [System.Serializable]
    public class UserSettings
    {
        public int difficulty;
        public float userLag;
        public List<int> keys;
    }

    public enum Difficulty
    {
        EASY, MEDIUM, HARD
    }

    public enum GameKeys
    {
        BUTTON1, BUTTON2, BUTTON3, BUTTON4, BUTTON5,
        REC_PAUSE, REC_DELETE,
    }
}
