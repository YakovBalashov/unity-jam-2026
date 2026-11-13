using System.Collections.Generic;
using UnityEngine;

namespace RacingGame.Core
{
    public class RaceManager : MonoBehaviour
    {
        [SerializeField] private List<CarTracker> cars;
        [SerializeField] private RaceTrack raceTrack;

        private void Start()
        {
            foreach (var carTracker in cars)
            {
                carTracker.Initialize(raceTrack);
            }
        }

        private void Update()
        {
            if (cars == null || cars.Count == 0) return;

            cars.Sort();

            for (var i = 0; i < cars.Count; i++)
            {
                cars[i].CurrentPosition = i + 1;
            }
        }
    }
}
