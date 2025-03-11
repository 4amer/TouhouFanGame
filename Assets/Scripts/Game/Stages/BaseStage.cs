using System.Collections.Generic;
using Stages.Manager;
using Stages.Parts;
using Stages.Parts.Selection;
using UniRx;
using UnityEngine;
using Zenject;

namespace Stages
{
    [CreateAssetMenu(fileName = "BaseStage", menuName = "TouhouLike/Stages/BaseStage", order = 1)]
    public class BaseStage : ScriptableObject, IBaseStage
    {
        [SerializeField] private SelectionPart _selectablePart = null; 

        [SerializeField] private APart[] initialParts = new APart[0];
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

        private CompositeDisposable _disposable = new CompositeDisposable();

        private Transform _partParent = default;

        private IStageManagerTimer _managerActions = null;

        private DiContainer _diContainer = null;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public void Init(Transform partParent, IStageManagerTimer managerActions)
        {
            //CheckForArrayAmount();

            _partParent = partParent;

            _managerActions = managerActions;

            _managerActions
                .TimeChanged
                .Subscribe(_ => OnTimerUpdated(_))
                .AddTo(_disposable);

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
                PrepareSelectablePart(parts);
                return;
            }

            PreparePart(parts);
        }

        private void PreparePart(APart[] parts)
        {
            APart part = TakeRandomPart(parts);

            PreparePart(part);
        }

        private void PreparePart(APart part)
        {
            _currentPart = CreatePart(part);

            /*_managerActions
                .TimeChanged
                .Subscribe(_ => _currentPart.TimerUpdated(_))
                .AddTo(_disposables);*/

            _currentPart.PartClear
                .Subscribe(_ => NextPart())
                .AddTo(_disposable);

            _currentPart.Init();
        }

        private void PrepareSelectablePart(APart[] parts)
        {
            APart[] selectParts = PreparePartsForSelect(parts);
            SelectionPart selectPartObject = (SelectionPart)CreatePart(_selectablePart);

            selectPartObject.PrepareParts(selectParts);

            selectPartObject.OnPartSelected
                .Subscribe(part => {
                    DestroyCurrentPart();
                    PreparePart(part);
                })
                .AddTo(_disposable);

            selectPartObject.Init();

            _currentPart = selectPartObject;
        }

        private void OnTimerUpdated(float time)
        {
            
        }

        private APart[] PreparePartsForSelect(APart[] parts)
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
            _diContainer.InjectGameObject(partObject);
            return partObject.GetComponent<APart>();
        }

        private void DestroyCurrentPart()
        {
            _disposable.Clear();
            _currentPart.Dispose();
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
        public void Init(Transform partParent, IStageManagerTimer managerActions);
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