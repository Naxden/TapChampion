using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScene.Player.Button;

namespace GameScene.Notes
{
    public class MoveNote : MonoBehaviour
    {
        [SerializeField]
        Button buttonPosition;
        [SerializeField]
        float speed;
        Vector3 direction;

        public void SetButton(Button button)
        {
            buttonPosition = button;
        }

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        void Start()
        {
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