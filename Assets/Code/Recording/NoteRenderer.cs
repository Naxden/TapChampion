using System.Collections.Generic;
using UnityEngine;
using Saving.Note;
using System;
using UnityEngine.UIElements;


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

        public void AddNote(Vector3 notePos, NoteType noteType)
        {
            float xPos = notePos.x;
            float yPos = notePos.y;

            if (yPos < -4.25f || yPos > 1.5f || xPos < -5f || xPos > (songLength * 10) - 5f)
                return;

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

            xPos = Mathf.Round((float)xPos / 0.75f) * 0.75f - 0.5f;
            Vector3 newPos = new Vector3(xPos, tracksPositions[track]);

            if (IsNoteThere(newPos) != null)
                return;

            GameObject note = Instantiate(notePrefabs[(int)noteType], newPos, Quaternion.identity);
            note.transform.SetParent(transform, false);
            
            trackList[track].Add(note);    
        }

        public GameObject IsNoteThere(Vector3 pos)
        {
            RaycastHit2D ray = Physics2D.Raycast(pos, Vector2.zero, 500f);
            
            if (ray.collider == null || !ray.transform.CompareTag("Note"))
                return null;

            return ray.transform.gameObject;
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

        public void DeleteNote() // TODO usuwanie nuty
        {

        }
    }
}
