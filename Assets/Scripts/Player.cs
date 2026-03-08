using UnityEngine;

[RequireComponent(typeof(MovableEntity))]
public class Player : MonoBehaviour
{
    
    public MovableEntity movableEntity;
    
    private void Start()
    {
        movableEntity = GetComponent<MovableEntity>();
    }
    
}