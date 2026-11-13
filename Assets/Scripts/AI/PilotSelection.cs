using System.Collections.Generic;
using UnityEngine;

namespace RacingGame.AI
{
    [CreateAssetMenu(fileName = "PilotSelection", menuName = "Racing/PilotSelection")]
    public class PilotSelection : ScriptableObject
    {
        [field: SerializeField] public Dictionary<AIPilotParameters, int> Pilots { get; private set; }
    }
}
