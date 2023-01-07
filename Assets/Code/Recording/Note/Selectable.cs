using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Recording.Note.Selectable
{
    public class Selectable : MonoBehaviour
    {
        public Color initialColor;
        private SpriteRenderer spriteRenderer;
        private Vector3 initialPosition;
        private Rigidbody2D myRigidbody2D;

        private NotesHandler.NotesHandler notesHandler;
        private bool isSelected = false;

        private void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            initialColor = spriteRenderer.color;
            initialPosition = transform.position;

            notesHandler = FindObjectOfType<NotesHandler.NotesHandler>();
        }

        public void Select()
        {
            if (myRigidbody2D == null) 
                myRigidbody2D = transform.AddComponent<Rigidbody2D>();
            myRigidbody2D.isKinematic = true;
            spriteRenderer.color = Color.blue;
            spriteRenderer.sortingOrder = 5;
            initialPosition = transform.position;
            isSelected = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision == null) 
                return;

            if (isSelected && collision.transform.CompareTag("Note"))
            {
                spriteRenderer.color = Color.red;

                notesHandler.NoteErrorOccurred();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (isSelected)
            {
                spriteRenderer.color = Color.blue;

                notesHandler.NoteErrorResolved();
            }
        }

        public void ResetPosition()
        {
            transform.position = initialPosition;
            spriteRenderer.color = Color.blue;
        }

        public void Deselect()
        {
            spriteRenderer.color = initialColor;
            spriteRenderer.sortingOrder = 0;
            isSelected = false;
            Destroy(myRigidbody2D);
        }
    }
}


