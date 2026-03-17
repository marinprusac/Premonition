using UnityEngine;

namespace Actors
{
    [CreateAssetMenu(fileName = "MovementStats", menuName = "ScriptableObject/Movement Stats", order = 0)]
    public class ActorStats : ScriptableObject
    {
        public int movementRange = 1;
        public int climbHeight = 1;
        public int dropHeight = 1;
        public int height = 1;
        public int maxEnergy = 5;
        public int energyRegeneration = 2;
        public int damageThreshold;

    }
}