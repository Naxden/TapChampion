using UnityEngine;
using Saving.Note;
using Recording.Note.Selectable;

namespace Recording.InputManager
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        private Recorder recorder;
        [SerializeField]
        private NoteRenderer.NoteRenderer noteRenderer;
        private KeyCode[] keys = {KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L};
        private float[] pushTimers = new float[5];
        private float[] addNotePositions = new float[5];
        private bool[] longNotesBeginAdded = {false, false, false, false, false};
        public float noteDelay = 0.175f;
        private bool inputEnabled = false;

        private GameObject selectedNote;

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
            }
        }

        private void CheckMouseInput()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
                MouseSingleClick(mousePosition);
            if (Input.GetMouseButton(0))
                MousePushDown(mousePosition);
            if (Input.GetMouseButtonUp(0))
                MouseRelease(mousePosition);
        }

        private void MouseSingleClick(Vector3 mousePosition)
        {
            recorder.PauseSong();

            if (selectedNote != null)
                selectedNote.GetComponent<Selectable>().Deselect();

            if (mousePosition.y < -4.25f || mousePosition.y > 1.5f)
                return;

            selectedNote = noteRenderer.IsNoteThere(mousePosition);

            if (selectedNote != null)
            {
                selectedNote.GetComponent<Selectable>().Select();
            }
            else
            {
                Debug.Log(mousePosition);
                noteRenderer.AddNote(mousePosition, NoteType.LongEnd);
            }
        }

        private void MousePushDown(Vector3 mousePosition)
        {
            if (mousePosition.y < -4.25f || mousePosition.y > 1.5f)
                return;

            if (selectedNote != null)
                selectedNote.GetComponent<Selectable>().Move(mousePosition);
        }

        private void MouseRelease(Vector3 mousePosition)
        {
            if (mousePosition.y < -4.25f || mousePosition.y > 1.5f)
                return;
        }

        private void CheckButtonsInput()
        {
            float xPosition = transform.position.x;
            
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

        // function called externally by Silder's Event
        public void MoveCamera(float time)
        {
            time *= 10;
            transform.position = new Vector3(time, 0f, -10f);
        }

    }
}
