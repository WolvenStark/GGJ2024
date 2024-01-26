using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [HideInInspector]
    public Camera cam;
    private Transform target;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float minX, maxX, minY, maxY;

    private void Awake()
    {
        // Find the main player and get it's transform
        target = GameObject.FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Transform>();
        cam = gameObject.GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        var targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);
        Vector3 velocity = (targetPos - transform.position) * smoothSpeed;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1.0f, Time.deltaTime);

        // Clamp the position between the bounds
        transform.position = new Vector3
            (
                Mathf.Clamp(transform.position.x, minX, maxX),
                Mathf.Clamp(transform.position.y, minY, maxY),
                transform.position.z
            );
    }
}