using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RaceStatus _raceStatus = default;
    [SerializeField] private RaceFlowController _raceFlowController = default;
    [SerializeField] private PlayerInitialization _playerInitialization = default;
    [SerializeField] private StartPerformance _startPerformance = default;
    [SerializeField] private CharacterModelManager _characterModelManager = default;

    private List<RaceProgressTracker> _trackers = default;

    private void Awake()
    {
        _characterModelManager.AwakeCharacterModelManager();
        _playerInitialization.AwakePlayer(_raceStatus);
    }

    private void Start()
    {
        _playerInitialization.StartPlayer();
        _raceFlowController.StartRaceFlowController();
        _trackers = FindObjectsByType<RaceProgressTracker>(FindObjectsSortMode.None).ToList();
        foreach (RaceProgressTracker tracker in _trackers)
        {
            tracker.StartRaceProgressTracker();
        }
    }

    private void Update()
    {
        _playerInitialization.UpdatePlayer();
        _startPerformance.UpdateStartPerformance();
        foreach(RaceProgressTracker tracker in _trackers)
        {
            tracker.UpdateRaceProgressTracker();
        }
    }

    private void FixedUpdate()
    {
        _playerInitialization.FixedUpdatePlayer();
    }
}
