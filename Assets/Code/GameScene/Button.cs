using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Player.Button
{
    public class Button : MonoBehaviour
    {
        [SerializeField]
        KeyCode keyCode;

        SpriteRenderer buttonSprite;
        
        Queue<Transform> notesQueue = new Queue<Transform>();
        private void Awake()
        {
            buttonSprite = transform.GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(keyCode))
            {
                buttonSprite.color = Color.white;

                if (notesQueue.Count > 0)
                    HitNote();
            }

            if (Input.GetKeyUp(keyCode))
            {
                buttonSprite.color = new Color(0.9f, 0.8f, 0.2f, 0.5f);
            }

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Transform target = collision.transform;

            if (target.CompareTag("Note") && !notesQueue.Contains(target))
            {
                notesQueue.Enqueue(target);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Transform target = collision.transform;

            if (target.CompareTag("Note") && notesQueue.Contains(target))
            {
                Destroy(notesQueue.Dequeue().gameObject, 3f);
            }
        }

        private void HitNote()
        {
            Transform note = notesQueue.Dequeue();

            float distance = Vector3.Distance(transform.position, note.position);
            if (distance <= 0.128f)
            {
                Debug.Log("Perfect hit!");
            }
            else if (distance <= 0.384f)
            {
                Debug.Log("Very good hit");
            }
            else if (distance <= 0.768f)
            {
                Debug.Log("Good hit");
            }
            else
            {
                Debug.Log("Fine hit");
            }
            Destroy(note.gameObject);
        }
        
        public int QueueLength()
        {
            return notesQueue.Count;
        }
    }
}