using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScene.Player.Button;

namespace GameScene.Notes
{
    public class Note : MonoBehaviour
    {
        public enum NoteType
        {
            Short,
            LongBegin, LongEnd
        };

        [SerializeField]
        NoteType noteType;

        [SerializeField]
        Button buttonPosition;
        [SerializeField]
        float speed;
        Vector3 direction;


        public NoteType GetNoteType()
        {
            return noteType;
        }

        public void Initialize(Vector3 startingPos, NoteType noteType, Button targetButton, float noteSpeed)
        {
            transform.position = startingPos;
            buttonPosition = targetButton;
            this.noteType = noteType;
            speed = noteSpeed;

            Vector3 pos = buttonPosition.transform.position;
            direction = new Vector3(pos.x - transform.position.x, pos.y - transform.position.y);
            direction = Vector3.Normalize(direction);
        }

        void Update()
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.black;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, 0.128f * 0.25f);
        }
    }
}