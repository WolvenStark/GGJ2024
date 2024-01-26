using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    protected CameraZoom camZoom;

    public string[] staticDirections = { "Static N", "Static NW", "Static W", "Static SW", "Static S", "Static SE", "Static E", "Static NE" };
    public string[] runDirections = { "Run N", "Run NW", "Run W", "Run SW", "Run S", "Run SE", "Run E", "Run NE" };

    protected int[] staticDirectionsHash;
    protected int[] runDirectionsHash;

    protected int lastDirection = 0;
    protected float directionalSliceCount = 8;
    protected float minimumVelocityThreshold = 0.01f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        camZoom = FindObjectOfType<CameraZoom>();

        staticDirectionsHash = AnimatorStringArrayToHashArray(staticDirections);
        runDirectionsHash = AnimatorStringArrayToHashArray(runDirections);
    }

    // Directional animation
    public void SetDirection(Vector2 _direction)
    {
        // Use the hash version of the animation names
        int[] directionArray = null;

        bool isPlayerStill = (_direction.magnitude < minimumVelocityThreshold);

        // The character is static if their velocity is close enough to zero.
        if (isPlayerStill)
        {
            directionArray = staticDirectionsHash;
        }
        else
        {
            directionArray = runDirectionsHash;

            // Gets the index of the slcie from the direction vector
            lastDirection = DirectionToIndex(_direction);
        }

        camZoom.UpdateZoom(isPlayerStill);

        anim.Play(directionArray[lastDirection]);
    }

    // Converts a Vector2 direction to an index to a slcie around a circle (anti-clockwise)
    private int DirectionToIndex(Vector2 _direction)
    {
        // Return this vector with a magnitude of 1 and get the normalized to an index
        Vector2 normDir = _direction.normalized; 

        // Calculate the degrees within a single slice / direction
        float step = 360 / directionalSliceCount;
        float halfstep = step / 2;

        // Returns the signed angle in degrees between A and B
        float angle = Vector2.SignedAngle(Vector2.up, normDir);

        angle += halfstep;

        // Wrap around negative number
        if (angle < 0)
        {
            angle += 360;
        }

        float stepCount = angle / step;
        return Mathf.FloorToInt(stepCount);
    }

    /// <summary>
    /// Converts a string array to a int (animator hash) array.
    /// </summary>
    /// <param name="animationArray"></param>
    /// <returns></returns>
    public static int[] AnimatorStringArrayToHashArray(string[] animationArray)
    {
        // Allocate the same array length for our hash array
        int[] hashArray = new int[animationArray.Length];
        for (int i = 0; i < animationArray.Length; i++)
        {
            hashArray[i] = Animator.StringToHash(animationArray[i]);
        }
        return hashArray;
    }
}
