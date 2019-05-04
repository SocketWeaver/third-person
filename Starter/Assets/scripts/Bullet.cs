using UnityEngine;

/// <summary>
/// Bullet.
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>
    /// Particle effect GameObject of the bullet
    /// </summary>
    public GameObject explode;

    public int damage = 10;

    void OnCollisionEnter(Collision collision)
    {
        // detact if the collision GameObject is player
        if (collision.gameObject.tag == "Player")
        {
            PlayerHP ps = collision.gameObject.GetComponent<PlayerHP>();

            if (ps != null)
            {
                // deduct health point
                ps.GotHit(damage);
            }
        }

        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.Log(contact.thisCollider.name + " hit " + contact.otherCollider.name);
            Instantiate(explode, contact.point, Quaternion.identity);
        }
    }
}