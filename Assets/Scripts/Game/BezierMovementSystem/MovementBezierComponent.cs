using System;
using System.Text;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace BezierMovementSystem
{
    public class MovementBezierComponent : MonoBehaviour, IMovementBezierComponent
    {
        [SerializeField] private BezierCurve[] _bezierCorves = new BezierCurve[1];

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
        }

        public void StopMovement()
        {
            if(_isMoving == true && _currentMovingSequence != null)
            {
                _currentMovingSequence.Kill();
            }
        }
    }

    [Serializable]
    public class BezierCurve
    {
        [SerializeField] private float _duration = 1.0f;
        [SerializeField] private Vector3 _startPosition = Vector3.zero;
        [SerializeField] private Vector3 _centralPosition = Vector3.zero;
        [SerializeField] private Vector3 _endPosition = Vector3.zero;
        [SerializeField] private Ease _moveEase = default;

        public float Duration { get => _duration; }
        public Vector3 StartPositions { get => _startPosition; }
        public Vector3 CentralPositions { get => _centralPosition; }
        public Vector3 EndPositions { get => _endPosition; }
        public Ease MoveEase { get => _moveEase; }

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