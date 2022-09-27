using UnityEngine;
using GameScene.Player.Button;
using System.Collections;
using System.Collections.Generic;

namespace GameScene.Notes.NoteManager
{
    public class NoteManager : MonoBehaviour
    {
        [SerializeField]
        GameObject notePrefab;

        [SerializeField]
        Button[] buttons;

        [SerializeField]
        Vector3[] noteStartingPositions;

        bool spawn = true;
        Queue<Note> notesQueue = new Queue<Note>(100);

        void Start()
        {
            FillQueue();

            StartCoroutine(sendNotesRoutine());
        }

        private IEnumerator sendNotesRoutine()
        {
            while (spawn)
            {
                yield return new WaitForSeconds(Random.Range(1.5f, 2f));

                SendNote();
            }
        }
        private void SendNote()
        {
            int buildIndex = Random.Range(0, 5);

            Note note = notesQueue.Dequeue();
            note.Initialize(
                            noteStartingPositions[buildIndex] ,
                            (Note.NoteType)0, 
                            buttons[buildIndex], 
                            2f);
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
            if (Input.GetKeyUp(KeyCode.Space))
                spawn = false;

            if (Input.GetKeyUp(KeyCode.Backspace))
                ClearAllNotes();
        }

        void ClearAllNotes()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}

