using UnityEngine;

public class Explosion : MonoBehaviour
{
    void Start()
    {
        var exp = GetComponent<ParticleSystem>();
        if (exp != null)
        {
            exp.Play();

            // destroy when particle effect ends
            Destroy(gameObject, exp.main.duration);
        }
    }
}
