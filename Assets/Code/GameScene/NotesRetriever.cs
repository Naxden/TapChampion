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
                var note = collision.GetComponent<NoteMB>();
                noteManager.RetrieveNote(note);
            }
            else if (collision.CompareTag("ConnectingLine"))
            {
                var connectingLine = collision.GetComponent<ConnectingLine>();
                noteManager.DestroyConnectingLine(connectingLine);
            }
        }
    }
}

