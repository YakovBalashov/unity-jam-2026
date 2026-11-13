using System.Collections.Generic;
using RacingGame.AI;
using RacingGame.UI;
using UnityEngine;
using Random = System.Random;
using RacingGame.Extensions;

namespace RacingGame.Core
{
    public class RaceManager : MonoBehaviour
    {
        [SerializeField] private RaceTrack raceTrack;
        [SerializeField] private GameObject playerCar;
        [SerializeField] private PilotSelection pilotSelection;
        [SerializeField] private RaceResults raceResultsUI;
        [SerializeField] private GameObject raceUI;
        [SerializeField] private int playerStartingPosition = 4;

        [SerializeField] private GameObject playerCamera;
        [SerializeField] private GameObject finishCamera;
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private List<GameObject> aiCarPrefabs;
        
        private readonly List<CarTracker> _cars = new();
        private readonly Dictionary<CarTracker, float> _finishedCars = new();
        private float _startTime;
        private readonly List<(int position, string name, float time)> _results = new();
        private bool _playerFinished = false;

        private readonly Random _random = new();

        private void Start()
        {
            var safePlayerIndex = Mathf.Clamp(playerStartingPosition - 1, 0, raceTrack.StartingPositions.Count - 1);

            var listOfPilots = CreateAIPilotList(pilotSelection);
            var currentAIPrefabIndex = 0;

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
                    if (listOfPilots.Count == 0) break;
                    var randomPilot = listOfPilots[_random.Range(0, listOfPilots.Count)];
                    listOfPilots.Remove(randomPilot);
                    CreateAIPilot(startingPosition, randomPilot, aiCarPrefabs[currentAIPrefabIndex]);
                    currentAIPrefabIndex++;
                }
            }

            _startTime = Time.time;
        }

        private static List<AIPilotParameters> CreateAIPilotList(PilotSelection selection)
        {
            var pilots = new List<AIPilotParameters>();
            foreach (var pilot in selection.Pilots)
            {
                for (var i = 0; i < pilot.Value; i++)
                {
                    pilots.Add(pilot.Key);
                }
            }

            return pilots;
        }

        private void CreateAIPilot(Transform startingPosition, AIPilotParameters pilotParameters, GameObject prefab)
        {
            var aiCarInstance = Instantiate(prefab, startingPosition.position, startingPosition.rotation);

            var aiCarTracker = aiCarInstance.GetComponent<CarTracker>();
            _cars.Add(aiCarTracker);
            aiCarTracker.Initialize(raceTrack);

            aiCarInstance.GetComponent<CarAIController>().Initialize(raceTrack,
                _random.RandomizeValue(pilotParameters.SteeringSensitivity, pilotParameters.SteeringSensitivityOffset), 
                pilotParameters.ActivatePercentage, pilotParameters.ActivatePercentageOffset);

            aiCarInstance.GetComponent<CarMovement>().Initialize(
                _random.RandomizeValue(pilotParameters.MaxSpeed, pilotParameters.MaxSpeedOffset),
                _random.RandomizeValue(pilotParameters.Acceleration, pilotParameters.AccelerationOffset));
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
                raceUI.SetActive(false);
                _playerFinished = true;
            }

            if (!_playerFinished) return;
            raceResultsUI.Show(_results);
        }
    }
}