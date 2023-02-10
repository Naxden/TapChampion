using System.Collections.Generic;
using UnityEngine;
using Saving.Note;
using System;
using UnityEngine.UIElements;
using Recording.Note;

namespace Recording.NoteRenderer
{
    using Track = List<GameObject>;

    public class NoteRenderer : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] notePrefabs = new GameObject[3];

        private float[] tracksPositions = {1.25f, 0f, -1.25f, -2.5f, -3.75f};

        private List<Track> trackList;
        private float songLength;

        // function called externally by Event
        public void CreateEmptyNotes(float songLength)
        {
            this.songLength = songLength;

            trackList = new List<Track>(5);

            for (int i = 0; i < 5; i++)
                trackList.Add(new Track(128)); // using some initial value as size of list
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

            Debug.Log(newPos);
            GameObject note = Instantiate(notePrefabs[(int)noteType], newPos, Quaternion.identity);
            note.transform.SetParent(transform, false);

            trackList[track].Add(note);
            
            if (noteType == NoteType.LongEnd) 
            {
                int indexOfLastNote = trackList[track].Count - 2;
                if (indexOfLastNote < 0)
                {
                    Debug.LogWarning("Adding connection, Begin-End notes, index less than zero");
                    return false;
                }
                
                LongNote longBegin = trackList[track][indexOfLastNote].transform.GetComponent<LongNote>();
                longBegin.SetOtherHalf(note);
                note.GetComponent<LongNote>().SetOtherHalf(longBegin.gameObject);
                longBegin.InitializePair();
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
                {
                    return true;
                }
            }

            return false;
        }

        // function called externally by Event
        public void ClearTracks()
        {
            if (trackList == null)
                return;

            Debug.Log("Clearing Tracks");

            for (int i = 0; i < trackList.Count; i++) 
            {
                foreach (var note in trackList[i])
                    Destroy(note);
                trackList[i].Clear();
            }
        }

        public void DeleteNote(Vector3 position) // TODO usuwanie nuty
        {
            position.x = Mathf.Ceil(position.x / 0.75f) * 0.75f - 0.5f; ;
            GameObject deleteNote = IsNoteThere(position);

            if (deleteNote == null)
                return;

            int track = WhichTrack(position.y);

            trackList[track].Remove(deleteNote);
            Destroy(deleteNote);
        }
    }
}
