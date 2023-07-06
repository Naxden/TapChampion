using System.Collections.Generic;
using UnityEngine;
using Saving;
using TMPro;
using System.Collections;

namespace GameScene
{
    public class Button : MonoBehaviour
    {
        [SerializeField]
        private KeyCode keyCode;

        [SerializeField]
        private TextMeshProUGUI keyTag;
        private SpriteRenderer buttonSprite;
        
        [SerializeField]
        private Player player;

        [SerializeField]
        private NoteManager noteManager;

        private Queue<NoteMB> notesQueue = new Queue<NoteMB>();
        private AudioSource audioSource;

        private bool isWaitingForEndNote = false;
        
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
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            NoteMB target = collision.GetComponent<NoteMB>();

            if (target.CompareTag("Note") && !notesQueue.Contains(target))
            {
                notesQueue.Enqueue(target);
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

            audioSource.PlayOneShot(audioSource.clip);

            player.NoteWasHit(HitInfo(distance));
            noteManager.RetrieveNote(note);
            isWaitingForEndNote = note.GetNoteType() == NoteType.LongBegin;
            StartCoroutine(LongNoteRoutine());
        }
        
        private IEnumerator LongNoteRoutine()
        {
            while (isWaitingForEndNote)
            {
                player.LongNoteBeingHit();
                yield return new WaitForSeconds(0.4f);
            }
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

        public int QueueLength()
        {
            return notesQueue.Count;
        }
    }
}