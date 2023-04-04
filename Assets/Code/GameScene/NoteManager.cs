using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Saving;
using System.Runtime.InteropServices;

namespace GameScene
{
    public class NoteManager : MonoBehaviour
    {
        #region NoteInit

        [SerializeField]
        GameObject[] notePrefabs;

        [SerializeField]
        Button[] buttons;

        [SerializeField]
        Vector3[] noteStartingPositions;

        Queue<NoteMB> notesPool = new Queue<NoteMB>(100);
        #endregion

        bool spawn = false;
        const float NOTE_TRAVEL_DISTANCE = 4.75f;
        const float NOTE_DELAY_TO_ARRIVE = NOTE_TRAVEL_DISTANCE / 3.5f;
        private const float TIMING_ERROR = -0.06f;


        float songTimer;
        private bool songIsPlaying = false;
        int noteIndex = 0;

        List<NoteObject> notesMap;
        float userLag = 0f;
        List<NoteMB> sendedNotes = new List<NoteMB>();

        void Awake()
        {
            FillQueue();
        }

        public void Intialize(List<NoteObject> noteObjects)
        {
            ResetSong();
            notesMap = noteObjects;
        }

        public void SetUserLag(float userLag)
        {

            this.userLag = userLag + TIMING_ERROR;
        }

        public void SetTimer(float time)
        {
            songTimer = time;
        }

        private void SendNote()
        {
            NoteMB note = notesPool.Dequeue();

            NoteType noteType = (NoteType)notesMap[noteIndex].noteType;
            
            note.Initialize(noteStartingPositions[notesMap[noteIndex].buttonIndex],
                            noteType,
                            buttons[notesMap[noteIndex].buttonIndex]);

            note.UpdateVisuals(notePrefabs[(int)noteType]);

            note.gameObject.SetActive(true);

            sendedNotes.Add(note);
        }

        private void FillQueue()
        {
            for (int i = 0; i < 100; i++)
            {
                NoteMB note = Instantiate(notePrefabs[0], transform.position, Quaternion.identity).GetComponent<NoteMB>();
                note.gameObject.SetActive(false);
                note.name = $"Note {i}";
                note.transform.SetParent(transform, false);
                notesPool.Enqueue(note);
            }
        }

        public void RetrieveNote(NoteMB note)
        {
            note.gameObject.SetActive(false);
            note.transform.position = transform.position;
            notesPool.Enqueue(note);
            sendedNotes.Remove(note);
        }

        int breakIndex = 0;

        void Update()
        {
            if (breakIndex < notesMap.Count && songTimer >= (notesMap[breakIndex].spawnTime))
            {
                breakIndex++;
                Debug.Break();
            }
            if (spawn)
            {
                if (noteIndex >= notesMap.Count)
                {
                    Debug.LogWarning("Load notes, index out of bound");
                    spawn = false;
                }
                else
                {
                    while (noteIndex < notesMap.Count && 
                           songTimer + NOTE_DELAY_TO_ARRIVE >= (notesMap[noteIndex].spawnTime + userLag))
                    {
                        SendNote();
                        noteIndex++;
                    }
                }
            }
            
            if (songIsPlaying)
                MoveNotes();
        }

        public void SongIsPlaying(bool isPlaing)
        {
            songIsPlaying = isPlaing;
        }

        public void SpawnNotes(bool spawn)
        {
            this.spawn = spawn;
        }

        private void MoveNotes()
        {
            foreach (NoteMB note in sendedNotes)
            {
                note.Move();
            }
        }

        public void ResetSong()
        {
            if (songIsPlaying)
            {
                Debug.LogWarning("NoteManager.ResetSong(): song is playing");
                return;
            }

            spawn = false;

            for (int i = sendedNotes.Count - 1; i >= 0 ; i--)
            {
                RetrieveNote(sendedNotes[i]);
            }

            noteIndex = 0;
        }
    }
}