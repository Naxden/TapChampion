using Recording.Note.Selectable;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Recording.NotesHandler
{
    public class NotesHandler : MonoBehaviour
    {
        [SerializeField]
        private GameObject NotesRenderer;

        private bool notesError = false;
        private bool firstSnap = true;

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

        public void AddChild(GameObject note)
        {
            if (note == null)
                return;

            if (note.transform.parent == transform)
                return;

            note.transform.parent = transform;
            note.GetComponent<Selectable>().Select();
        }

        public void RemoveChild(GameObject note)
        {
            if (note == null) 
                return;
            if (note.transform.parent != transform)
                return;

            note.GetComponent<Selectable>().Deselect();
            note.transform.parent = NotesRenderer.transform;
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

            if (Mathf.Abs(destination.x - transform.position.x) >= 0.75f)
            {
                newPosition.x = Mathf.Ceil((destination.x) / 0.75f) * 0.75f - 0.5f;
            }

            float childMinYPos = MinMoveRange();
            float childMaxYPos = MaxMoveRange();

            newPosition.y = Mathf.Round(destination.y / 1.25f) * 1.25f;

            if (childMinYPos == -3.75f && newPosition.y < transform.position.y ||
                childMaxYPos ==  1.25f && newPosition.y > transform.position.y)
                    newPosition.y = transform.position.y;

            transform.position = newPosition;
        }

        private float MinMoveRange()
        {
            float min = 1.25f;

            foreach (Transform child in transform)
                if (child.position.y < min)
                        min = child.position.y;

            return min;
        }

        private float MaxMoveRange()
        {
            float max = -3.75f;

            foreach (Transform child in transform)
                if (child.position.y > max)
                    max = child.position.y;

            return max;
        }

        public void NoteErrorOccurred()
        {
            notesError = true;
        }

        public void NoteErrorResolved()
        {
            notesError = false;
        }

        public bool IsEmpty()
        {
            return transform.childCount == 0;
        }

        public void MoveEnded()
        {
            if (notesError)
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

                child.GetComponent<Selectable>().Deselect();
                child.parent = NotesRenderer.transform;
            }

            transform.position = Vector3.zero;
            firstSnap = true;
        }
    }
}