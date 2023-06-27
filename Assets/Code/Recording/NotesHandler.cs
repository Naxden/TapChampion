using System.Collections.Generic;
using UnityEngine;
using Recording.Note;
using System;

namespace Recording
{
    public class NotesHandler : MonoBehaviour
    {
        [SerializeField]
        private NoteRenderer noteRenderer;

        [SerializeField]
        private List<LongNote> longNotes;

        private bool firstSnap = true;

        private const float minX = -5f;
        private float maxX = 0f;

        public void SetStartPosition(Vector3 position)
        {
            
            position.x = Mathf.Ceil((position.x) / 0.75f) * 0.75f - 0.5f;
            position.y = Mathf.Round(position.y / 1.25f) * 1.25f;
            position.z = 0f;

            if (!IsEmpty())
            {
                Vector3 move = transform.position - position;
                foreach (Transform child in transform)
                {
                    child.localPosition += move;
                }
            }
            transform.position = position;
        }

        //Function called by OnSongLoadEvent
        public void SetSongLength(float  length)
        {
            maxX = length * 7.5f - 5.375f + 0.375f;
            maxX = Mathf.Ceil((maxX) / 0.75f) * 0.75f - 0.5f;
        }

        public void AddChild(GameObject note)
        {
            if (note == null)
                return;

            if (note.transform.parent == transform)
                return;

            note.transform.parent = transform;
            note.GetComponent<Selectable>().Select();

            LongNote longNote = note.GetComponent<LongNote>();
            if (longNote != null)
                longNotes.Add(longNote);
        }

        public void RemoveChild(GameObject note)
        {
            if (note == null) 
                return;
            if (note.transform.parent != transform)
                return;

            note.GetComponent<Selectable>().Deselect();
            note.transform.parent = noteRenderer.transform;

            LongNote longNote = note.transform.GetComponent<LongNote>();
            if (longNote != null)
                longNotes.Remove(longNote);
        }

        public bool IsChild(GameObject note)
        {
            if (note == null)
                return false;
            
            foreach (Transform child in transform)
            {
                if (child.transform == note.transform)
                    return true;
            }

            return false;
        }

        public void Move(Vector3 destination)
        {
            if (firstSnap)
            {
                firstSnap = false;
                return;
            }

            Vector3 newPosition = transform.position;

            Tuple<float, float> xRange = XRange();

            if (Mathf.Abs(destination.x - transform.position.x) >= 0.75f && //MoveX if movement is >= than step
                 (destination.x < transform.position.x && xRange.Item1 > minX ||  // Move Left if you can move left
                 destination.x > transform.position.x && xRange.Item2 < maxX)) // Move Right if you can move right
            {
                newPosition.x = Mathf.Ceil((destination.x) / 0.75f) * 0.75f - 0.5f;
            }

            Tuple<float, float> yRange = YRange();
            newPosition.y = Mathf.Round(destination.y / 1.25f) * 1.25f;

            if (yRange.Item1 == -3.75f && newPosition.y < transform.position.y ||
                yRange.Item2 ==  1.25f && newPosition.y > transform.position.y)
            {
                newPosition.y = transform.position.y;
            }

 
            transform.position = newPosition;
            UpdateLongNotes();        
        }

        private Tuple<float, float> XRange()
        {
            float min = float.MaxValue;
            float max = float.MinValue;

            foreach (Transform child in transform)
            {
                if (child.position.x < min)
                    min = child.position.x;
                if (child.position.x > max)
                    max = child.position.x;
            }

            return Tuple.Create(min, max);
        }

        private Tuple<float, float> YRange()
        {
            float min = 1.25f;
            float max = -3.75f;

            foreach (Transform child in transform)
            {
                if (child.position.y < min)
                    min = child.position.y;
                if (child.position.y >  max)
                    max = child.position.y;
            }

            return Tuple.Create(min, max);
        }

        private void UpdateLongNotes()
        {
            foreach (LongNote longNote in longNotes)
            {
                longNote.MoveLongNote();
            }
        }

        private bool DoesNotesFit()
        {
            foreach (Transform child in transform)
            {
                if (!child.GetComponent<Selectable>().DoesFit())
                    return false;
            }

            return true;
        }

        public bool IsEmpty()
        {
            return transform.childCount == 0;
        }

        public void MoveEnded()
        {
            if (!DoesNotesFit())
            {
                foreach (Transform child in transform)
                {
                    child.GetComponent<Selectable>().ResetPosition();
                }
            }
        }

        public void Deselect()
        {
            for (int childCount = transform.childCount; childCount > 0; childCount--)
            {
                Transform child = transform.GetChild(0);

                noteRenderer.MoveNoteToDifferentTrack(child);

                child.GetComponent<Selectable>().Deselect();
                child.parent = noteRenderer.transform;
            }

            longNotes.Clear();

            transform.position = Vector3.zero;
            firstSnap = true;
        }

        public void Delete()
        {
            foreach (var longNote in longNotes)
            {
                GameObject otherHalf = longNote.GetOtherHalf();

                if (!IsChild(otherHalf))
                    otherHalf.transform.parent = transform;
            }

            longNotes.Clear();

            foreach (Transform child in transform)
            {
                noteRenderer.DeleteNote(child.transform.position);
            }

            transform.position = Vector3.zero;
            firstSnap = true;
        }
    }
}