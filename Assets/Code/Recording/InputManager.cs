using UnityEngine;
using Saving.Note;

namespace Recording.InputManager
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        NoteRenderer.NoteRenderer noteRenderer;
        private KeyCode[] keys = {KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K, KeyCode.L};
        private float[] pushTimers = new float[5];
        private float[] addNotePositions = new float[5];
        private bool[] longNotesBeginAdded = {false, false, false, false, false};
        public float noteDelay = 0.175f;
        private bool inputEnabled = false;

        public void EnableInput()
        {
            inputEnabled = true;
        }

        public void DiasableInput()
        {
            inputEnabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (inputEnabled)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    noteRenderer.AddNote(Camera.main.ScreenToWorldPoint(Input.mousePosition), NoteType.LongEnd);
                }

                CheckButtonsInput();
            }
        }

        private void CheckButtonsInput()
        {
            for (int i = 0; i < 5; i++)
            {
                if (!longNotesBeginAdded[i] && pushTimers[i] > noteDelay)
                {
                    PlaceNoteByKey(i, true);
                    longNotesBeginAdded[i] = true;
                }
                if (Input.GetKeyDown(keys[i]))
                    addNotePositions[i] = transform.position.x - 5f;
                if (Input.GetKey(keys[i]))
                    pushTimers[i] += Time.deltaTime;
                if (Input.GetKeyUp(keys[i]))
                {
                    if (longNotesBeginAdded[i])
                        addNotePositions[i] = transform.position.x - 5f;

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
        // external Slider calling the method
        public void MoveCamera(float time)
        {
            // accuracy to 0.1 of a second
            // powiedzmy że alokuje 3000 GO dla każdego toru, i maks długość piosenki to 5 minut
            // set propper multipler
            time *= 10;
            transform.position = new Vector3(time, 0f, -10f);
        }

    }
}
