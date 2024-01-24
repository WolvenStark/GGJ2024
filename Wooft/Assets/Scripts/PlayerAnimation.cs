using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    public string[] staticDirections = { "Static N", "Static NW", "Static W", "Static SW", "Static S", "Static SE", "Static E", "Static NE" };
    public string[] runDirections = { "Run N", "Run NW", "Run W", "Run SW", "Run S", "Run SE", "Run E", "Run NE" };

    int lastDirection;
    protected float directionalSliceCount = 8;
    protected float minimumVelocityThreshold = 0.3f;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        float result1 = Vector2.SignedAngle(Vector2.up, Vector2.right);
        Debug.Log("R1 " + result1);

        float result2 = Vector2.SignedAngle(Vector2.up, Vector2.left);
        Debug.Log("R2 " + result2);

        float result3 = Vector2.SignedAngle(Vector2.up, Vector2.down);
        Debug.Log("R3 " + result3);
    }

    // Directional animation
    public void SetDirection(Vector2 _direction)
    {
        string[] directionArray = null;

        // The character is static if their velocity is close enough to zero.
        if (_direction.magnitude < minimumVelocityThreshold)
        {
            directionArray = staticDirections;
        }
        else
        {
            directionArray = runDirections;

            // Gets the index of the slcie from the direction vector
            lastDirection = DirectionToIndex(_direction); 
        }

        anim.Play(directionArray[lastDirection]);
    }

    // Converts a Vector2 direction to an index to a slcie around a circle (anti-clockwise)
    private int DirectionToIndex(Vector2 _direction)
    {
        // Return this vector with a magnitude of 1 and get the normalized to an index
        Vector2 norDir = _direction.normalized; 

        // Calculate the degrees within a single slice / direction
        float step = 360 / directionalSliceCount;
        float offset = step / 2;

        // Returns the signed angle in degrees between A and B
        float angle = Vector2.SignedAngle(Vector2.up, norDir);

        angle += offset;

        // Wrap around negative number
        if (angle < 0)
        {
            angle += 360;
        }

        float stepCount = angle / step;
        return Mathf.FloorToInt(stepCount);
    }
}
