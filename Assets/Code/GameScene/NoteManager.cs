using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameScene.Player.Button;
using Saving.Note;
using GameScene.UI.UIController;

namespace GameScene.Notes.NoteManager
{
    public class NoteManager : MonoBehaviour
    {
        #region NoteInit
        [SerializeField]
        GameObject notePrefab;

        [SerializeField]
        Button[] buttons;

        [SerializeField]
        Vector3[] noteStartingPositions;

        Queue<Note> notesQueue = new Queue<Note>(100);
        #endregion

        bool spawn = false;
        const float NOTE_TRAVEL_DISTANCE = 4.75f;

        float songTimer = -3f;
        int noteIndex = 0;

        List<NoteObject> loadedNotes;

        UIController uiController;

        void Start()
        {
            uiController = FindObjectOfType<UIController>();
            FillQueue();
        }

        public void Intialize(List<NoteObject> noteObjects)
        {
            loadedNotes = noteObjects;
            spawn = true;
        }

        private void SendNote()
        {
            Note note = notesQueue.Dequeue();
            note.Initialize(noteStartingPositions[loadedNotes[noteIndex].buttonIndex],
                            (Note.NoteType)loadedNotes[noteIndex].noteType,
                            buttons[loadedNotes[noteIndex].buttonIndex]);

            note.gameObject.SetActive(true);
        }

        private void FillQueue()
        {
            for (int i = 0; i < 100; i++)
            {
                Note note = Instantiate(notePrefab, transform.position, Quaternion.identity).GetComponent<Note>();
                note.gameObject.SetActive(false);
                note.name = $"Note {i}";
                note.transform.SetParent(transform, false);
                notesQueue.Enqueue(note);
            }
        }

        public void RetrieveNote(Note note, float timeOffset = 0f)
        {
            StartCoroutine(RetrieveNoteRoutine(note, timeOffset));
        }

        private IEnumerator RetrieveNoteRoutine(Note note, float timeOffset)
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
                    while (noteIndex < loadedNotes.Count && songTimer >= loadedNotes[noteIndex].spawnTime )
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
                RetrieveNote(child.GetComponent<Note>());
            }
        }

    }
}