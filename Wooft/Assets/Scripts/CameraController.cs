using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float minX, maxX, minY, maxY;

    private void Awake()
    {
        // Find the main player and get it's transform
        target = GameObject.FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp
            (
                transform.position, 
                new Vector3(target.position.x, target.position.y, transform.position.z), 
                smoothSpeed * Time.deltaTime
            );

        transform.position = new Vector3
            (
                Mathf.Clamp(transform.position.x, minX, maxX),
                Mathf.Clamp(transform.position.y, minY, maxY),
                transform.position.z
            );
    }
}