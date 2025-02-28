using System;
using System.Text;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor.ShaderGraph.Internal;
using System.Collections.Generic;
using UnityEditor.Search;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BezierMovementSystem
{
    public class MovementBezierComponent : MonoBehaviour, IMovementBezierComponent
    {
        [Header("Movement")]
        [SerializeField] private List<BezierCurve> _bezierCorves = new List<BezierCurve>();

        [Space(10)]
        [Header("Editor")]
        [SerializeField] private int _linesInTheCurve = 10;
        [SerializeField] private float _pointSphereRadious = 2;

        private int _currentCurve = 0;
        private bool _isMoving = false;

        private GameObject _enemy = null;

        private Sequence _currentMovingSequence = null;

        private Subject<Unit> MoveCompleted;

        public void Init(GameObject entity)
        {
            _enemy = entity;
        }

        private void MoveAlongCurve(BezierCurve bezierCurve)
        {
            float interpolateValue = 0f;
            Transform _enemyTransform = _enemy.transform;

            _currentMovingSequence = DOTween.Sequence();
            _currentMovingSequence.Pause();
            _currentMovingSequence.Append(DOTween.To(() => interpolateValue, t =>
            {
                interpolateValue = t;
                _enemyTransform.position = bezierCurve.GetPoint(t);

            }, 1, bezierCurve.Duration)
                .SetEase(bezierCurve.MoveEase)
                .OnComplete(() =>
                {
                    _isMoving = false;
                    MoveCompleted?.OnNext(Unit.Default);
                }));

            _currentMovingSequence.Play();
        }

        public void StartMovement()
        {
            _isMoving = true;

            BezierCurve bezierCurve = _bezierCorves[_currentCurve];

            MoveAlongCurve(bezierCurve);

            _currentCurve += 1;
        }

        public void StopMovement()
        {
            if(_isMoving == true && _currentMovingSequence != null)
            {
                _currentMovingSequence.Kill();
            }
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            List<Vector3> allPoints = new List<Vector3>();

            foreach (BezierCurve bezierCurve in _bezierCorves)
            {
                if (bezierCurve.IsHideGizmos) continue;

                Gizmos.DrawSphere(bezierCurve.StartPositions, _pointSphereRadious);
                Gizmos.DrawSphere(bezierCurve.CentralPositions, _pointSphereRadious);
                Gizmos.DrawSphere(bezierCurve.EndPositions, _pointSphereRadious);
                for (int i = 0; i < _linesInTheCurve + 1; i++)
                {
                    float t = (float)i / (float)_linesInTheCurve;
                    Vector3 point = bezierCurve.GetPoint(t);
                    allPoints.Add(point);
                }
            }

            Handles.color = Color.red;
            Handles.DrawAAPolyLine(3, allPoints.ToArray());
#endif
        }
    }

    [Serializable]
    public class BezierCurve
    {
        [SerializeField] private float _duration = 1.0f;
        [SerializeField] private Transform _startPosition = default;
        [SerializeField] private Transform _centralPosition = default;
        [SerializeField] private Transform _endPosition = default;
        [SerializeField] private Ease _moveEase = default;
        [SerializeField] private bool _hideGizmos = false;

        public float Duration { get => _duration; }
        public Vector3 StartPositions { get => _startPosition.position; }
        public Vector3 CentralPositions { get => _centralPosition.position; }
        public Vector3 EndPositions { get => _endPosition.position; }
        public Ease MoveEase { get => _moveEase; }
        public bool IsHideGizmos { get => _hideGizmos; }

        public Vector3 GetPoint(float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 p = uu * StartPositions;
            p += 2 * u * t * CentralPositions;
            p += tt * EndPositions;

            return p;
        }
    }

    public interface IMovementBezierComponent
    {
        public void Init(GameObject entity);
        public void StartMovement();
        public void StopMovement();
    }
}