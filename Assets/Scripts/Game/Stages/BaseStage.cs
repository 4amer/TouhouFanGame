using System.Collections.Generic;
using Stages.Manager;
using Stages.Parts;
using UniRx;
using UnityEngine;

namespace Stages
{
    [CreateAssetMenu(fileName = "BaseStage", menuName = "TouhouLike/Stages/BaseStage", order = 1)]
    public class BaseStage : ScriptableObject, IBaseStage
    {
        [SerializeField] private GameplayPart[] initialParts = new GameplayPart[0];
        [SerializeField] private GameplayPart[] secondaryParts = new GameplayPart[0];
        [SerializeField] private APart[] firstPassiveParts = new APart[0];
        [SerializeField] private GameplayPart[] tertioryParts = new GameplayPart[0];
        [SerializeField] private GameplayPart[] quaternaryParts = new GameplayPart[0];
        [SerializeField] private APart[] secondPassiveParts = new APart[0];
        [SerializeField] private BossPart bossPart = default;

        public Subject<APart> PartClear { get; set; } = new Subject<APart>();
        public Subject<Unit> SelectingPart { get; set; } = new Subject<Unit>();
        public Subject<APart> PartInit { get; set; } = new Subject<APart>();
        public Subject<IBaseStage> StageClear { get; set; } = new Subject<IBaseStage>();
        public Subject<IBaseStage> StageInited { get; set; } = new Subject<IBaseStage>();

        private const int SELECTABLE_PARTS_AMOUNT = 2;

        private PartSteps _currentStep = PartSteps.None;
        private APart _currentPart = default;

        private CompositeDisposable _disposables = new CompositeDisposable();

        private Transform _partParent = default;

        private IStageManagerActions _managerActions = null;

        public void Init(Transform partParent, IStageManagerActions managerActions)
        {
            CheckForArrayAmount();

            _partParent = partParent;

            _managerActions = managerActions;

            _managerActions
                .TimeChanged
                .Subscribe(_ => OnTimerUpdated(_))
                .AddTo(_disposables);

            InitFirstPart();
        }

        private void InitFirstPart()
        {
            _currentStep = PartSteps.Initial;

            PreparePart(initialParts);
        }

        private void NextPart()
        {
            DestroyCurrentPart();

            _currentStep += 1;

            APart[] parts = FindAPartVersions();

            if (_currentStep == PartSteps.FirstPassive || _currentStep == PartSteps.SecondPassive)
            {
                OnPartSelected(parts);
                return;
            }

            PreparePart(parts);
        }

        private void PreparePart(APart[] parts)
        {
            APart part = TakeRandomPart(parts);

            CreatePart(part);

            _managerActions
                .TimeChanged
                .Subscribe(_ => part.TimerUpdated(_))
                .AddTo(_disposables);

            part.PartClear
                .Subscribe(_ => NextPart())
                .AddTo(_disposables);

            part.Init();

            _currentPart = part;
        }

        private void PreparePart(APart part)
        {
            CreatePart(part);



            part.PartClear
                .Subscribe(_ => NextPart())
                .AddTo(_disposables);

            part.Init();

            _currentPart = part;
        }

        private void OnPartSelected(APart[] parts)
        {
            PrepareSelectableParts(parts);
        }

        private void OnTimerUpdated(float time)
        {
            Debug.Log($"New time = {time}");
        }

        private APart[] PrepareSelectableParts(APart[] parts)
        {
            List<APart> partsList = new List<APart>(parts);
            List<APart> selectableParts = new List<APart>();

            for (int i = 0; i < SELECTABLE_PARTS_AMOUNT; i++)
            {
                int partIndex = Random.Range(0, partsList.Count);
                APart part = partsList[partIndex];
                selectableParts.Add(part);
                partsList.Remove(part);
            }

            return selectableParts.ToArray();
        }

        private void PrepareSelectedPart(APart part)
        {
            PreparePart(part);
        }

        private APart[] FindAPartVersions()
        {
            APart[] parts = new APart[1];

            switch (_currentStep)
            {
                case PartSteps.Initial:
                    parts = initialParts;
                    break;
                case PartSteps.Secondary:
                    parts = secondaryParts;
                    break;
                case PartSteps.FirstPassive:
                    parts = firstPassiveParts;
                    break;
                case PartSteps.Tertiory:
                    parts = tertioryParts;
                    break;
                case PartSteps.Quaternary:
                    parts = quaternaryParts;
                    break;
                case PartSteps.SecondPassive:
                    parts = secondPassiveParts;
                    break;
                default:
                    parts = new APart[] { bossPart };
                    break;
            }

            return parts;
        }

        private APart TakeRandomPart(APart[] AParts)
        {
            int index = Random.Range(0, AParts.Length);
            return AParts[index];
        }

        private APart CreatePart(APart aPart)
        {
            //TODO: Rewrite and use Facroty instead

            GameObject partObject = Instantiate(aPart.gameObject, _partParent);
            return partObject.GetComponent<APart>();
        }

        private void DestroyCurrentPart()
        {
            _disposables.Clear();
            Destroy(_currentPart.gameObject);
        }

        private void CheckForArrayAmount()
        {
            if (initialParts.Length == 0)
            {
                Debug.LogError($"Initial parts contains 0 elements!");
                return;
            }
            if (secondaryParts.Length == 0)
            {
                Debug.LogError($"Secondary parts contains 0 elements!");
                return;
            }
            if (firstPassiveParts.Length < SELECTABLE_PARTS_AMOUNT)
            {
                Debug.LogError($"First passive parts contains less then 2 elements!");
                return;
            }
            if (tertioryParts.Length == 0)
            {
                Debug.LogError($"Tertiory parts contains 0 elements!");
                return;
            }
            if (quaternaryParts.Length == 0)
            {
                Debug.LogError($"Quaternary parts contains 0 elements!");
                return;
            }
            if (secondPassiveParts.Length < SELECTABLE_PARTS_AMOUNT)
            {
                Debug.LogError($"Second passive parts contains less then 2 elements!");
                return;
            }
            if (bossPart == null)
            {
                Debug.LogError($"bossPart is null!");
                return;
            }
        }
    }

    public interface IBaseStage
    {
        public Subject<APart> PartClear { get; set; }
        public Subject<APart> PartInit { get; set; }
        public Subject<IBaseStage> StageClear { get; set; }
        public Subject<IBaseStage> StageInited { get; set; }
        public void Init(Transform partParent, IStageManagerActions managerActions);
    }
    public enum PartSteps
    {
        None = 0,
        Initial = 1,
        Secondary = 2,
        FirstPassive = 3,
        Tertiory = 4,
        Quaternary = 5,
        SecondPassive = 6,
        Boss = 7
    }
}