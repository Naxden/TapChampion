using UnityEngine;

namespace GameScene
{
    public class NotesRetriever : MonoBehaviour
    {
        [SerializeField]
        private NoteManager noteManager;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Note"))
            {
                NoteMB note = collision.GetComponent<NoteMB>();
                noteManager.RetrieveNote(note);
            }
        }
    }
}

