using System.Collections.Generic;
using RacingGame.AI;
using UnityEngine;

namespace RacingGame.Core
{
    public class RaceManager : MonoBehaviour
    {
        [SerializeField] private RaceTrack raceTrack;
        [SerializeField] private GameObject playerCar;
        [SerializeField] private GameObject aiCarPrefab;
        [SerializeField] private int playerStartingPosition = 4;
        private readonly List<CarTracker> _cars = new();

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
    }
}