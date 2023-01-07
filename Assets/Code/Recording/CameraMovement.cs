using UnityEngine;

namespace Recording
{
    public class CameraMovement : MonoBehaviour
    {
        // function called externally by Silder's Event
        public void MoveCamera(float time)
        {
            time *= 10;
            transform.position = new Vector3(time, 0f, -10f);
        }
    }
}
