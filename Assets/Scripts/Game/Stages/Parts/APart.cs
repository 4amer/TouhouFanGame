using System;
using UniRx;
using UnityEngine;
using Unity.VisualScripting;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Stages.Parts
{
    public abstract class APart : MonoBehaviour, IDisposable
    {
        [SerializeField] private Color _partColor = Color.white;
        [SerializeField] private string _partName = string.Empty;

        public Subject<APart> PartClear = new Subject<APart>();
        public Subject<APart> PartStarted = new Subject<APart>();

        private Vector3 TopRightCorner = new Vector3(10.3f, 5.8f, 0f);
        private Vector3 DownRightCorner = new Vector3(10.3f, -5.8f, 0f);
        private Vector3 TopLeftCorner = new Vector3(-10.3f, 5.8f, 0f);
        private Vector3 DownLeftCorner = new Vector3(-10.3f, -5.8f, 0f);

        protected CompositeDisposable disposable = new CompositeDisposable();
        public Color PartColor => _partColor;
        public string PartName => _partName;
        public virtual void Init()
        {
            PartClear.OnNext(this);
        }

        public virtual void Clear()
        {
            PartClear.OnNext(this);
        }

        public virtual void TimerUpdated(float time)
        {

        }

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
            Handles.DrawAAPolyLine(3,windowsEdges);
        }
#endif

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}
