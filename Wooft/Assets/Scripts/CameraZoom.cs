using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    protected Camera cam;

    public bool isZoomIn;

    [Range(0.01f, 2.0f)]
    public float zoomInSize;
    [Range(0.01f, 2.0f)]
    public float zoomOutSize;
    [Range(0.01f, 0.1f)]
    public float zoomInSpeed;
    [Range(0.01f, 0.1f)]
    public float zoomOutSpeed;
    [Range(1, 3)]
    public float waitTime;

    protected float waitCounter;
    protected float minimumVelocityThreshold = 0.01f;

    private void Awake()
    {
    }

    public void Start()
    {
        cam = FindObjectOfType<CameraController>().cam;
    }

    public void Zoom(bool toggle)
    {
        cam.orthographicSize = toggle ?
            Mathf.Lerp(cam.orthographicSize, zoomInSize, zoomInSpeed) :
            Mathf.Lerp(cam.orthographicSize, zoomOutSize, zoomOutSpeed);
    }

    public void UpdateZoom(bool shouldZoomIn)
    {
        if (shouldZoomIn)
        {
            waitCounter += Time.deltaTime;
            if (waitCounter > waitTime)
            {
                isZoomIn = true;
            }
        }
        else
        {
            isZoomIn = false;
            waitCounter = 0;
        }

        Zoom(isZoomIn);
    }
}
