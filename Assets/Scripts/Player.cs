using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Player : MonoBehaviour
{
    
    public Actor actor;
    
    public Sword sword;
    public Shield shield;
    
    
    private void Awake()
    {
        actor = GetComponent<Actor>();
    }
    
    
    
}