using UnityEngine;
using Saving.Note;

namespace Recording.InputManager
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        private Camera MainCamera; 
        [SerializeField]
        private Recorder recorder;
        [SerializeField]
        private NoteRenderer.NoteRenderer noteRenderer;
        private KeyCode[] keys = {KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L};
        private float[] pushTimers = new float[5];
        private float[] addNotePositions = new float[5];
        private bool[] longNotesBeginAdded = {false, false, false, false, false};
        private const float BOTTOM_BORDER = -4.6f, UPPER_BORDER = 2.1f;
        public float noteDelay = 0.175f;
        private bool inputEnabled = false;

        [SerializeField]
        private NotesHandler.NotesHandler notesHandler;
        private Vector3 startMousePos = Vector3.zero;
        [SerializeField]
        private Transform selectionBox;
        private bool isSelecting = false;    

        // function called externally by Event
        public void EnableInput()
        {
            inputEnabled = true;
        }

        // function called externally by Event
        public void DiasableInput()
        {
            inputEnabled = false;
        }

        void Update()
        {
            if (inputEnabled)
            {
                CheckMouseInput();

                CheckButtonsInput();

                CheckSpaceInput();

                if (Input.GetKeyDown(KeyCode.Escape)) { Debug.Break(); }
            }
        }

        private void CheckMouseInput()
        {
            Vector3 mousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
                MouseSingleClick(mousePosition);
            if (Input.GetMouseButton(0))
                MouseHold(mousePosition);
            if (Input.GetMouseButtonUp(0))
                MouseRelease(mousePosition);
        }

        private void MouseSingleClick(Vector3 mousePosition)
        {
            recorder.PauseSong();
            Debug.Log(mousePosition);

            GameObject clickedNote = noteRenderer.IsNoteThere(mousePosition);

            if (!notesHandler.IsEmpty() && !notesHandler.IsChild(clickedNote))
            {
                notesHandler.Deselect();
            }

            if (mousePosition.y < BOTTOM_BORDER || mousePosition.y > UPPER_BORDER)
            {
                notesHandler.Deselect();
                isSelecting = false;
                return;
            }

            notesHandler.SetStartPosition(mousePosition);
            notesHandler.AddChild(clickedNote);

            if (notesHandler.IsEmpty())
            {
                startMousePos = mousePosition;
                isSelecting = true;
            }
        }

        private void MouseHold(Vector3 mousePosition)
        {
            if (mousePosition.y < BOTTOM_BORDER || mousePosition.y > UPPER_BORDER)
                return;
            
            if (isSelecting)
                SelectingBox(mousePosition);
            else if (!notesHandler.IsEmpty())
                notesHandler.Move(mousePosition);
        }

        void SelectingBox(Vector3 mousePosition)
        {
            Vector3 lowerLeft = new Vector3(
                Mathf.Min(startMousePos.x, mousePosition.x),
                Mathf.Min(startMousePos.y, mousePosition.y),
                10f);
            Vector3 upperRight = new Vector3(
                Mathf.Max(startMousePos.x, mousePosition.x),
                Mathf.Max(startMousePos.y, mousePosition.y));

            selectionBox.position = lowerLeft;
            selectionBox.localScale = upperRight - lowerLeft;
        }

        private void MouseRelease(Vector3 mousePosition)
        {
            notesHandler.MoveEnded();

            isSelecting = false;
            ResetSelectingBox();
        }

        void ResetSelectingBox()
        {
            selectionBox.localScale = new Vector3(0.01f, 0.01f);
            selectionBox.position = new Vector3(-7f, 0f, 10f);
        }

        private void CheckButtonsInput()
        {
            float xPosition = MainCamera.transform.position.x;
            
            for (int i = 0; i < 5; i++)
            {
                if (!longNotesBeginAdded[i] && pushTimers[i] > noteDelay)
                {
                    PlaceNoteByKey(i, true);
                    longNotesBeginAdded[i] = true;
                }
                if (Input.GetKeyDown(keys[i]))
                    addNotePositions[i] = xPosition - 5f;
                if (Input.GetKey(keys[i]))
                    pushTimers[i] += Time.deltaTime;
                if (Input.GetKeyUp(keys[i]))
                {
                    if (longNotesBeginAdded[i])
                        addNotePositions[i] = xPosition - 5f;

                    PlaceNoteByKey(i, false);
                    pushTimers[i] = 0f;
                    longNotesBeginAdded[i] = false;
                }
            }
        }

        private void PlaceNoteByKey(int keyIndex, bool isLongBegin)
        {
            Vector3 position = new Vector3(addNotePositions[keyIndex],
                                            1.25f - keyIndex * 1.25f,
                                            0f);
            NoteType noteType = isLongBegin ? NoteType.LongBegin :
                                (pushTimers[keyIndex] <= noteDelay) ? 
                                NoteType.Short : NoteType.LongEnd;

            noteRenderer.AddNote(position, noteType);
        }

        void CheckSpaceInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (recorder.songIsPlaying)
                    recorder.PauseSong();
                else
                    recorder.PlaySong();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision == null)
                return;

            if (collision.CompareTag("Note"))
                notesHandler.AddChild(collision.gameObject);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision == null)
                return;

            if (collision.CompareTag("Note") && isSelecting)
                notesHandler.RemoveChild(collision.gameObject);
        }
    }
}
