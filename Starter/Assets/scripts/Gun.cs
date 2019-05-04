using UnityEngine;

/// <summary>
/// Fires given bullet prefab at the specified bulletSpeed.
/// </summary>
public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;

    public int bulletSpeed;

    public void FireBullet(Vector3 position, Vector3 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab) as GameObject;

        bullet.transform.position = position;

        bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;
    }
}
