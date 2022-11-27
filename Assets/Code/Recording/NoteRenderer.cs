using System.Collections.Generic;
using UnityEngine;
using Saving.Note;

namespace Recording.NoteRenderer
{
    public class NoteRenderer : MonoBehaviour
    {
        [SerializeField]
        private GameObject noteShortPrefab;
        
        [SerializeField]
        private GameObject noteLongBegPrefab;

        [SerializeField]
        private GameObject noteLongEndPrefab;

        private float[] tracksPositions = {1.25f, 0f, -1.25f, -2.5f, -3.75f};
        private GameObject[,] notesArrays = new GameObject[5, 1200];
        private Vector3 offScreenPos = new Vector3(-2000, 0, 0);

        
        private void Start() 
        {
            FillTracks();
        }

        private void FillTracks()
        {
            for (int track = 0; track < 5; track++)
            {
                for (int i = 0; i < 1200; i++)
                {
                    offScreenPos.y = tracksPositions[track];
                    GameObject note = Instantiate(noteShortPrefab, offScreenPos, Quaternion.identity);
                    note.gameObject.SetActive(false);
                    note.name = $"{track} Note {i}";
                    note.transform.SetParent(transform, false);
                    notesArrays[track, i] = note;
                }
            }
        }
        

        public void AddNote(Vector3 position, NoteType noteType)
        {
            int xPos = (int)Mathf.Round(position.x);
            float yPos = position.y;

            if (yPos < -4.25f || yPos > 1.5f || xPos < -5 || xPos > 1194)
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

            if (notesArrays[track, xPos + 5].activeSelf)
                return;

            SetNote(track, xPos, noteType);
        }

        void SetNote(int track, int xPos, NoteType noteType)
        {
            GameObject note = notesArrays[track, xPos + 5];
            
            note.transform.position = new Vector3(xPos, tracksPositions[track], 0);
            
            if (noteType == NoteType.Short)
                NoteUpdateVisuals(note, noteShortPrefab);
            if (noteType == NoteType.LongBegin)
                NoteUpdateVisuals(note, noteLongBegPrefab);
            if (noteType == NoteType.LongEnd)
                NoteUpdateVisuals(note, noteLongEndPrefab);
            
            note.SetActive(true);
        }

        void NoteUpdateVisuals(GameObject note, GameObject prefab)
        {
            var noteSR = note.GetComponent<SpriteRenderer>();
            var prefabSR = prefab.GetComponent<SpriteRenderer>();
            noteSR.sprite = prefabSR.sprite;
            noteSR.color = prefabSR.color;
        }

    }

}
