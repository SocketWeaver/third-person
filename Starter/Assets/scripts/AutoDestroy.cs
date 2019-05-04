using UnityEngine;

/// <summary>
/// Automatically destroys the GameObject on becoming invisible and Collision.
/// </summary>
public class AutoDestroy : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
