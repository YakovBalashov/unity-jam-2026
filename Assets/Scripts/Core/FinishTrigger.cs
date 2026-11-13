using System;
using UnityEngine;

namespace RacingGame.Core
{
    public class FinishTrigger : MonoBehaviour
    {
        public Action<CarTracker> onFinishCrossed;
        
        private void OnTriggerEnter(Collider other)
        {
            var carTracker = other.GetComponentInParent<CarTracker>();
            if (carTracker == null) return;
            onFinishCrossed?.Invoke(carTracker);
        }
    }
}
