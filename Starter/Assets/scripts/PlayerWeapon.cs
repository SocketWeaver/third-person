using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    // aimable layers
    public LayerMask layerMask;

    private Vector3 currentLookTarget = Vector3.zero;
    public Gun gun;

    // launch position of bulletes
    public Transform launchPosition;

    void FixedUpdate()
    {
        // find player's cursor position in the environment
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.green);
        if (Physics.Raycast(ray, out hit, 1000, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.point != currentLookTarget)
            {
                currentLookTarget = hit.point;
            }
        }

        // ignore cursor position's y value.
        Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);

        // calculate player's new rotation
        Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position);

        // lerp
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10.0f);
    }

    void Update()
    {
        // get mouse inputs
        if (Input.GetMouseButtonDown(0))
        {
            // 0.5 seconds interval between shots
            if (!IsInvoking("FireBullet"))
            {
                InvokeRepeating("FireBullet", 0f, 0.5f);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            CancelInvoke("FireBullet");
        }
    }

    void FireBullet()
    {
        gun.FireBullet(launchPosition.position, transform.forward);
    }
}
