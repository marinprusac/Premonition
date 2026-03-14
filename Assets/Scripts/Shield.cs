using UnityEngine;

public class Shield : MonoBehaviour
{
    public ParticleSystem braceEffect;
    
    public void Brace()
    {
        braceEffect.Play();
    }

    public void Unbrace()
    {
        braceEffect.Stop();
    }
}