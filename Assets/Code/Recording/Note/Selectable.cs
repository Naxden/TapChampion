using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Recording.Note.Selectable
{
    public class Selectable : MonoBehaviour
    {
        public Color initialColor;
        private SpriteRenderer spriteRenderer;
        private Vector3 initialPosition;

        void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            initialColor = spriteRenderer.color;
            initialPosition = transform.position;
        }

        public void Move(Vector3 destination)
        {
            Vector3 newPostion = transform.position;
            if ( Mathf.Abs( destination.x - transform.position.x) >= 0.75f)
            {
                newPostion.x = Mathf.Round((destination.x) / 0.75f) * 0.75f - 0.5f;
            }

            transform.position = newPostion;
        }

        public void Select()
        {
            spriteRenderer.color = Color.blue;
        }

        public void Deselect()
        {
            spriteRenderer.color = initialColor;
        }
    }
}


