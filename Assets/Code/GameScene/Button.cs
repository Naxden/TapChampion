using System.Collections.Generic;
using UnityEngine;
using GameScene.Notes;
using GameScene.Notes.NoteManager;

namespace GameScene.Player.Button
{
    public class Button : MonoBehaviour
    {
        [SerializeField]
        KeyCode keyCode;

        SpriteRenderer buttonSprite;
        
        Queue<Note> notesQueue = new Queue<Note>();
        Player player;
        NoteManager noteManager;

        bool isWaitingForEndNote = false;
        
        private void Awake()
        {
            buttonSprite = transform.GetComponent<SpriteRenderer>();
            player = GameObject.FindObjectOfType<Player>();
            noteManager = GameObject.FindObjectOfType<NoteManager>();
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
            Note target = collision.GetComponent<Note>();

            if (target.CompareTag("Note") && !notesQueue.Contains(target))
            {
                notesQueue.Enqueue(target);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Note target = collision.GetComponent<Note>();

            if (target.CompareTag("Note") && notesQueue.Contains(target))
            {
                isWaitingForEndNote = false;
                player.NoteWasMissed();
                noteManager.RetrieveNote(notesQueue.Dequeue(), 3f);
            }
        }

        private void HitNote()
        {
            Note note = notesQueue.Dequeue();
            float distance = Vector3.Distance(transform.position, note.transform.position);

            if (note.GetNoteType() == Note.NoteType.LongBegin)
            {
                player.NoteWasHit(HitInfo(distance));
                isWaitingForEndNote = true;
                noteManager.RetrieveNote(note);
            }
            else 
            {
                if (note.GetNoteType() == Note.NoteType.Short)
                {
                    player.NoteWasHit(HitInfo(distance));
                    isWaitingForEndNote = false;
                    noteManager.RetrieveNote(note);
                }
                else if (note.GetNoteType() == Note.NoteType.LongEnd && isWaitingForEndNote)
                {
                    player.NoteWasHit(HitInfo(distance));
                    isWaitingForEndNote = false;
                    noteManager.RetrieveNote(note);
                }
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