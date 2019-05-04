using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{

    public GameObject target;       //Public variable to store a reference to the player game object
    public float damping = 0.5f;
    public Vector3 offset;

    void Start()
    {
        // offset = player.transform.position - transform.position;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.transform.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, damping);
            transform.position = smoothedPosition;

            transform.LookAt(target.transform);
        }
    }
}