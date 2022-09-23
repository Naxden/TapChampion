using UnityEngine;
using GameScene.Player.Button;
using System.Collections;
using Unity.VisualScripting;
using System.Linq.Expressions;


namespace GameScene.Notes.NoteManager
{
    public class NoteManager : MonoBehaviour
    {
        [SerializeField]
        GameObject notePrefab;
        [SerializeField]
        Button[] buttons;
        [SerializeField]
        Vector3[] noteStartingPositions;

        int noteCount;
        bool spawn = true;

        void Start()
        {
            StartCoroutine(generateNotesRoutine());
        }

        private IEnumerator generateNotesRoutine()
        {
            while (spawn)
            {
                yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));

                noteCount++;
                int index = Random.Range(0, 5);
                GameObject note = Instantiate(notePrefab, noteStartingPositions[index], Quaternion.identity);
                MoveNote noteMN = note.GetComponent<MoveNote>();
                noteMN.SetButton(buttons[index]);
                noteMN.SetSpeed(2f);
                note.transform.SetParent(transform, false);
                note.name = "Note " + noteCount.ToString();
            }
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
                spawn = false;

            if (Input.GetKeyUp(KeyCode.Backspace))
                ClearAllNotes();
        }

        void ClearAllNotes()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}

