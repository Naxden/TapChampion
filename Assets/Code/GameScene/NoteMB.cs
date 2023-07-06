using UnityEngine;
using Saving;

namespace GameScene
{
    public class NoteMB : MonoBehaviour
    {
        [SerializeField]
        protected NoteType noteType;

        private Vector3 startPosition;
        [SerializeField]
        Button buttonPosition;
        private int buttonIndex = -1;

        private float velocity = 3.5f;
        Vector3 direction;

        private ConnectingLine connectingLine = null;
        private float travelDistance;

        private LongNoteConnector noteConnector;

        public NoteType GetNoteType()
        {
            return noteType;
        }

        public int GetButtonIndex()
        {
            return buttonIndex;
        }

        public void SetConnectingLine(ConnectingLine connectingLine)
        {
            this.connectingLine = connectingLine;
            connectingLine.AddNote(this);
        }

        public void Initialize(Vector3 startingPos, float velocity, NoteType noteType, int buttonIndex, Button targetButton)
        {
            this.startPosition = startingPos;
            transform.position = startingPos;
            this.velocity = velocity;
            this.buttonIndex = buttonIndex;
            buttonPosition = targetButton;
            this.noteType = noteType;

            Vector3 pos = buttonPosition.transform.position;
            direction = new Vector3(pos.x - transform.position.x, pos.y - transform.position.y);
            direction = Vector3.Normalize(direction);

            travelDistance = Vector3.Distance(startPosition, targetButton.transform.position);
        }

        public void UpdateVisuals(GameObject notePrefab)
        {
            if (!notePrefab)
            {
                Debug.LogWarning("UpdateVisuals: Prefab was empty");
                return;
            }

            GetComponent<Animator>().runtimeAnimatorController = 
                notePrefab.GetComponent<Animator>().runtimeAnimatorController;
            
            var mySR = GetComponent<SpriteRenderer>();
            var otherSR = notePrefab.GetComponent<SpriteRenderer>();
            mySR.sprite = otherSR.sprite;
            mySR.color = otherSR.color;
        }

        public void Move()
        {
            transform.Translate(velocity * Time.deltaTime * direction);

            if (noteConnector != null)
            {
                if (noteType == NoteType.LongBegin)
                    noteConnector.SetBegin(transform.position);
                else if (noteType == NoteType.LongEnd)
                    noteConnector.SetEnd(transform.position);
            }
        }

        public float GetTravelStatus()
        {
            float travelCurrentDistance = Vector3.Distance(startPosition, transform.position);

            return travelCurrentDistance / travelDistance;
        }

        public LongNoteConnector GetNoteConnector()
        {
            return noteConnector;
        }

        public void SetNoteConnector(LongNoteConnector noteConnector)
        {
            this.noteConnector = noteConnector;
        }

        public void NoteRemoved()
        {
            if (connectingLine != null)
                connectingLine.NoteRemoved(this);

            if (noteType == NoteType.LongEnd)
            {
                Destroy(noteConnector.gameObject);
                noteConnector = null;
            }
        }

        //private void OnDrawGizmos()
        //{
        //    UnityEditor.Handles.color = Color.black;
        //    UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, 0.128f * 0.25f);
        //}
    }
}