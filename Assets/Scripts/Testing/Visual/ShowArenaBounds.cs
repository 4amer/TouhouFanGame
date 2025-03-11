using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Testing
{
    public class ShowArenaBounds : MonoBehaviour
    {
        private Vector3 TopRightCorner = new Vector3(10.3f, 5.8f, 0f);
        private Vector3 DownRightCorner = new Vector3(10.3f, -5.8f, 0f);
        private Vector3 TopLeftCorner = new Vector3(-10.3f, 5.8f, 0f);
        private Vector3 DownLeftCorner = new Vector3(-10.3f, -5.8f, 0f);

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3[] windowsEdges = new Vector3[5];

            windowsEdges[0] = TopRightCorner;
            windowsEdges[1] = DownRightCorner;
            windowsEdges[2] = DownLeftCorner;
            windowsEdges[3] = TopLeftCorner;
            windowsEdges[4] = TopRightCorner;

            Handles.color = Color.white;
            Handles.DrawAAPolyLine(3, windowsEdges);
        }
#endif
    }
}
