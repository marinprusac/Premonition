using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "WeaponStats", menuName = "ScriptableObjects/WeaponStats", order = 1)]
    public class WeaponStats : ScriptableObject
    {
        public int minRange;
        public float maxRange;
        public int baseDamage;
        public float heightBenefit;
    }
}