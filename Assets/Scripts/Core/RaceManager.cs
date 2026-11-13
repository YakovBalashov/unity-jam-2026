using System.Collections.Generic;
using RacingGame.AI;
using RacingGame.UI;
using UnityEngine;

namespace RacingGame.Core
{
    public class RaceManager : MonoBehaviour
    {
        [SerializeField] private RaceTrack raceTrack;
        [SerializeField] private GameObject playerCar;
        [SerializeField] private GameObject aiCarPrefab;
        [SerializeField] private RaceResults raceResultsUI;
        [SerializeField] private int playerStartingPosition = 4;
        
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private GameObject finishCamera;
        [SerializeField] private Canvas mainCanvas;
        
        private readonly List<CarTracker> _cars = new();
        private readonly Dictionary<CarTracker, float> _finishedCars = new();
        private float _startTime;
        private readonly List<(int position, string name, float time)> _results = new();
        private bool _playerFinished = false;

        private void Start()
        {
            var safePlayerIndex = Mathf.Clamp(playerStartingPosition - 1, 0, raceTrack.StartingPositions.Count - 1);
            for (var i = 0; i < raceTrack.StartingPositions.Count; i++)
            {
                var startingPosition = raceTrack.StartingPositions[i];
                if (i == safePlayerIndex)
                {
                    if (playerCar.TryGetComponent<Rigidbody>(out var rb))
                    {
                        rb.position = startingPosition.position;
                        rb.rotation = startingPosition.rotation;

                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
        
                        Physics.SyncTransforms();
                    }
                    
                    var playerCarTracker = playerCar.GetComponent<CarTracker>();
                    _cars.Add(playerCarTracker);
                    playerCarTracker.Initialize(raceTrack);
                }
                else
                {
                    var aiCarInstance = Instantiate(aiCarPrefab, startingPosition.position, startingPosition.rotation);
                    var aiCarTracker = aiCarInstance.GetComponent<CarTracker>();
                    _cars.Add(aiCarTracker);
                    aiCarTracker.Initialize(raceTrack);
                    aiCarInstance.GetComponent<CarAIController>().Initialize(raceTrack);
                }
            }
            _startTime = Time.time;
        }

        private void Update()
        {
            if (_cars == null || _cars.Count == 0) return;

            _cars.Sort();

            for (var i = 0; i < _cars.Count; i++)
            {
                _cars[i].CurrentPosition = i + 1;
            }
        }
        
        private void OnEnable()
        {
            raceTrack.FinishTrigger.onFinishCrossed += HandleFinish;
        }

        private void OnDisable()
        {
            raceTrack.FinishTrigger.onFinishCrossed -= HandleFinish;
        }

        private void HandleFinish(CarTracker carTracker)
        {
            if (carTracker.LapsCompleted != raceTrack.LapCount) return;
            
            var finishTime = Time.time - _startTime;
            _finishedCars[carTracker] = finishTime;
            var finishPosition = _finishedCars.Count;
            _results.Add((finishPosition, carTracker.name, finishTime));
            
            if (carTracker.gameObject == playerCar)
            {
                playerCamera.SetActive(false);
                finishCamera.SetActive(true);
                mainCanvas.worldCamera = finishCamera.GetComponent<UnityEngine.Camera>();
                _playerFinished = true;
            }
            
            if (!_playerFinished) return;
            raceResultsUI.Show(_results);
        }
    }
}