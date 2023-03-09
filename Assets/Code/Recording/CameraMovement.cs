using UnityEngine;

namespace Recording
{
    public class CameraMovement : MonoBehaviour
    {
        private const float noteSpace = 0.75f;
        // function called externally by Silder's Event
        public void MoveCamera(float time)
        {
            time *= noteSpace * 10f;
            
            transform.position = new Vector3(time, 0f, -10f);
        }
    }
}
