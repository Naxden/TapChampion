using UnityEngine;
using System.Collections.Generic;
using Saving;

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

        private const float precision = 0.09f;

        private bool spawn = false;
        [SerializeField, Range(0.5f, 3f)]
        private float noteTimeToArrive = 2.5f;
        private float[] notesVelocity = new float[5];

        [SerializeField]
        private GameObject connectingLinePrefab;
        private NoteMB leftConnectingNote = null;
        private List<ConnectingLine> sendedConnectingLines = new List<ConnectingLine>();

        [SerializeField]
        private GameObject longNoteConnectorPrefab;
        private NoteMB[] sendedLongBeginNotes = new NoteMB[5];

        float songTimer;
        private bool songIsPlaying = false;
        int noteIndex = 0;

        List<NoteObject> notesMap;
        float userLag = 0f;
        List<NoteMB> sendedNotes = new List<NoteMB>();

        void Awake()
        {
            FillQueue();

            for (int i = 0; i < 5; i++)
            {
                float distance = Vector3.Distance(noteStartingPositions[i], buttons[i].transform.position);
                notesVelocity[i] = distance / noteTimeToArrive;
            }
        }

        public void Intialize(List<NoteObject> noteObjects)
        {
            ResetSong();
            notesMap = noteObjects;
        }

        public void SetUserLag(float userLag)
        {
            this.userLag = userLag;
        }

        public void SetTimer(float time)
        {
            songTimer = time;
        }

        public void SongIsPlaying(bool isPlaing)
        {
            songIsPlaying = isPlaing;
        }

        public void SpawnNotes(bool spawn)
        {
            this.spawn = spawn;
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
            note.NoteRemoved();
        }

        public void ResetSong()
        {
            if (songIsPlaying)
            {
                Debug.LogWarning("NoteManager.ResetSong(): song is playing");
                return;
            }

            spawn = false;

            for (int i = sendedNotes.Count - 1; i >= 0; i--)
            {
                RetrieveNote(sendedNotes[i]);
            }

            noteIndex = 0;

            leftConnectingNote = null;
        }

        //int breakIndex = 0;

        void Update()
        {
            //if (breakIndex < notesMap.Count && songTimer >= (notesMap[breakIndex].spawnTime))
            //{
            //    breakIndex++;
            //    Debug.Break();
            //}
            if (spawn)
            {
                if (noteIndex >= notesMap.Count)
                {
                    Debug.Log("Load notes, index out of bound");
                    spawn = false;
                }
                else
                {
                    while (noteIndex < notesMap.Count && 
                           songTimer + noteTimeToArrive >= (notesMap[noteIndex].spawnTime + userLag))
                    {
                        SendNote();
                        CheckConnectingLine();
                        noteIndex++;
                    }
                }
            }
            
            if (songIsPlaying)
                MoveNotes();
        }

        private void SendNote()
        {
            NoteMB note = notesPool.Dequeue();

            int buttonIndex = notesMap[noteIndex].buttonIndex;
            NoteType noteType = (NoteType)notesMap[noteIndex].noteType;

            note.Initialize(noteStartingPositions[buttonIndex],
                            notesVelocity[buttonIndex],
                            noteType,
                            buttonIndex,
                            buttons[buttonIndex]);

            note.UpdateVisuals(notePrefabs[(int)noteType]);

            note.gameObject.SetActive(true);

            if (noteType != NoteType.Short)
                SendLongNote(note, buttonIndex);

            sendedNotes.Add(note);
        }

        private void SendLongNote(NoteMB longNote, int buttonIndex)
        {
            if (longNote == null)
            {
                Debug.LogError("SendLongNote: longNote argument is null");
                return;
            }

            if (longNote.GetNoteType() == NoteType.LongBegin)
            {
                LongNoteConnector connector = Instantiate(longNoteConnectorPrefab).GetComponent<LongNoteConnector>();

                longNote.SetNoteConnector(connector);

                connector.SetBegin(noteStartingPositions[buttonIndex]);
                connector.SetEnd(noteStartingPositions[buttonIndex]);
                connector.EnableLine();

                sendedLongBeginNotes[buttonIndex] = longNote;
            }
            else
            {
                NoteMB beginNote = sendedLongBeginNotes[buttonIndex];

                if (beginNote == null)
                {
                    Debug.LogError("SendLongNote: longNoteBegin is null");
                    return;
                }

                LongNoteConnector noteConnector = beginNote.GetNoteConnector();

                longNote.SetNoteConnector(noteConnector);

                sendedLongBeginNotes[buttonIndex] = null;
            }
        }

        private void CheckConnectingLine()
        {
            if (noteIndex == notesMap.Count - 1)
            {
                if (leftConnectingNote != null)
                    SpawnConnectingLine();
                return;
            }

            float currentNoteSpawnTime = notesMap[noteIndex].spawnTime;
            float nextNoteSpawnTime = notesMap[noteIndex + 1].spawnTime;

            if (nextNoteSpawnTime - currentNoteSpawnTime <= precision)
            {
                if (leftConnectingNote == null)
                {
                    leftConnectingNote = sendedNotes[^1];
                }
            }
            else
                if (leftConnectingNote != null)
                    SpawnConnectingLine();
        }

        private void SpawnConnectingLine()
        {
            ConnectingLine connectingLine = Instantiate(connectingLinePrefab).GetComponent<ConnectingLine>();

            NoteMB rightConnectingNote = sendedNotes[^1];

            int beginIndex = sendedNotes.IndexOf(leftConnectingNote);
            int endIndex = sendedNotes.IndexOf(rightConnectingNote, beginIndex + 1);

            for (int index = beginIndex; index <= endIndex; index++)
            {
                sendedNotes[index].SetConnectingLine(connectingLine);
            }

            int beginNoteButton = leftConnectingNote.GetButtonIndex();
            int endNoteButton = rightConnectingNote.GetButtonIndex();

            connectingLine.Initialize(beginNoteButton, endNoteButton, this);
            sendedConnectingLines.Add(connectingLine);

            leftConnectingNote = null;
        }

        public void DestroyConnectingLine(ConnectingLine connectingLine)
        {
            if (sendedConnectingLines.Count <= 0)
                return;

            if (!sendedConnectingLines.Contains(connectingLine))
                return;

            sendedConnectingLines.Remove(connectingLine);
            Destroy(connectingLine.gameObject);
        }

        private void MoveNotes()
        {
            foreach (NoteMB note in sendedNotes)
            {
                note.Move();
            }

            for (int i = 0; i < sendedConnectingLines.Count; i++)
            {
                sendedConnectingLines[i].Move();
            }
        }
    }
}