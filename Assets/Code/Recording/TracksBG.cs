using UnityEngine;

namespace Recording
{
    public class TracksBG : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRenderer;

        //Function called by OnSongLoadEvent
        public void SetLineRenderer(float songLength)
        {
            lineRenderer.SetPosition(1, new Vector3(songLength * 7.5f - 5.375f + 0.375f, -1.25f, 0));
            EnableLineRenderer(true);
        }

        //Function called by OnSongSelectEvent
        public void EnableLineRenderer(bool enabled)
        {
            lineRenderer.enabled = enabled;
        }
    }

}
