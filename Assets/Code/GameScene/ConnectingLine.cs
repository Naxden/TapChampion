using UnityEngine;

namespace GameScene
{
    public class ConnectingLine : MonoBehaviour
    {
        [SerializeField]
        private Vector3 startPos;
        [SerializeField]
        private Vector3 endPos;

        [SerializeField]
        private Vector3 startScale;
        [SerializeField]
        private Vector3 endScale;

        private NoteManager noteManager;
        private readonly float error = 0.1f;
        private float lerpDuration = 0f;
        private float timeElpased = 0f;

        private int notesCount;
        private float begin;
        private float end;

        public void Initialize(int begin, int end, float time, NoteManager noteManager)
        {
            lerpDuration = time;
            this.noteManager = noteManager;
            this.begin = begin / 4f;
            this.end = end / 4f;

            Resize();
        }

        private void Resize()
        {
            Renderer myRenderer = GetComponent<Renderer>();

            myRenderer.material.SetFloat("_Begin", begin);
            myRenderer.material.SetFloat("_End", end);
        }

        public void Move()
        {
            if (timeElpased <= lerpDuration)
            {
                float timeOverLerp = timeElpased / (lerpDuration - error);

                transform.position = Vector3.Lerp(startPos, endPos, timeOverLerp);
                transform.localScale = Vector3.Lerp(startScale, endScale, timeOverLerp);

                timeElpased += Time.deltaTime;
            }
            else
                noteManager.DestroyConnectingLine(this);
        }

        public void NoteRemoved()
        {
            notesCount--;

            if (notesCount <= 1)
            {
                Debug.Log("Niszcze sie");
                noteManager.DestroyConnectingLine(this);
            }
        }
    }
}
