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

        float songTimer;
        private bool songIsPlaying = false;
        int noteIndex = 0;

        List<NoteObject> notesMap;
        float timeCalibration = 0f;
        List<NoteMB> sendedNotes = new List<NoteMB>();

        void Awake()
        {
            FillQueue();
        }

        public void Intialize(List<NoteObject> noteObjects, float timeCalibration)
        {
            this.timeCalibration = timeCalibration; 
            notesMap = noteObjects;
            spawn = true;
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

        public void RetrieveNote(NoteMB note, float timeOffset = 0f)
        {
            StartCoroutine(RetrieveNoteRoutine(note, timeOffset));
        }

        private IEnumerator RetrieveNoteRoutine(NoteMB note, float timeOffset)
        {
            yield return new WaitForSeconds(timeOffset);

            note.gameObject.SetActive(false);
            note.transform.position = transform.position;
            notesPool.Enqueue(note);
            sendedNotes.Remove(note);
        }

        //int breakIndex = 0;

        void Update()
        {
            //if (breakIndex < loadedNotes.Count && songTimer >= (loadedNotes[breakIndex].spawnTime))
            //{
            //    breakIndex++;
            //    Debug.Break();
            //}
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
                           songTimer + NOTE_DELAY_TO_ARRIVE >= (notesMap[noteIndex].spawnTime + timeCalibration))
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

        private void MoveNotes()
        {
            foreach (NoteMB note in sendedNotes)
            {
                note.Move();
            }
        }

        void ClearAllNotes()
        {
            foreach (Transform child in transform)
            {
                RetrieveNote(child.GetComponent<NoteMB>());
            }
        }

    }
}