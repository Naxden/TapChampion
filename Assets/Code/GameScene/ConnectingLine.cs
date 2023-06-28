using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class ConnectingLine : MonoBehaviour
    {
        [SerializeField]
        private Vector3 startPos;
        [SerializeField]
        private Vector3 endPos;

        [SerializeField]
        private Vector3 startScale;
        [SerializeField]
        private Vector3 endScale;

        [SerializeField, Range(1f, 1.2f)]
        private float lerpError = 1.029f;

        private NoteManager noteManager;

        private List<NoteMB> notes = new List<NoteMB>(5);
        private int leftEnd;
        private int rightEnd;

        public void AddNote(NoteMB note)
        {
            notes.Add(note);
        }

        public void Initialize(int leftEnd, int rightEnd, NoteManager noteManager)
        {
            this.noteManager = noteManager;
            this.leftEnd = leftEnd;
            this.rightEnd = rightEnd;

            Resize();
        }

        private void Resize()
        {
            Renderer myRenderer = GetComponent<Renderer>();

            myRenderer.material.SetFloat("_Begin", leftEnd / 4f);
            myRenderer.material.SetFloat("_End", rightEnd / 4f);
        }

        public void Move()
        {
            float lerp = notes[^1].GetTravelStatus();

            transform.localScale = Vector3.LerpUnclamped(startScale, endScale, lerp * lerpError);
            transform.position = Vector3.LerpUnclamped(startPos, endPos, lerp * lerpError);
        }

        public void NoteRemoved(int buttonIndex)
        {

            if (notes.Count <= 2)
            {
                Debug.Log("Niszcze sie");
                noteManager.DestroyConnectingLine(this);
            }
            else
            {
                if (buttonIndex == leftEnd)
                {
                    notes.RemoveAt(0);
                    leftEnd = notes[0].GetButtonIndex();
                }
                else if (buttonIndex == rightEnd)
                {
                    notes.RemoveAt(notes.Count - 1);
                    rightEnd = notes[^1].GetButtonIndex();
                }

                Resize();
            }
        }
    }
}
