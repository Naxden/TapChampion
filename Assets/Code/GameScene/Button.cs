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
        Player player;
        
        private void Awake()
        {
            buttonSprite = transform.GetComponent<SpriteRenderer>();
            player = GameObject.FindObjectOfType<Player>();
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
                player.NoteWasMissed();
                Destroy(notesQueue.Dequeue().gameObject, 3f);
            }
        }

        private void HitNote()
        {
            Transform note = notesQueue.Dequeue();

            float distance = Vector3.Distance(transform.position, note.position);
            player.NoteWasHit(HitInfo(distance));

            Destroy(note.gameObject);
        }
        
        private string HitInfo(float distance)
        {
            if (distance <= 0.128f)
            {
                return "Perfect";
            }
            else if (distance <= 0.384f)
            {
                return "Very good";
            }
            else if (distance <= 0.768f)
            {
                return "Good";
            }
            else
            {
                return "Fine";
            }
        }
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, 1.28f);

            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, 0.768f);

            UnityEditor.Handles.color = Color.blue;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, 0.384f);

            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, 0.128f);

        }
        public int QueueLength()
        {
            return notesQueue.Count;
        }
    }
}