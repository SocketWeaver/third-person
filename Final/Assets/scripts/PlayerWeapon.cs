using UnityEngine;
using SWNetwork;

public class PlayerWeapon : MonoBehaviour
{
    // aimable layers
    public LayerMask layerMask;

    private Vector3 currentLookTarget = Vector3.zero;
    public Gun gun;

    // launch position of bulletes
    public Transform launchPosition;

    NetworkID networkId;
    RemoteEventAgent remoteEventAgent;

    private void Start()
    {
        networkId = GetComponent<NetworkID>();
        remoteEventAgent = gameObject.GetComponent<RemoteEventAgent>();
    }

    void FixedUpdate()
    {
        if (networkId.IsMine)
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
    }

    void Update()
    {
        if (networkId.IsMine)
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
    }

    void FireBullet()
    {
        SWNetworkMessage msg = new SWNetworkMessage();
        msg.Push(launchPosition.position);
        msg.Push(transform.forward);
        msg.PushUTF8ShortString(NetworkClient.Instance.PlayerId);
        remoteEventAgent.Invoke("fire", msg);
    }

    public void RemoteFire(SWNetworkMessage msg)
    {
        Vector3 position = msg.PopVector3();
        Vector3 direction = msg.PopVector3();
        string ownerId = msg.PopUTF8ShortString();
        gun.FireBullet(position, direction, ownerId);
    }
}
