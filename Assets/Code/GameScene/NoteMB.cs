using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving;

namespace GameScene
{
    public class NoteMB : MonoBehaviour
    {

        [SerializeField]
        NoteType noteType;

        [SerializeField]
        Button buttonPosition;

        private float velocity = 3.5f;
        Vector3 direction;

        public NoteType GetNoteType()
        {
            return noteType;
        }

        public void Initialize(Vector3 startingPos, float velocity, NoteType noteType, Button targetButton)
        {
            transform.position = startingPos;
            this.velocity = velocity;
            buttonPosition = targetButton;
            this.noteType = noteType;

            Vector3 pos = buttonPosition.transform.position;
            direction = new Vector3(pos.x - transform.position.x, pos.y - transform.position.y);
            direction = Vector3.Normalize(direction);
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
        }

        //private void OnDrawGizmos()
        //{
        //    UnityEditor.Handles.color = Color.black;
        //    UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, 0.128f * 0.25f);
        //}
    }
}