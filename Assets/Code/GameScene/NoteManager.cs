using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Saving;

namespace GameScene
{
    public class NoteManager : MonoBehaviour
    {
        #region NoteInit
        [SerializeField]
        GameObject noteShortPrefab;
        [SerializeField]
        GameObject noteLongBegPrefab;
        [SerializeField]
        GameObject noteLongEndPrefab;

        [SerializeField]
        Button[] buttons;

        [SerializeField]
        Vector3[] noteStartingPositions;

        Queue<NoteMB> notesQueue = new Queue<NoteMB>(100);
        #endregion

        bool spawn = false;
        const float NOTE_TRAVEL_DISTANCE = 4.75f;

        float songTimer = -3f;
        int noteIndex = 0;

        List<NoteObject> loadedNotes;
        float timeCalibration = 0f;

        UIController uiController;

        void Start()
        {
            uiController = FindObjectOfType<UIController>();
            FillQueue();
        }

        public void Intialize(List<NoteObject> noteObjects, float timeCalibration)
        {
            this.timeCalibration = timeCalibration; 
            loadedNotes = noteObjects;
            spawn = true;
        }

        private void SendNote()
        {
            NoteMB note = notesQueue.Dequeue();

            NoteType noteType = (NoteType)loadedNotes[noteIndex].noteType;
            
            note.Initialize(noteStartingPositions[loadedNotes[noteIndex].buttonIndex],
                            noteType,
                            buttons[loadedNotes[noteIndex].buttonIndex]);
            
            if (noteType == NoteType.Short)
                note.UpdateVisuals(noteShortPrefab);
            else if (noteType == NoteType.LongBegin)
                note.UpdateVisuals(noteLongBegPrefab);
            else
                note.UpdateVisuals(noteLongEndPrefab);

            note.gameObject.SetActive(true);
        }

        private void FillQueue()
        {
            for (int i = 0; i < 100; i++)
            {
                NoteMB note = Instantiate(noteShortPrefab, transform.position, Quaternion.identity).GetComponent<NoteMB>();
                note.gameObject.SetActive(false);
                note.name = $"Note {i}";
                note.transform.SetParent(transform, false);
                notesQueue.Enqueue(note);
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
            notesQueue.Enqueue(note);
        }

        void Update()
        {
            if (spawn)
            {
                songTimer += Time.deltaTime;

                if (noteIndex >= loadedNotes.Count)
                {
                    Debug.LogWarning("Load notes, index out of bound");
                    spawn = false;
                }
                else
                {
                    while (noteIndex < loadedNotes.Count && 
                           songTimer >= (loadedNotes[noteIndex].spawnTime + timeCalibration))
                    {
                        SendNote();
                        noteIndex++;
                    }
                }
                uiController.UpdateTime(songTimer);
            }

            if (Input.GetKeyUp(KeyCode.Space))
                spawn = false;

            if (Input.GetKeyUp(KeyCode.Backspace))
                ClearAllNotes();

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