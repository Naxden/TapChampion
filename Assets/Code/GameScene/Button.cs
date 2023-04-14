using System.Collections.Generic;
using UnityEngine;
using Saving;
using TMPro;

namespace GameScene
{
    public class Button : MonoBehaviour
    {
        [SerializeField]
        KeyCode keyCode;

        [SerializeField]
        private TextMeshProUGUI keyTag;
        SpriteRenderer buttonSprite;
        
        [SerializeField]
        Player player;

        [SerializeField]
        NoteManager noteManager;

        Queue<NoteMB> notesQueue = new Queue<NoteMB>();
        AudioSource audioSource;


        bool isWaitingForEndNote = false;
        
        public void SetKey(KeyCode keyCode)
        {
            this.keyCode = keyCode;
            keyTag.text = keyCode.ToString();
        }

        public void ClearNotesQueue()
        {
            notesQueue.Clear();
        }

        private void Awake()
        {
            buttonSprite = GetComponent<SpriteRenderer>();

            audioSource = GetComponent<AudioSource>();
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

                if (isWaitingForEndNote)
                {
                    if (notesQueue.Count > 0)
                        HitNote();
                    else
                    {
                        isWaitingForEndNote = false;
                        player.NoteWasMissed();
                    }
                }
            }

            if (isWaitingForEndNote)
                player.LongNoteBeingHit();

        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            NoteMB target = collision.GetComponent<NoteMB>();

            if (target.CompareTag("Note") && !notesQueue.Contains(target))
            {
                notesQueue.Enqueue(target);
                audioSource.PlayOneShot(audioSource.clip);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            NoteMB target = collision.GetComponent<NoteMB>();

            if (target.CompareTag("Note") && notesQueue.Contains(target))
            {
                isWaitingForEndNote = false;
                player.NoteWasMissed();
                notesQueue.Dequeue();
            }
        }

        private void HitNote()
        {
            NoteMB note = notesQueue.Dequeue();
            float distance = Vector3.Distance(transform.position, note.transform.position);

            player.NoteWasHit(HitInfo(distance));
            noteManager.RetrieveNote(note);
            isWaitingForEndNote = note.GetNoteType() == NoteType.LongBegin;
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

        //private void OnDrawGizmos()
        //{
        //    UnityEditor.Handles.color = Color.green;
        //    UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, 1.28f);

        //    UnityEditor.Handles.color = Color.yellow;
        //    UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, 0.768f);

        //    UnityEditor.Handles.color = Color.blue;
        //    UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, 0.384f);

        //    UnityEditor.Handles.color = Color.red;
        //    UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, 0.128f);

        //}

        public int QueueLength()
        {
            return notesQueue.Count;
        }
    }
}