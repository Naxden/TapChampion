using System.Collections.Generic;

namespace Saving
{
    [System.Serializable]
    public class NoteFile
    {
        public string title = "";
        public string author = "";
        public int year;

        public List<float> highScores;
        public List<NoteObject> easy;
        public List<NoteObject> medium;
        public List<NoteObject> hard;
    }
    
    [System.Serializable]
    public struct NoteObject
    {
        public float spawnTime;
        public int buttonIndex;
        public int noteType;

        public NoteObject(float spawnTime = 0.001f, int buttonIndex = 0, int noteType = 0)
        {
            this.spawnTime = spawnTime;
            this.buttonIndex = buttonIndex;
            this.noteType = noteType;
        }
    }

    public enum NoteType
    {
        Short,
        LongBegin, LongEnd
    };
}


