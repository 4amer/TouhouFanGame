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
    public class MovementComponent : MonoBehaviour, IMovementBezierComponent
    {
        [Header("Movement")]
        [SerializeField] private List<BezierCurve> _bezierCurves = new List<BezierCurve>();

        [Space(10)]
        [Header("Editor")]
        [SerializeField] private int _linesInTheCurve = 10;
        [SerializeField] private float _pointSphereRadious = 2;

        private int _currentCurve = 0;
        private bool _isMoving = false;

        private bool _isInited = false;

        private Transform _enemy = null;

        private Sequence _currentMovingSequence = null;

        public Subject<Unit> MoveCompleted = new Subject<Unit>();

        private const float ADITIONAL_TIME_TO_TIMER = 0.05f;

        public void Init(Transform entity)
        {
            if (_isInited) return;
            _isInited = true;
            _enemy = entity;
        }

        private void MoveAlongCurve(BezierCurve bezierCurve)
        {
            float interpolateValue = 0f;
            Transform _enemyTransform = _enemy;

            _currentMovingSequence = DOTween.Sequence();
            _currentMovingSequence.Pause();
            _currentMovingSequence.Append(DOTween.To(() => interpolateValue, t =>
            {
                interpolateValue = t;
                _enemyTransform.position = bezierCurve.GetPoint(t);

            }, 1, (bezierCurve.Duration + ADITIONAL_TIME_TO_TIMER))
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

            BezierCurve bezierCurve = _bezierCurves[_currentCurve];

            MoveAlongCurve(bezierCurve);

            _currentCurve += 1;
            if (_currentCurve >= _bezierCurves.Count) _currentCurve = 0;
        }

        public void StopMovement()
        {
            if(_isMoving == true && _currentMovingSequence != null)
            {
                _currentMovingSequence.Kill();
            }
        }

        [ContextMenu("Sync positions")]
        public void SynchronizeMovePositionsWithObject()
        {
            if (_bezierCurves == null || _bezierCurves.Count == 0)
                return;

            Vector3 objectPosition = transform.position;
            Vector3 initialCurveStartOffset = _bezierCurves[0].StartPositions - objectPosition; // Always zero initially

            foreach (BezierCurve curve in _bezierCurves)
            {
                Vector3 startOffset = curve.StartPositions - _bezierCurves[0].StartPositions;
                Vector3 centerOffset = curve.CentralPositions - _bezierCurves[0].StartPositions;
                Vector3 endOffset = curve.EndPositions - _bezierCurves[0].StartPositions;

                curve.StartPositions = objectPosition + startOffset;
                curve.CentralPositions = objectPosition + centerOffset;
                curve.EndPositions = objectPosition + endOffset;
            }
        }

        public void PrepareStartPosition(float timeForMovement)
        {
            BezierCurve curve = new BezierCurve();

            Vector3 StartPosition = _enemy.position;
            Vector3 EndPosition = _bezierCurves[0].StartPositions;

            Vector3 CenterPosition = (StartPosition + EndPosition) / 2f;

            curve.StartPositions = StartPosition;
            curve.CentralPositions = CenterPosition;
            curve.EndPositions = EndPosition;

            curve.Duration = timeForMovement;

            curve.MoveEase = Ease.InCubic;

            MoveAlongCurve(curve);
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            List<Vector3> allPoints = new List<Vector3>();

            foreach (BezierCurve bezierCurve in _bezierCurves)
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
        [SerializeField] private Vector3 _startPosition = default;
        [SerializeField] private Vector3 _centralPosition = default;
        [SerializeField] private Vector3 _endPosition = default;
        [SerializeField] private bool _followPlayersX = false;
        [SerializeField] private float _followPlayersXDelay = 0f;
        [SerializeField] private bool _followPlayersY = false;
        [SerializeField] private float _followPlayersYDelay = 0f;
        [SerializeField] private Ease _moveEase = default;
        [SerializeField] private bool _hideGizmos = false;

        public float Duration { get => _duration; set => _duration = value; }
        public Vector3 StartPositions { get => _startPosition; set => _startPosition = value; }
        public Vector3 CentralPositions { get => _centralPosition; set => _centralPosition = value; }
        public Vector3 EndPositions { get => _endPosition; set => _endPosition = value; }
        public Ease MoveEase { get => _moveEase; set => _moveEase = value; }
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
        public void Init(Transform entity);
        public void PrepareStartPosition(float timeForMovement);
        public void StartMovement();
        public void StopMovement();
    }
}