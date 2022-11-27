using UnityEngine;
using Saving.Note;

namespace Recording.InputDetection
{
    public class InputDetection : MonoBehaviour
    {
        [SerializeField]
        NoteRenderer.NoteRenderer noteRenderer;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                noteRenderer.AddNote(Camera.main.ScreenToWorldPoint(Input.mousePosition), NoteType.LongEnd);
            }
        }

        void Test()
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(worldPosition);
        }
    }
}
