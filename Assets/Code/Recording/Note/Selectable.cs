using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Recording.Note
{
    public class Selectable : MonoBehaviour
    {
        public Color initialColor;
        private SpriteRenderer mySpriteRenderer;
        private Vector3 initialPosition;
        private Rigidbody2D myRigidbody2D;

        private bool isSelected = false;
        private bool isColliding = false;
        private bool longNoteError = false;

        private void OnEnable()
        {
            mySpriteRenderer = GetComponent<SpriteRenderer>();
            initialColor = mySpriteRenderer.color;
            initialPosition = transform.position;

        }

        public void Select()
        {
            if (myRigidbody2D == null) 
                myRigidbody2D = transform.AddComponent<Rigidbody2D>();
            myRigidbody2D.isKinematic = true;
            mySpriteRenderer.color = Color.blue;
            mySpriteRenderer.sortingOrder = 5;
            initialPosition = transform.position;
            isSelected = true;
        }


        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision == null) 
                return;

            if (isSelected && collision.transform.CompareTag("Note"))
            {
                isColliding = true;

                UpdateVisuals();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (isSelected)
            {
                isColliding = false;

                UpdateVisuals();
            }
        }

        public void SetLongNoteError(bool condition)
        {
            longNoteError = condition;
        }

        public bool DoesFit()
        {
            return !isColliding && !longNoteError;
        }

        public void UpdateVisuals()
        {
            if (DoesFit())
                mySpriteRenderer.color = Color.blue;
            else
                mySpriteRenderer.color = Color.red;
        }

        public Vector3 GetInitialPosition()
        {
            return initialPosition;
        }

        public void ResetPosition()
        {
            transform.position = initialPosition;
            isColliding = false;
            if (longNoteError)
            {
                longNoteError = false;
                GetComponent<LongNote>().MoveLongNote();
            }
            UpdateVisuals();
        }

        public void Deselect()
        {
            mySpriteRenderer.color = initialColor;
            mySpriteRenderer.sortingOrder = 0;
            isSelected = false;
            isColliding = false;
            longNoteError = false;
            Destroy(myRigidbody2D);
        }
    }
}


