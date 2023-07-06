using UnityEngine;

namespace Recording.Note
{
    [RequireComponent(typeof(LineRenderer))]
    public class LongNoteConnector : MonoBehaviour
    {
        [SerializeField]
        private Transform end;

        private LineRenderer lineRenderer;
    
        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        public void UpdateLine(float uselessVal)
        {
            var position = transform.position;

            lineRenderer.SetPosition(0, position);
            lineRenderer.SetPosition(1, new Vector3(end.position.x, position.y));
        }

        public void SetEnd(Transform end)
        {
            this.end = end;
        }
    }

}
