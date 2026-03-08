using UnityEngine;

[CreateAssetMenu(fileName = "MovementStats", menuName = "Movement Stats", order = 0)]
public class MovementStats : ScriptableObject
{
    public int movementRange = 1;
    public int climbHeight = 1;
    public int dropHeight = 1;
}