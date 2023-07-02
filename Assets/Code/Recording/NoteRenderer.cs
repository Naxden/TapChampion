using System.Collections.Generic;
using UnityEngine;
using Saving;
using Recording.Note;
using Slider = UnityEngine.UI.Slider;
using System.Linq;

namespace Recording
{
    using Track = List<GameObject>;

    public class NoteRenderer : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] notePrefabs = new GameObject[3];

        [SerializeField]
        private Slider slider;

        [SerializeField] 
        private Transform timeLine;

        private readonly float[] tracksPositions = {1.25f, 0f, -1.25f, -2.5f, -3.75f};

        private List<Track> tracksList;
        private float songLength;


        private void Start()
        {
            CreateEmptyNotes();
        }

        private void CreateEmptyNotes()
        {
            tracksList = new List<Track>(5);

            for (int i = 0; i < 5; i++)
                tracksList.Add(new Track(128)); // using some initial value as size of list
        }

        // function called externally by Event
        public void SetSongLength(float songLength)
        {
            this.songLength = songLength;
        }

        public float[] GetTrackPositions()
        {
            return tracksPositions;
        }

        public bool TryAddNote(Vector3 notePos, NoteType noteType)
        {
            float xPos = notePos.x;
            float yPos = notePos.y;

            if (yPos < -4.25f || yPos > 1.5f || xPos < -5f || xPos > (songLength * 10) - 5f)
                return false;

            int track = WhichTrack(yPos);

            xPos = Mathf.Ceil(xPos / 0.75f) * 0.75f - 0.5f;
            Vector3 newPos = new Vector3(xPos, tracksPositions[track]);

            if (IsNoteThere(newPos) != null)
                return false;

            GameObject note = Instantiate(notePrefabs[(int)noteType], newPos, Quaternion.identity);
            note.transform.SetParent(transform, false);

            tracksList[track].Add(note);

            if (noteType == NoteType.LongBegin)
            {
                LongNoteConnector longNoteConnector = note.GetComponent<LongNoteConnector>();
                longNoteConnector.SetEnd(timeLine);
                slider.onValueChanged.AddListener(longNoteConnector.UpdateLine);
            }
            else if (noteType == NoteType.LongEnd)
            {
                int indexOfPreLastNote = tracksList[track].Count - 2;
                if (indexOfPreLastNote < 0)
                {
                    Debug.LogWarning("Adding connection, Begin-End notes, index less than zero");
                    return false;
                }
                    
                LongNote longBegin = tracksList[track][indexOfPreLastNote].transform.GetComponent<LongNote>();
                longBegin.SetOtherHalf(note);
                note.GetComponent<LongNote>().SetOtherHalf(longBegin.gameObject);
                longBegin.InitializePair();

                LongNoteConnector longNoteConnector = longBegin.gameObject.GetComponent<LongNoteConnector>();
                slider.onValueChanged.RemoveListener(longNoteConnector.UpdateLine);
                longNoteConnector.SetEnd(note.transform);
                longNoteConnector.UpdateLine(0f);
            }

            return true;
        }

        private int WhichTrack(float yPos)
        {
            int track = 0;
            float distance = Mathf.Abs(yPos - tracksPositions[track]);

            for (int i = 1; i < 5; i++)
            {
                float temp = Mathf.Abs(yPos - tracksPositions[i]);

                if (temp < distance)
                {
                    distance = temp;
                    track = i;
                }
            }

            return track;
        }

        public GameObject IsNoteThere(Vector3 pos)
        {
            RaycastHit2D ray = Physics2D.Raycast(pos, Vector2.zero, 500f);
            
            if (ray.collider == null || !ray.transform.CompareTag("Note"))
                return null;

            return ray.transform.gameObject;
        }

        public bool IsNoteBetween(Vector3 pos1, Vector3 pos2)
        {
            pos1.x = Mathf.Ceil(pos1.x / 0.75f) * 0.75f - 0.5f;
            pos2.x = Mathf.Ceil(pos2.x / 0.75f) * 0.75f - 0.5f;

            Vector3 checkingPos = pos1;

            for (float x = pos1.x + 0.75f; x <= pos2.x; x += 0.75f)
            {
                checkingPos.x = x;
                if (IsNoteThere(checkingPos) != null)
                    return true;
            }

            return false;
        }

        // function called externally by Event
        public void ClearTracks()
        {
            if (tracksList == null)
                return;
             
            for (int i = 0; i < tracksList.Count; i++) 
            {
                foreach (var note in tracksList[i])
                    Destroy(note);
                tracksList[i].Clear();
            }
        }

        public void DeleteNote(Vector3 position)
        {
            position.x = Mathf.Ceil(position.x / 0.75f) * 0.75f - 0.5f; ;
            GameObject deleteNote = IsNoteThere(position);

            if (deleteNote == null)
                return;

            int track = WhichTrack(position.y);

            tracksList[track].Remove(deleteNote);
            Destroy(deleteNote);
        }

        public void MoveNoteToDifferentTrack(Transform noteToMove)
        {
            Vector3 noteInitPos = noteToMove.GetComponent<Selectable>().GetInitialPosition();
            int initTrack = WhichTrack(noteInitPos.y);
            int targetTrack = WhichTrack(noteToMove.position.y);


            if (initTrack == targetTrack)
                return;


            if (!tracksList[initTrack].Remove(noteToMove.gameObject))
                Debug.LogWarning($"Failed removing {noteToMove.name} from track {initTrack}");

            tracksList[targetTrack].Add(noteToMove.gameObject);
        }

        private void SortTrack(int track)
        {
            tracksList[track].Sort((p, q) => 
                p.transform.position.x.CompareTo(q.transform.position.x));
        }

        public List<NoteObject> GetSortedNoteMap()
        {
            List<NoteObject> noteMap = new List<NoteObject>();
            
            for (int track = 0; track < 5; track++) 
            {
                SortTrack(track);

                foreach (GameObject noteObj in tracksList[track])
                {
                    float spawnTime = (noteObj.transform.position.x + 5f) / 7.5f;
                    int noteType = (int)noteObj.transform.GetComponent<Selectable>().GetNoteType();
                    noteMap.Add(new NoteObject(spawnTime, track, noteType));
                }
            }

            return noteMap.OrderBy(s => s.spawnTime).ThenBy(b => b.buttonIndex).ToList();
        }
    }
}
