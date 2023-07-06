using UnityEngine;

namespace GameScene
{
    [RequireComponent(typeof(LineRenderer))]
    public class LongNoteConnector : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer myLineRenderer;
            
        public void EnableLine()
        {
            myLineRenderer.enabled = true;
        }

        public void SetBegin(Vector3 begin)
        {
            myLineRenderer.SetPosition(0, begin);
        }

        public void SetEnd(Vector3 end)
        {
            myLineRenderer.SetPosition(1, end);
        }
    }

}